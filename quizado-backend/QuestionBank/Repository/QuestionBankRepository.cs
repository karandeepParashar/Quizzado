
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using QuestionBank.Model;

namespace QuestionBank.Repository
{
    public class QuestionBankRepository : IQuestionBankRepository
    {
        private readonly QuestionsContext context;
        private readonly CategoryContext categoryContext;//needed to get random question id for Next Question
        public QuestionBankRepository(QuestionsContext questionBankContext, CategoryContext categoryContext)
        {
            context = questionBankContext;
            this.categoryContext = categoryContext;

            //initialise default questions if not present
            if (context.Question.Find(_ => true).FirstOrDefault() == null)
            {
                new PopulateDB(context).PopulateQuestionBank();
            }
        }

        /**
         * RETURN - Bool - Whether question was deleted or not from Question Collection
         * PARAMS - 1.Question 2.CategoryId - Question to be deleted and the category to which it belongs
         * Delete the question from Questions Collection
         * and calls DeleteQuestionFromCategoryList which will..
         * ..delete the question id list from the list in the category which thw question belongs to
         */
        public bool DeleteQuestion(Questions question, int categoryId)
        {
            context.Question.DeleteOne(q => q._id == question._id);
            return DeleteQuestionFromCategoryList(question, categoryId);
        }


        /**
         * RETURN - Bool - Whether question was deleted or not from Category Collection
         * PARAMS - 1.Question 2.CategoryId - Question to be deleted and the category to which it belongs
         * Delete the Id of question from category collection 
         * Depending on difficulty the question id would be present in one of the three list, i.e. 
         * EasyQuestions, MediumQuestions, HardQuestions
         */
        private bool DeleteQuestionFromCategoryList(Questions question, int categoryId)
        {
            var filter = Builders<Category>.Filter.Where(c => c._id == categoryId);
            UpdateDefinition<Category> update = null;
            //long modifiedCount = 0;
            switch (question.Difficulty)
            {
                case 1:
                    update = Builders<Category>.Update.Pull(c => c.EasyQuestions, question._id);
                    break;
                case 2:
                    update = Builders<Category>.Update.Pull(c => c.MediumQuestions, question._id);
                    break;
                case 3:
                    update = Builders<Category>.Update.Pull(c => c.HardQuestions, question._id);
                    break;
            }
            return categoryContext.Category.UpdateOne(filter, update).ModifiedCount > 0;
        }

        /**
         * Return all the questions present in the Questions collection
         */
        public List<Questions> GetQuestionBank()
        {
            return context.Question.Find(_ => true).ToList();
        }

        /**
         * RETURNS - Question - The Next Question for a category quiz
         * PARAMS 1. the prevquestion 2. category in which quiz is being taken
         * Checks whether the prev question was correctly answered or not then increase/decrease difficulty
         * calls FindNextQuestion which randomly selects a question from the specified category and dificulty
         */
        public Questions GetNextQuestion(Questions prevQuestion, int categoryId)
        {
            int nextQuestionDifficulty;
            if (prevQuestion.CorrectOption.Equals(prevQuestion.MarkedOption))
            {
                nextQuestionDifficulty = IncreaseDifficulty(prevQuestion);
            }
            else
            {
                nextQuestionDifficulty = DecreaseDifficulty(prevQuestion);
            }
            return FindNextQuestion(nextQuestionDifficulty, categoryId);
        }

        /**
         * RETURNS - Question - Randomly finds a Question 
         * PARAMS - 1. difficulty 2. categoryId 
         * Finds a random question id from the specified category and difficulty 
         * and return the corresponding question matching that id in questions collection
         */
        private Questions FindNextQuestion(int nextQuestionDifficulty, int categoryId)
        {
            Random random = new Random();
            if (nextQuestionDifficulty == 1)
            {
                int length = categoryContext.Category.Find(c => c._id == categoryId).FirstOrDefault().EasyQuestions.Count;
                int randomQuestionId = categoryContext.Category.Find(c => c._id == categoryId).FirstOrDefault().EasyQuestions[random.Next(0, length - 1)];
                return context.Question.Find(q => q._id == randomQuestionId).FirstOrDefault();
            }
            else if (nextQuestionDifficulty == 2)
            {
                int length = categoryContext.Category.Find(c => c._id == categoryId).FirstOrDefault().MediumQuestions.Count;
                int randomQuestionId = categoryContext.Category.Find(c => c._id == categoryId).FirstOrDefault().MediumQuestions[random.Next(0, length - 1)];
                return context.Question.Find(q => q._id == randomQuestionId).FirstOrDefault();
            }
            else
            {
                int length = categoryContext.Category.Find(c => c._id == categoryId).FirstOrDefault().HardQuestions.Count;
                int randomQuestionId = categoryContext.Category.Find(c => c._id == categoryId).FirstOrDefault().HardQuestions[random.Next(0, length - 1)];
                return context.Question.Find(q => q._id == randomQuestionId).FirstOrDefault();
            }
        }

