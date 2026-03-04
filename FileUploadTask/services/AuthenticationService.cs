using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

using System.IO;
namespace FileUploadTask
{
    class AuthenticationService
    {
        public static GraphServiceClient GetGraphServiceClient()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            string clientId = config["ClientId"]!;
            string tenantId = config["TenantId"]!;
            string[] scopes = new[] { "User.Read", "Files.ReadWrite" };

            var options = new DeviceCodeCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
                ClientId = clientId,
                TenantId = tenantId,
                DeviceCodeCallback = (code, cancellation) =>
                {
                    Console.WriteLine(code.Message);
                    return Task.FromResult(0);
                },
            };

            var deviceCodeCredential = new DeviceCodeCredential(options);
            return new GraphServiceClient(deviceCodeCredential, scopes);
        }

    }
}