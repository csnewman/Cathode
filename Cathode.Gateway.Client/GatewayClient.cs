using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Cathode.Gateway.Protocol.Index;
using Newtonsoft.Json;

namespace Cathode.Gateway.Client
{
    public class GatewayClient : IGatewayClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializer _serializer;

        public GatewayClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _serializer = new JsonSerializer();
        }

        public async Task<PingResponse> PingAsync()
        {
            await using var s = await _httpClient.GetStreamAsync("/api/v1/index/ping");
            using var sr = new StreamReader(s);
            using JsonReader reader = new JsonTextReader(sr);
            return _serializer.Deserialize<PingResponse>(reader);
        }
    }
}