        // Used to find next difficulty level and make sure if alredy at minimum difficulty then it should not be reduced further
        private int DecreaseDifficulty(Questions prevQuestion)
        {
            int prevDifficulty = prevQuestion.Difficulty;
            int nextDifficluty = prevDifficulty - 1;
            if (nextDifficluty == 0)
            {
                nextDifficluty = 1;
            }
            return nextDifficluty;
        }

        // Used to find next difficulty level and make sure if alredy at maximum difficulty then it should not be increased further
        private int IncreaseDifficulty(Questions prevQuestion)
        {
            int prevDifficulty = prevQuestion.Difficulty;
            int nextDifficluty = prevDifficulty + 1;
            if (nextDifficluty == 4)
            {
                nextDifficluty = 3;
            }
            return nextDifficluty;
        }

        /**
        * RETURNS - Question - Randomly finds a Question of min difficulty 
        * PARAMS - 1. difficulty 2. categoryId 
        * Finds a random question id from the specified category and easy difficulty as it the first question of the quiz
        * and return the corresponding question matching that id in questions collection
        */
        public Questions GetFirstQuestion(int categoryId)
        {
            Random random = new Random();
            int length = categoryContext.Category.Find(c => c._id == categoryId).FirstOrDefault().EasyQuestions.Count;
            int randomQuestionId = categoryContext.Category.Find(c => c._id == categoryId).FirstOrDefault().EasyQuestions[random.Next(0, length - 1)];
            return context.Question.Find(q => q._id == randomQuestionId).FirstOrDefault();
        }

        /**
        * RETURNS - List<Question> - All Questions from a specified category and difficlty. 
        * PARAMS - 1. difficulty 2. categoryId 3. start 4. count
        * Finds all questions ids from the specified category and difficulty in a certain limit (needed for pagination) from category collection 
        * and return the corresponding questions matching those ids in questions collection
        */
        public List<Questions> GetQuestionByCategoryIdAndDifficulty(int categoryId, int difficulty, int start, int count)
        {
            if (difficulty == 1)
            {
                var listOfQuestionIds = categoryContext.Category.Find(c => c._id == categoryId).FirstOrDefault().EasyQuestions.Skip(start).Take(count).ToList();
                var filter = Builders<Questions>.Filter.In("_id", listOfQuestionIds);
                return context.Question.Find(filter).ToList();
            }
            else if (difficulty == 2)
            {
                var listOfQuestionIds = categoryContext.Category.Find(c => c._id == categoryId).FirstOrDefault().MediumQuestions.Skip(start).Take(count).ToList();
                var filter = Builders<Questions>.Filter.In("_id", listOfQuestionIds);
                return context.Question.Find(filter).ToList();
            }
            else
            {
                var listOfQuestionIds = categoryContext.Category.Find(c => c._id == categoryId).FirstOrDefault().HardQuestions.Skip(start).Take(count).ToList();
                var filter = Builders<Questions>.Filter.In("_id", listOfQuestionIds);
                return context.Question.Find(filter).ToList();
            }

        }

        /**
        * RETURNS - Bool - Whether a record was updated or not 
        * PARAMS - 1. question - question to be updated
        */
        public bool UpdateQuestion(Questions question)
        {
            var filter = Builders<Questions>.Filter.Where(q => q._id == question._id);
            return context.Question.ReplaceOne(filter, question).ModifiedCount > 0;
        }

