using System.Threading.Tasks;
using Cathode.Common.Api;
using Cathode.Gateway.Authentication;
using Cathode.Gateway.Protocol.Index;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cathode.Gateway.Index
{
    [ApiV1("index")]
    public class IndexController
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IIndexService _indexService;

        private HttpContext Context => _contextAccessor.HttpContext!;

        public IndexController(IHttpContextAccessor contextAccessor, IIndexService indexService)
        {
            _contextAccessor = contextAccessor;
            _indexService = indexService;
        }

        [HttpGet("ping")]
        public Task<ApiResult<PingResponse>> PingAsync()
        {
            return _indexService.PingAsync(Context.Connection);
        }

        [HttpPost("register")]
        public Task<ApiResult<RegisterResponse>> RegisterAsync(RegisterRequest request)
        {
            return _indexService.RegisterAsync(request);
        }

        [Authorize]
        [HttpPost("update")]
        public Task<ApiResult<UpdateResponse>> UpdateAsync(UpdateRequest request)
        {
            return IAuthEntity.Parse(Context.User) switch
            {
                NodeAuthEntity e => _indexService.UpdateAsync(request, e),
                _ => Task.FromResult(ApiResultHelper.Forbidden<UpdateResponse>()),
            };
        }

        [HttpGet("lookup")]
        public Task<ApiResult<LookupResponse>> LookupAsync(LookupRequest request)
        {
            return _indexService.LookupAsync(request);
        }
    }
}