using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestionBank.Model
{
    public class QuestionsContext
    {
        MongoClient mongoClient;
        IMongoDatabase database;
        public QuestionsContext(IConfiguration configuration)
        {
            //string server = Environment.GetEnvironmentVariable("Mongo_DB");
            string server = configuration.GetSection("MongoDB:server").Value;
            string db = configuration.GetSection("MongoDB:database").Value;
            mongoClient = new MongoClient(server);
            database = mongoClient.GetDatabase(db);

            //mongoClient = new MongoClient("mongodb+srv://krishna71:krishnaniitquizado@quizado-gdnpe.azure.mongodb.net/test?retryWrites=true&w=majority");
            //database = mongoClient.GetDatabase("quizado");

        }
        public IMongoCollection<Questions> Question => database.GetCollection<Questions>("Questions");
    }
}
