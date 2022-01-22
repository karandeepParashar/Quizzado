using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestionBank.Model
{
    public class Questions
    {
        public int _id { get; set; }

        public string QuestionString { get; set; }

        public int Difficulty { get; set; }

        public string Option1 { get; set; }

        public string Option2 { get; set; }

        public string Option3 { get; set; }

        public string Option4 { get; set; }

        public string CorrectOption { get; set; }

        public string MarkedOption { get; set; }

    }
}
