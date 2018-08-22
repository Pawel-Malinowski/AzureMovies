using System;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace Movies.UnitTesting.Integration
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class IntegrationTestFixture : IDisposable
    {
        private readonly TestServer _testServer;
        public HttpClient Client { get; }

        public IntegrationTestFixture()
        {
            var builder = new WebHostBuilder()
                .UseStartup<Startup>();

            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "IntegrationTesting");

            _testServer = new TestServer(builder);
            Client = _testServer.CreateClient();
        }

        public void Dispose()
        {
            Client.Dispose();
            _testServer.Dispose();
        }
    }
}
