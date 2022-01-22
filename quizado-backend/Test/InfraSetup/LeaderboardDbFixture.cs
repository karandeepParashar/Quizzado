using LeaderBoard.Model;
using Microsoft.Extensions.Configuration;
using Moq;
using Neo4j.Driver.V1;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;


namespace Test.InfraSetup
{
    [ExcludeFromCodeCoverage]
    public class LeaderboardDbFixture : IDisposable
    {
        public UserRepository userRepository;
        private IConfigurationRoot configuration;
        public LeaderboardDbFixture()
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            configuration = builder.Build();
            userRepository = new UserRepository(configuration);
        }

        public void Dispose()
        {
            userRepository = null;
        }
    }
}
