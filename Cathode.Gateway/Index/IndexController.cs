using System.Threading.Tasks;
using Cathode.Common.Api;
using Cathode.Gateway.Protocol.Index;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cathode.Gateway.Index
{
    [ApiV1("index")]
    public class IndexController
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IIndexService _indexService;

        private HttpContext Context => _contextAccessor.HttpContext;

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

    }
}