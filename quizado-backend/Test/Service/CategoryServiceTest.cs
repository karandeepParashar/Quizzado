
using Moq;
using QuestionBank.Exceptions;
using QuestionBank.Model;
using QuestionBank.Repository;
using QuestionBank.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Xunit;

namespace Test.Service
{
    [ExcludeFromCodeCoverage]
    [TestCaseOrderer("Test.PriorityOrderer", "Test")]

    public class CategoryServiceTest
    {
        #region Positive Test Cases

        [Fact, TestPriority(1)]
        public void AddCategoryShouldReturnCategory()
        {
            Category category = new Category
            {
                Name = "Football",
                ImageUrl = null
            };
            var mockRepo = new Mock<ICategoryRepository>();

            Category category1 = null;
            mockRepo.Setup(repo => repo.GetCategoryByName(category.Name)).Returns(category1);
            mockRepo.Setup(repo => repo.AddCategory(category)).Returns(category);

            var service = new CategoryService(mockRepo.Object);

            var actual = service.AddCategory(category);
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<Category>(actual);
        }

        [Fact, TestPriority(2)]
        public void GetCategoriesShouldReturnCategoryList()
        {
            List<Category> categories = new List<Category> { new Category { _id = 5, Name = "Football", ImageUrl = null } };
            var mockRepo = new Mock<ICategoryRepository>();
            mockRepo.Setup(repo => repo.GetCategories()).Returns(categories);

            var service = new CategoryService(mockRepo.Object);

            var actual = service.GetCategories();
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<List<Category>>(actual);
        }

        [Fact, TestPriority(3)]
        public void GetCategoryByIdShouldReturnCategory()
        {
            Category category = new Category { _id = 5, Name = "Football", ImageUrl = null };
            var mockRepo = new Mock<ICategoryRepository>();
            mockRepo.Setup(repo => repo.GetCategoryById(5)).Returns(category);

            var service = new CategoryService(mockRepo.Object);

            var actual = service.GetCategoryById(5);
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<Category>(actual);
            Assert.Equal(category, actual);
        }

        [Fact, TestPriority(4)]
        public void UpdateCategoryShouldReturnTrue()
        {
            Category category = new Category { _id = 5, Name = "Football", ImageUrl = null };
            var mockRepo = new Mock<ICategoryRepository>();
            mockRepo.Setup(repo => repo.UpdateCategory(category)).Returns(true);
            mockRepo.Setup(repo => repo.GetCategoryById(5)).Returns(category);

            var service = new CategoryService(mockRepo.Object);

            var actual = service.UpdateCategory(category);
            Assert.True(actual);
        }

        #endregion

        #region Negative Test Cases

        [Fact, TestPriority(5)]
        public void AddCategoryShouldThrowException()
        {
            Category category = new Category
            {
                Name = "history",
                ImageUrl = null
            };
            var mockRepo = new Mock<ICategoryRepository>();

            mockRepo.Setup(repo => repo.GetCategoryByName(category.Name)).Returns(category);
            mockRepo.Setup(repo => repo.AddCategory(category)).Returns(category);

            var service = new CategoryService(mockRepo.Object);

            var actual = Assert.Throws<CategoryAlreadyExistsException>(() => service.AddCategory(category));
        }

        [Fact, TestPriority(6)]
        public void GetCategoryByIdShouldThrowException()
        {
            Category category = new Category { _id = 5, Name = "Football", ImageUrl = null };
            var mockRepo = new Mock<ICategoryRepository>();

            Category category1 = null;
            mockRepo.Setup(repo => repo.GetCategoryById(5)).Returns(category1);

            var service = new CategoryService(mockRepo.Object);

            var actual = Assert.Throws<CategoryNotFoundException>(() => service.GetCategoryById(5));

        }

        [Fact, TestPriority(6)]
        public void UpdateCategoryShouldThrowException()
        {
            Category category = new Category { _id = 5, Name = "Football", ImageUrl = null };
            var mockRepo = new Mock<ICategoryRepository>();

            Category category1 = null;
            mockRepo.Setup(repo => repo.GetCategoryById(5)).Returns(category1);

            var service = new CategoryService(mockRepo.Object);

            var actual = Assert.Throws<CategoryNotFoundException>(() => service.UpdateCategory(category));

        }

        #endregion
    }
}
