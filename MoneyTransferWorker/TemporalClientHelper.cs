using System;
using System.IO;
using System.Collections.Generic;
using Temporalio.Client;

// FIXME FIXME FIXME
// TODO TODO TODO
// This is a COPY of the class defined in the other (MoneyTransferClient) subproject 
// because I could not figure out the magic incantation for referencing the file that 
// was already defined there. I need to solve that, but I am deferring that problem
// until I actually get the class itself working.

public static class TemporalClientHelper
{
    public static async Task<ITemporalClient> CreateClientAsync()
    {
        var address = Environment.GetEnvironmentVariable("TEMPORAL_ADDRESS") ?? "localhost:7233";
        var ns = Environment.GetEnvironmentVariable("TEMPORAL_NAMESPACE") ?? "default";
        var clientCertPath = Environment.GetEnvironmentVariable("TEMPORAL_TLS_CERT");
        var clientKeyPath = Environment.GetEnvironmentVariable("TEMPORAL_TLS_KEY");
        var apiKey = Environment.GetEnvironmentVariable("TEMPORAL_API_KEY");

        var options = new TemporalClientConnectOptions(address)
        {
            Namespace = ns
        };

        if (!string.IsNullOrEmpty(clientCertPath) && !string.IsNullOrEmpty(clientKeyPath))
        {
            // mTLS authentication
            options.Tls = new()
            {
                ClientCert = await File.ReadAllBytesAsync(clientCertPath),
                ClientPrivateKey = await File.ReadAllBytesAsync(clientKeyPath),
            };
        }
        else if (!string.IsNullOrEmpty(apiKey))
        {
            // API Key authentication
            options.ApiKey = apiKey;
            options.RpcMetadata = new Dictionary<string, string>()
            {
                ["temporal-namespace"] = ns
            };
            options.Tls = new();
        }

        return await TemporalClient.ConnectAsync(options);
    }
}
