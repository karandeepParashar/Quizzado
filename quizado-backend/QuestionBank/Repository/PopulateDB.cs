using Newtonsoft.Json;
using QuestionBank.Model;
using System.Collections.Generic;
using System.IO;
using System;

namespace QuestionBank.Repository
{
    public class PopulateDB
    {
        private readonly QuestionsContext questionBankContext;
        private readonly CategoryContext categoryContext;
        readonly string questionsJson = "";
        readonly string categoryJson = "";
        readonly string rootdir = "";
        public PopulateDB(QuestionsContext questionBankContext)
        {
            this.questionBankContext = questionBankContext;
            var mydirectory = Environment.CurrentDirectory;
            int indexofmyvar = mydirectory.IndexOf("bin");
            if(indexofmyvar == -1)
            {
                questionsJson = "./Data/Questions.json";
            }
            else
            {
                string substr = mydirectory.Substring(0, indexofmyvar);
                if (substr.Contains("Test"))
                {
                    int testindex = substr.IndexOf("Test");
                    rootdir = substr.Substring(0, testindex);
                    questionsJson = rootdir + "QuestionBank/Data/Questions.json";
                }
            }
            
        }
        public PopulateDB(CategoryContext categoryContext)
        {
            this.categoryContext = categoryContext;
            var mydirectory = Environment.CurrentDirectory;
            int indexofmyvar = mydirectory.IndexOf("bin");
            if (indexofmyvar == -1)
            {
                categoryJson = "./Data/Categories.json";
            }
            else
            {
                string substr = mydirectory.Substring(0, indexofmyvar);
                if (substr.Contains("Test"))
                {
                    int testindex = substr.IndexOf("Test");
                    rootdir = substr.Substring(0, testindex);
                    categoryJson = rootdir + "QuestionBank/Data/Categories.json";
                }
            }
            
        }
        public void PopulateCategories()
        {
            try
            {
                using (StreamReader r = new StreamReader(categoryJson))
                {
                    try
                    {
                        string json = r.ReadToEnd();
                        List<Category> items = JsonConvert.DeserializeObject<List<Category>>(json);
                        categoryContext.Category.InsertMany(items);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        public void PopulateQuestionBank()
        {
            try
            {
                
                using (StreamReader r = new StreamReader(questionsJson))
                {
                    try
                    {
                        string json = r.ReadToEnd();
                        List<Questions> items = JsonConvert.DeserializeObject<List<Questions>>(json);
                        questionBankContext.Question.InsertMany(items);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
    }
}