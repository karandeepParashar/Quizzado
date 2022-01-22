using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using QuestionBank.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Test.InfraSetup
{
    [ExcludeFromCodeCoverage]
    public class CategoryDbFixture : IDisposable
    {
        private IConfigurationRoot configuration;
        public CategoryContext context;
        public CategoryDbFixture()
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");

            configuration = builder.Build();
            context = new CategoryContext(configuration);
            context.Category.DeleteMany(Builders<Category>.Filter.Empty);
        }
        public void Dispose()
        {
            context = null;
        }
    }
}
