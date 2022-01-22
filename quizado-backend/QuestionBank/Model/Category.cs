using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestionBank.Model
{
    public class Category
    {
        [BsonId]
        public int _id { get; set; }

        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public List<int> EasyQuestions { get; set; }

        public List<int> MediumQuestions { get; set; }

        public List<int> HardQuestions { get; set; }

        public DateTime CreatedAt { get; set; }

        public Category()
        {
            this.CreatedAt = DateTime.UtcNow;
        }
    }
}
