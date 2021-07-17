using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Cathode.Common.Api;
using Cathode.Common.Protocol;
using Cathode.Gateway.Authentication;
using Cathode.Gateway.Protocol.Index;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Cathode.Gateway.Index
{
    public class IndexService : IIndexService
    {
        private readonly GatewayDb _db;
        private readonly IAuthenticationService _authenticationService;

        public IndexService(GatewayDb db, IAuthenticationService authenticationService)
        {
            _db = db;
            _authenticationService = authenticationService;
        }

        public Task<ApiResult<PingResponse>> PingAsync(ConnectionInfo connection)
        {
            return Task.FromResult(ApiResultHelper.Success(new PingResponse
            {
                RequesterIpAddress = connection.RemoteIpAddress?.ToString(),
            }));
        }

        private static string GenerateKey()
        {
            using var cryptoProvider = new RNGCryptoServiceProvider();
            var secretKeyByteArray = new byte[64];
            cryptoProvider.GetBytes(secretKeyByteArray);
            return Convert.ToBase64String(secretKeyByteArray);
        }

        public async Task<ApiResult<RegisterResponse>> RegisterAsync(RegisterRequest request)
        {
            await _db.SaveChangesAsync();

            if (await _db.Nodes.AnyAsync(x => x.AccountId == request.AccountId && x.DeviceId == request.DeviceId))
            {
                return ApiResultHelper.BadRequest<RegisterResponse>(new ApiError(ApiErrorCode.AlreadyInUse));
            }

            try
            {
                var now = DateTime.Now;
                var id = Guid.NewGuid();
                var controlSalt = Guid.NewGuid();
                var authToken = GenerateKey();

                await _db.Nodes.AddAsync(new Node
                {
                    Id = id,
                    AccountId = request.AccountId,
                    DeviceId = request.DeviceId,
                    LookupToken = request.LookupToken,
                    AuthenticationToken = authToken,
                    ControlTokenChallenge = controlSalt,
                    FirstSeen = now,
                    LastSeen = now,
                });
                await _db.SaveChangesAsync();

                return ApiResultHelper.Success(new RegisterResponse
                {
                    RegistrationId = id,
                    ControlTokenSalt = controlSalt,
                    AuthenticationToken = authToken
                });
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is PostgresException
                {
                    SqlState: PostgresErrorCodes.UniqueViolation
                })
                {
                    return ApiResultHelper.BadRequest<RegisterResponse>(new ApiError(ApiErrorCode.AlreadyInUse));
                }

                throw;
            }
        }

        public async Task<ApiResult<UpdateResponse>> UpdateAsync(UpdateRequest request, NodeAuthEntity entity)
        {
            var node = await _db.Nodes
                .Include(x => x.ConnectionInfo)
                .SingleOrDefaultAsync(n =>
                    n.Id == Guid.Parse(entity.RegistrationId)
                );
            if (node == null)
            {
                return ApiResultHelper.Forbidden<UpdateResponse>();
            }

            var now = DateTime.Now;

            foreach (var ci in request.ConnectionInfo)
            {
                var dbci = node.ConnectionInfo.SingleOrDefault(x => x.Address == ci.Address);

                if (dbci == null)
                {
                    node.ConnectionInfo.Add(new NodeConnectionInformation
                    {
                        Address = ci.Address,
                        Priority = ci.Priority,
                    });
                }
                else
                {
                    dbci.Priority = ci.Priority;
                }
            }

            node.ConnectionInfo = node.ConnectionInfo.Where(dbci =>
                request.ConnectionInfo.Any(x => x.Address == dbci.Address)
            ).ToList();

            if (request.ControlToken != null)
            {
                node.ControlToken = request.ControlToken;   
            }

            node.LastSeen = now;

            await _db.SaveChangesAsync();

            return ApiResultHelper.Success(new UpdateResponse());
        }
    }
}