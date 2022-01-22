using QuestionBank.Model;
using QuestionBank.Repository;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Test.InfraSetup;
using Xunit;

namespace Test.Repository
{
    [ExcludeFromCodeCoverage]
    [TestCaseOrderer("Test.PriorityOrderer", "Test")]
    public class CategoryRepositoryTest : IClassFixture<CategoryDbFixture>
    {
        #region Setup
        private ICategoryRepository repository;
        public CategoryRepositoryTest(CategoryDbFixture _fixture)
        {
            repository = new CategoryRepository(_fixture.context);
        }
        #endregion

        #region Postive Test Cases
        [Fact(Skip = "Covered in Controller Test Cases"), TestPriority(1), ]
        public void CreateCategoryMustReturnCategory()
        {
            Category category = new Category
            {
                Name = "Football",
                ImageUrl = null
            };
            var actual = repository.AddCategory(category);
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<Category>(actual);
            Assert.Equal(category.Name, actual.Name);
        }

        [Fact(Skip = "Covered in Controller Test Cases"), TestPriority(2)]
        public void GetAllCategoriesShouldReturnCategoryList()
        {
            var actual = repository.GetCategories();
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<List<Category>>(actual);
        }

        [Fact(Skip = "Covered in Controller Test Cases"), TestPriority(3)]
        public void GetCategoryByIdShouldReturnCategory()
        {
            var actual = repository.GetCategoryById(1);
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<Category>(actual);
        }

        [Fact(Skip = "Covered in Controller Test Cases"), TestPriority(3)]
        public void UpdateCategoryShouldReturnTrue()
        {
            Category category = new Category { _id = 5, Name = "Football", ImageUrl = "Some Link" };
            var actual = repository.UpdateCategory(category);
            Assert.True(actual);
        }

        #endregion
    }
}
