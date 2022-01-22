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
    public class QuestionBankDbFixture: IDisposable
    {
        private IConfigurationRoot configuration;
        public QuestionsContext context;
        public CategoryContext categoryContext;
        public QuestionBankDbFixture()
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");

            configuration = builder.Build();
            context = new QuestionsContext(configuration);
            categoryContext = new CategoryContext(configuration);
            categoryContext.Category.DeleteMany(Builders<Category>.Filter.Empty);
            context.Question.DeleteMany(Builders<Questions>.Filter.Empty);
        }
        public void Dispose()
        {
            context = null;
        }
    }
}
