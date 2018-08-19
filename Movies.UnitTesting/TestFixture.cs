using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace Movies.UnitTesting
{
    public class TestFixture : IDisposable
    {
        private readonly TestServer _testServer;
        public HttpClient Client { get; }

        public TestFixture()
        {
            var builder = new WebHostBuilder()
                //  .UseContentRoot(GetContentRootPath())
                .UseEnvironment("Development")
                .UseStartup<Startup>();  // Uses Start up class from your API Host project to configure the test server

            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");

            _testServer = new TestServer(builder);
            Client = _testServer.CreateClient();
        }

        //private string GetContentRootPath()
        //{
        //    var testProjectPath = PlatformServices.Default.Application.ApplicationBasePath;
        //    var relativePathToHostProject = @"..\..\..\..\..\..\Product.CommandService";
        //    return Path.Combine(testProjectPath, relativePathToHostProject);
        //}

        public void Dispose()
        {
            Client.Dispose();
            _testServer.Dispose();
        }
    }
}
