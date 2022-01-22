using QuestionBank.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestionBank.Service
{
    public interface ICategoryService
    {
        List<Category> GetCategories();
        List<Category> GetCategoriesForAdmin();
        Category GetCategoryById(int categoryId);
        Category AddCategory(Category category);
        bool UpdateCategory(Category category);

    }
}
