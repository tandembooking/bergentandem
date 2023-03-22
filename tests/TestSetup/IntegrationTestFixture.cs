using System;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace TandemBooking.Tests.TestSetup
{
    [CollectionDefinition("Integration Tests")]
    public class IntegrationTestFixture : IDisposable
    {
        public IntegrationTestFixture()
        {
            // Arrange
            var builder = new WebHostBuilder()
                .UseEnvironment("Development")
                .UseStartup<TestStartup>();

            Server = new TestServer(builder);
            Client = Server.CreateClient();
        }

        public TestServer Server { get; }
        public HttpClient Client { get; }

        public void Dispose()
        {
            Server.Dispose();
            Client.Dispose();
        }
    }
}