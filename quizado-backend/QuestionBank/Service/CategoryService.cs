using QuestionBank.Exceptions;
using QuestionBank.Model;
using QuestionBank.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestionBank.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository repository;
        public CategoryService(ICategoryRepository categoryRepository)
        {
            repository = categoryRepository;
        }

        /**
         * RETURNS - Category
         * PARAMS - 1.Category - category to be added
         * Checks whether the category name is provided
         * Checkes whether a category with the same name already exists or not
         * If all validations are valid then call the repo function to execute the logic
         */
        public Category AddCategory(Category category)
        {
            if (String.IsNullOrEmpty(category.Name))
            {
                throw new InvalidRequestBodyException("Category Name Must Be Present");
            }
            if (repository.GetCategoryByName(category.Name.ToLower()) != null)
            {
                throw new CategoryAlreadyExistsException($"Category with name {category.Name} already exists");
            }
            category.Name = category.Name.ToLower();
            return repository.AddCategory(category);
        }

        /**
         * RETURNS - List of Category
         * PARAMS - None
         * Calss repo function which execute query to return all category list present in collection
         */
        public List<Category> GetCategories()
        {
            return repository.GetCategories();
        }

        public List<Category> GetCategoriesForAdmin()
        {
            return repository.GetCategoriesForAdmin();
        }


        /**
         * RETURNS - Category
         * PARAMS - 1.Category ID - category's id which is needed 
         * Checks whether the category with the provided id exists or not
         * If all validations are valid then call the repo function to execute the logic
         */
        public Category GetCategoryById(int categoryId)
        {
            if (repository.GetCategoryById(categoryId) == null)
            {
                throw new CategoryNotFoundException($"Category with Id: {categoryId} not found.");

            }
            return repository.GetCategoryById(categoryId);
        }

        /**
         * RETURNS - Bool - Whether the record is updated or not
         * PARAMS - 1.Category - category to be updated
         * Checks whether the category with the provided id exists or not
         * If all validations are valid then call the repo function to execute the logic
         */
        public bool UpdateCategory(Category category)
        {
            if (repository.GetCategoryById(category._id) == null)
            {
                throw new CategoryNotFoundException($"Category with Id: {category._id} not found.");
            }
            return repository.UpdateCategory(category);
        }
    }
}
