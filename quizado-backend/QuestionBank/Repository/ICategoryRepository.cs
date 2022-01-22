using QuestionBank.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestionBank.Repository
{
    public interface ICategoryRepository
    {
        //Get Requests Functions
        Category GetCategoryById(int categoryId);
        List<Category> GetCategories();
        List<Category> GetCategoriesForAdmin();

        //Post Request Functions
        Category AddCategory(Category category);

        //Put Request Functions
        bool UpdateCategory(Category category);

        Category GetCategoryByName(string categoryName);

    }
}
