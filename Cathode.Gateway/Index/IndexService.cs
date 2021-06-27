using System;
using System.Linq;
using System.Threading.Tasks;
using Cathode.Common.Api;
using Cathode.Gateway.Protocol.Index;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Cathode.Gateway.Index
{
    public class IndexService : IIndexService
    {
        private readonly GatewayDb _db;

        public IndexService(GatewayDb db)
        {
            _db = db;
        }

        public Task<ApiResult<PingResponse>> PingAsync(ConnectionInfo connection)
        {
            return Task.FromResult(ApiResultHelper.Success(new PingResponse
            {
                RequesterIpAddress = connection.RemoteIpAddress?.ToString(),
            }));
        }

        public async Task<ApiResult<IndexRegisterResponse>> RegisterAsync(IndexRegisterRequest request)
        {
            var now = DateTime.Now;

            var node = await _db.Nodes
                .Include(x => x.ConnectionInfo)
                .SingleOrDefaultAsync(n =>
                    n.AccountId == request.AccountId && n.AccountId == request.AccountId
                );

            if (node == null)
            {
                node = new Node
                {
                    AccountId = request.AccountId,
                    DeviceId = request.DeviceId,
                    FirstSeen = now,
                    LastSeen = now,
                    ConnectionInfo = request.ConnectionInfo.Select(i => new NodeConnectionInformation
                    {
                        Address = i.Address,
                        Priority = i.Priority,
                        LastSeen = now
                    }).ToList()
                };
                await _db.Nodes.AddAsync(node);
            }
            else
            {
                foreach (var ci in request.ConnectionInfo)
                {
                    var dbci = node.ConnectionInfo.SingleOrDefault(x => x.Address == ci.Address);

                    if (dbci == null)
                    {
                        node.ConnectionInfo.Add(new NodeConnectionInformation
                        {
                            Address = ci.Address,
                            Priority = ci.Priority,
                            LastSeen = now,
                        });
                    }
                    else
                    {
                        dbci.Priority = ci.Priority;
                        dbci.LastSeen = now;
                    }
                }
            }

            var changes = await _db.SaveChangesAsync();

            return ApiResultHelper.Success(new IndexRegisterResponse
            {
                Updated = changes != 0
            });
        }
    }
}