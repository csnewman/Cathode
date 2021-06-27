using System.Threading.Tasks;
using Cathode.Common.Api;
using Cathode.Gateway.Protocol.Index;
using Microsoft.AspNetCore.Http;

namespace Cathode.Gateway.Index
{
    public interface IIndexService
    {
        Task<ApiResult<PingResponse>> PingAsync(ConnectionInfo connection);

        Task<ApiResult<IndexRegisterResponse>> RegisterAsync(IndexRegisterRequest request);
    }
}