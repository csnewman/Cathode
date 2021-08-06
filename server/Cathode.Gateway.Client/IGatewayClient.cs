using System.Threading.Tasks;
using Cathode.Gateway.Protocol.Index;

namespace Cathode.Gateway.Client
{
    public interface IGatewayClient
    {
        Task<PingResponse> PingAsync();
    }
}