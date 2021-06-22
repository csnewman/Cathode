using System;
using Microsoft.Extensions.DependencyInjection;

namespace Cathode.Gateway.Client
{
    public static class GatewayExtensions
    {
        public static void AddGatewayClient(this IServiceCollection collection, string address)
        {
            collection.AddHttpClient<IGatewayClient, GatewayClient>(c => c.BaseAddress = new Uri(address));
        }
    }
}