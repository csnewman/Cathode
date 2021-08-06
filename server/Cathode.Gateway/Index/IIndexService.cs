using System.Threading.Tasks;
using Cathode.Common.Api;
using Cathode.Gateway.Authentication;
using Cathode.Gateway.Protocol.Index;
using Microsoft.AspNetCore.Http;

namespace Cathode.Gateway.Index
{
    public interface IIndexService
    {
        Task<ApiResult<PingResponse>> PingAsync(ConnectionInfo connection);

        Task<ApiResult<RegisterResponse>> RegisterAsync(RegisterRequest request);

        Task<ApiResult<UpdateResponse>> UpdateAsync(UpdateRequest request, NodeAuthEntity entity);

        Task<ApiResult<LookupResponse>> LookupAsync(LookupRequest request);
    }
}