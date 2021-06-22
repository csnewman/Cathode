using System.Threading.Tasks;
using Cathode.Common.Api;
using Cathode.Gateway.Protocol.Index;
using Microsoft.AspNetCore.Http;

namespace Cathode.Gateway.Index
{
    public class IndexService : IIndexService
    {
        public Task<ApiResult<PingResponse>> PingAsync(ConnectionInfo connection)
        {
            return Task.FromResult(ApiResultHelper.Success(new PingResponse
            {
                RequesterIpAddress = connection.RemoteIpAddress?.ToString(),
            }));
        }
    }
}