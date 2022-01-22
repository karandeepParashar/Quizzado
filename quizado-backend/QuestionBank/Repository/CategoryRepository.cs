using QuestionBank.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace QuestionBank.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly CategoryContext context;
        public CategoryRepository(CategoryContext categoryContext)
        {
            context = categoryContext;
            //initialise default categories is not present
            if (context.Category.Find(_ => true).FirstOrDefault() == null)
            {
                new PopulateDB(context).PopulateCategories();
            }

        }

        /**
         * Get Categories returns all documents from the Category collection.
         * Using Project Function we can decide the key,value pairs that we want to project from a documnet.
         * As question id arrays are not neccesary when a simple category list is required
         */
        public List<Category> GetCategories()
        {
            return context.Category.Find(c => c.EasyQuestions.Count >= 20 && c.MediumQuestions.Count >= 20 && c.HardQuestions.Count >= 20).Project(c => new Category { _id = c._id, Name = c.Name, ImageUrl = c.ImageUrl}).ToList();
        }

        public List<Category> GetCategoriesForAdmin()
        {
            return context.Category.Find(_=> true).Project(c => new Category { _id = c._id, Name = c.Name, ImageUrl = c.ImageUrl }).ToList();
        }

        /**
         * Get Category by Id returns a Category object whose Id is passed
         * It returns null if passed Id is not present in ther collection
         */
        public Category GetCategoryById(int categoryId)
        {
            return context.Category.Find(c => c._id == categoryId).Project(c => new Category { _id = c._id, Name = c.Name, ImageUrl = c.ImageUrl }).FirstOrDefault();
        }


        /**
         * Update the category in the collection matching the same id as in the passed category.
         */
        public bool UpdateCategory(Category category)
        {
            var filter = Builders<Category>.Filter.Where(c => c._id == category._id);
            var update = Builders<Category>.Update.Set(c => c.Name, category.Name).Set(c => c.ImageUrl, category.ImageUrl);
            return context.Category.UpdateOne(filter, update).ModifiedCount > 0;
        }

        /**
         * Add Category adds a new category to the collection and returns Category object with an Id.
         * Since Id's are being assigned programatically. First it is being checkwd whether there is any record in collection
         * if no category is present then Id of 1 is assigned if not then last Id is calculated by getting the highest id and incrementing by one
         */
        public Category AddCategory(Category category)
        {
            if (context.Category == null || (context.Category.Find(_ => true).ToList().Count == 0))
            {
                category._id = 1;
            }
            else
            {
                category._id = context.Category.Find(_ => true).Project(c => new Category { _id = c._id}).SortByDescending(c => c._id).ToList()[0]._id + 1;
            }
            category.EasyQuestions = new List<int>();
            category.MediumQuestions = new List<int>();
            category.HardQuestions = new List<int>();
            context.Category.InsertOne(category);
            return category;
        }

        /**
         * Return a category by matching Name.
         * Used to check duplicacy in name
         */
        public Category GetCategoryByName(string categoryName)
        {
            return context.Category.Find(c => c.Name == categoryName).FirstOrDefault();
        }
    }



}