        /**
        * RETURNS - Question - Added Question with the id
        * PARAMS - 1.Question 2.Categoryid - question to be added along with in which categoty it is to be added
        * This function adds the question to question collection and passes it as a arg with the new generated id to AddQuestionToCategoryList
        * which will add it to category collection
        */
        public Questions AddQuestion(Questions question, int categoryId)
        {
            question._id = GetNewQuestionId();
            context.Question.InsertOne(question);
            return AddQuestionToCategoryList(question, categoryId);
        }

        /**
        * RETURNS - Question - Added Question with the id
        * PARAMS - 1.Question 2.Categoryid - question to be added along with in which categoty it is to be added
        * This function adds the question to question collection and passes it as a arg with the new generated id to AddQuestionToCategoryList
        * which will add it to category collection
        */
        public List<Questions> AddMultipleQuestions(List<Questions> questions, int categoryId)
        {
            int firstQuestionId = GetNewQuestionId();
            foreach (var question in questions)
            {
                question._id = firstQuestionId++;
            }
            context.Question.InsertMany(questions);
            return AddMultipleQuestionsToCategoryList(questions, categoryId);
        }
              

        /**
         * RETURNS - Question Id 
         * PARAMS - none
         * Return the id that should be given to a new added questions
         */
        private int GetNewQuestionId()
        {
            int newQuestionId;
            if (context.Question == null || (context.Question.Find(_ => true).ToList().Count == 0))
            {
                newQuestionId = 1;
            }
            else
            {
                newQuestionId = context.Question.Find(_ => true).Project(c => new Questions { _id = c._id }).SortByDescending(c => c._id).ToList()[0]._id + 1;
            }
            return newQuestionId;
        }

        /**
        * RETURNS - Question - Added Question with the id
        * PARAMS - 1.Question 2.Categoryid - question to be added along with in which categoty it is to be added
        * This function adds the question's id to category collection 
        */
        private Questions AddQuestionToCategoryList(Questions question, int categoryId)
        {
            var filter = Builders<Category>.Filter.Where(c => c._id == categoryId);
            UpdateDefinition<Category> update;
            switch (question.Difficulty)
            {
                case 1:
                    update = Builders<Category>.Update.Push(c => c.EasyQuestions, question._id);
                    categoryContext.Category.UpdateOne(filter, update);
                    break;
                case 2:
                    update = Builders<Category>.Update.Push(c => c.MediumQuestions, question._id);
                    categoryContext.Category.UpdateOne(filter, update);
                    break;
                case 3:
                    update = Builders<Category>.Update.Push(c => c.HardQuestions, question._id);
                    categoryContext.Category.UpdateOne(filter, update);
                    break;
            }
            return question;
        }

        /**
        * RETURNS - Question - Added Question with the id
        * PARAMS - 1.Question 2.Categoryid - question to be added along with in which categoty it is to be added
        * This function adds the question's id to category collection 
        */
        private List<Questions> AddMultipleQuestionsToCategoryList(List<Questions> questions, int categoryId)
        {
            foreach (var question in questions)
            {
                var filter = Builders<Category>.Filter.Where(c => c._id == categoryId);
                UpdateDefinition<Category> update;
                switch (question.Difficulty)
                {
                    case 1:
                        update = Builders<Category>.Update.Push(c => c.EasyQuestions, question._id);
                        categoryContext.Category.UpdateOne(filter, update);
                        break;
                    case 2:
                        update = Builders<Category>.Update.Push(c => c.MediumQuestions, question._id);
                        categoryContext.Category.UpdateOne(filter, update);
                        break;
                    case 3:
                        update = Builders<Category>.Update.Push(c => c.HardQuestions, question._id);
                        categoryContext.Category.UpdateOne(filter, update);
                        break;
                }
            }
            return questions;
        }

        /**
        * RETURNS - Question or Null
        * PARAMS - 1.Question 
        * this function checks whether a Question exists in the collection or not
        */
        public Questions GetQuestion(Questions question)//To identify if the question in argument exists in database

        {
            return context.Question.Find(q => q._id == question._id).FirstOrDefault();
        }

        public Questions SkipQuestion(Questions question, int categoryId)
        {
            return FindNextQuestion(question.Difficulty, categoryId);
        }
    }
}
