using Cathode.Common.Protocol;

namespace Cathode.Gateway.Protocol.Index
{
    public class PingResponse : ApiResponse
    {
        public string? RequesterIpAddress { get; set; }
    }
}