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

    public class QuestionBankServiceTest
    {
        
        #region Postive Test Cases
        [Fact, TestPriority(1)]
        public void AddQuestionShouldReturnQuestion()
        {

            var mockRepo = new Mock<IQuestionBankRepository>();
            var mockCategoryRepo = new Mock<ICategoryRepository>();
            Questions question = new Questions
            {
                QuestionString = "Who is the captain of FCBarcelona",
                CorrectOption = "Messi",
                Difficulty = 1,
                MarkedOption = null,
                Option1 = "Pique",
                Option2 = "Roberto",
                Option3 = "Messi",
                Option4 = "Ter Stegen"
            };
            Category category = new Category();
            mockCategoryRepo.Setup(repo => repo.GetCategoryById(4)).Returns(category);
            mockRepo.Setup(repo => repo.AddQuestion(question, 4)).Returns(question);
            mockRepo.Setup(repo => repo.GetQuestion(question)).Returns(question);
            var service = new QuestionBankService(mockRepo.Object, mockCategoryRepo.Object);

            var actual = service.AddQuestion(question, 4);
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<Questions>(actual);
            Assert.Equal(question, actual);

        }

        [Fact, TestPriority(2)]
        public void GetQuestionByCategoryAndDifficultyShouldReturnQuestionsList()
        {
            Questions question = new Questions
            {
                QuestionString = "Who is the captain of FCBarcelona",
                CorrectOption = "Messi",
                Difficulty = 1,
                MarkedOption = null,
                Option1 = "Pique",
                Option2 = "Roberto",
                Option3 = "Messi",
                Option4 = "Ter Stegen"
            };
            var mockRepo = new Mock<IQuestionBankRepository>();
            var mockRepoCategory = new Mock<ICategoryRepository>();
            int categoryId = 4, start = 1, count = 50;
            int difficulty = 1;
            List<Questions> questions = new List<Questions> { question };

            mockRepoCategory.Setup(repo => repo.GetCategoryById(categoryId)).Returns(new Category { _id = categoryId, Name = "Sports", ImageUrl = null });
            mockRepo.Setup(repo => repo.GetQuestionByCategoryIdAndDifficulty(categoryId, difficulty, start, count)).Returns(questions);
            var service = new QuestionBankService(mockRepo.Object, mockRepoCategory.Object);

            var actual = service.GetQuestionByCategoryIdAndDifficulty(categoryId, difficulty, start, count);
            Assert.NotNull(actual);
            Assert.NotEmpty(actual);
            Assert.IsAssignableFrom<List<Questions>>(actual);
        }

        [Fact, TestPriority(3)]
        public void UpdateQuestionShouldReturnTrue()
        {
            var mockRepo = new Mock<IQuestionBankRepository>();
            Questions question = new Questions
            {
                QuestionString = "Who is the captain of FCBarcelona",
                CorrectOption = "Messi",
                Difficulty = 1,
                MarkedOption = null,
                Option1 = "Pique",
                Option2 = "Roberto",
                Option3 = "Messi",
                Option4 = "Ter Stegen"
            };
           

            mockRepo.Setup(repo => repo.UpdateQuestion(question)).Returns(true);
            mockRepo.Setup(repo => repo.GetQuestion(question)).Returns(question);
            var service = new QuestionBankService(mockRepo.Object, null);

            var actual = service.UpdateQuestion(question);
            Assert.True(actual);
        }

        [Fact, TestPriority(4)]
        public void GetFirstQuestionShouldReturnQuestion()
        {
            var mockRepo = new Mock<IQuestionBankRepository>();
            var mockRepoCategory = new Mock<ICategoryRepository>();

            Questions question = new Questions
            {
                QuestionString = "Who is the captain of FCBarcelona",
                CorrectOption = "Messi",
                Difficulty = 1,
                MarkedOption = null,
                Option1 = "Pique",
                Option2 = "Roberto",
                Option3 = "Messi",
                Option4 = "Ter Stegen"
            };
            int categoryId = 1;

            mockRepo.Setup(repo => repo.GetFirstQuestion(categoryId)).Returns(question);
            mockRepoCategory.Setup(repo => repo.GetCategoryById(categoryId)).Returns(new Category { _id = 1, Name = "Animal", ImageUrl = null });
            var service = new QuestionBankService(mockRepo.Object, mockRepoCategory.Object);

            var actual = service.GetFirstQuestion(categoryId);
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<Questions>(actual);
            Assert.Equal(question, actual);
        }

        [Fact, TestPriority(5)]
        public void GetNextQuestionShouldReturnQuestion()
        {
            var mockRepo = new Mock<IQuestionBankRepository>();
            var mockRepoCategory = new Mock<ICategoryRepository>();

            Questions question = new Questions
            {
                _id = 8500,
                QuestionString = "Who is the captain of FCBarcelona",
                CorrectOption = "Messi",
                Difficulty = 3,
                MarkedOption = "Messi",
                Option1 = "Pique",
                Option2 = "Roberto",
                Option3 = "Messi",
                Option4 = "Ter Stegen"
            };
            int categoryId = 4;
            mockRepo.Setup(repo => repo.GetQuestion(question)).Returns(question);
            mockRepo.Setup(repo => repo.GetNextQuestion(question, categoryId)).Returns(question);
            mockRepoCategory.Setup(repo => repo.GetCategoryById(categoryId)).Returns(new Category { _id = 4, Name = "Sports", ImageUrl = null });
            var service = new QuestionBankService(mockRepo.Object, mockRepoCategory.Object);

            var actual = service.GetNextQuestion(question, categoryId);
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<Questions>(actual);
        }

        [Fact, TestPriority(5)]
        public void SkipQuestionShouldReturnQuestion()
        {
            var mockRepo = new Mock<IQuestionBankRepository>();
            var mockRepoCategory = new Mock<ICategoryRepository>();

            Questions question = new Questions
            {
                _id = 8500,
                QuestionString = "Who is the captain of FCBarcelona",
                CorrectOption = "Messi",
                Difficulty = 2,
                MarkedOption = "Messi",
                Option1 = "Pique",
                Option2 = "Roberto",
                Option3 = "Messi",
                Option4 = "Ter Stegen"
            };
            int categoryId = 4;
            mockRepo.Setup(repo => repo.GetQuestion(question)).Returns(question);
            mockRepo.Setup(repo => repo.GetNextQuestion(question, categoryId)).Returns(question);
            mockRepoCategory.Setup(repo => repo.GetCategoryById(categoryId)).Returns(new Category { _id = 4, Name = "Sports", ImageUrl = null });
            var service = new QuestionBankService(mockRepo.Object, mockRepoCategory.Object);

            var actual = service.GetNextQuestion(question, categoryId);
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<Questions>(actual);
        }

        [Fact, TestPriority(6)]
        public void DeleteQuestionShouldReturnTrue()
        {
            Questions question = new Questions
            {
                _id = 1,
                Difficulty = 1,
            };
            
            var mockRepo = new Mock<IQuestionBankRepository>();
            var mockRepoCategory = new Mock<ICategoryRepository>();

            mockRepo.Setup(repo => repo.GetQuestion(question)).Returns(question);
            mockRepo.Setup(repo => repo.DeleteQuestion(question, 1)).Returns(true);
            mockRepoCategory.Setup(repo => repo.GetCategoryById(1)).Returns(new Category { _id = 1, Name = "Animals", ImageUrl = null });

            var service = new QuestionBankService(mockRepo.Object, mockRepoCategory.Object);

            var actual = service.DeleteQuestion(question, 1);
            Assert.True(actual);
        }

        #endregion

        #region Negative Test Case

        [Fact, TestPriority(7)]
        public void AddQuestionShouldThrowException()
        {

            var mockRepo = new Mock<IQuestionBankRepository>();
            var mockCategoryRepo = new Mock<ICategoryRepository>();
            Questions question = new Questions
            {
                QuestionString = "Who is the captain of FCBarcelona",
                CorrectOption = "Messi",
                Difficulty = 1,
                MarkedOption = null,
                Option1 = "Pique",
                Option2 = "Roberto",
                Option3 = "Messi",
                Option4 = "Ter Stegen"
            };
            Category category = null;
            int categoryId = 5;
            mockRepo.Setup(repo => repo.AddQuestion(question, categoryId)).Returns(question);
            mockCategoryRepo.Setup(repo => repo.GetCategoryById(categoryId)).Returns(category);
            var service = new QuestionBankService(mockRepo.Object, mockCategoryRepo.Object);
            
            var actual = Assert.Throws<CategoryNotFoundException>(() => service.AddQuestion(question, categoryId));
        }

        [Fact, TestPriority(8)]
        public void GetQuestionByCategoryAndDifficultyShouldThrowCategoryNotFoundException()
        {
            Questions question = new Questions
            {
                QuestionString = "Who is the captain of FCBarcelona",
                CorrectOption = "Messi",
                Difficulty = 1,
                MarkedOption = null,
                Option1 = "Pique",
                Option2 = "Roberto",
                Option3 = "Messi",
                Option4 = "Ter Stegen"
            };
            var mockRepo = new Mock<IQuestionBankRepository>();
            var mockRepoCategory = new Mock<ICategoryRepository>();
            int categoryId = 4, start = 1, count = 50;
            int difficulty = 1;
            List<Questions> questions = new List<Questions> { question };
            Category category = null;

            mockRepoCategory.Setup(repo => repo.GetCategoryById(categoryId)).Returns(category);
            mockRepo.Setup(repo => repo.GetQuestionByCategoryIdAndDifficulty(categoryId, difficulty, start, count)).Returns(questions);
            var service = new QuestionBankService(mockRepo.Object, mockRepoCategory.Object);

            var actual = Assert.Throws<CategoryNotFoundException>(() => service.GetQuestionByCategoryIdAndDifficulty(categoryId, difficulty, start, count));
        }

        [Fact, TestPriority(9)]
        public void GetQuestionByCategoryAndDifficultyShouldThrowDifficultyNotFoundException()
        {
            Questions question = new Questions
            {
                QuestionString = "Who is the captain of FCBarcelona",
                CorrectOption = "Messi",
                Difficulty = 5,
                MarkedOption = null,
                Option1 = "Pique",
                Option2 = "Roberto",
                Option3 = "Messi",
                Option4 = "Ter Stegen"
            };
            var mockRepo = new Mock<IQuestionBankRepository>();
            var mockRepoCategory = new Mock<ICategoryRepository>();
            int categoryId = 4, start = 1, count = 50;
            int difficulty = 5;
            List<Questions> questions = new List<Questions> { question };

            mockRepoCategory.Setup(repo => repo.GetCategoryById(categoryId)).Returns(new Category { _id = categoryId, Name = "Sports", ImageUrl = null });
            mockRepo.Setup(repo => repo.GetQuestionByCategoryIdAndDifficulty(categoryId, difficulty, start, count)).Returns(questions);
            var service = new QuestionBankService(mockRepo.Object, mockRepoCategory.Object);

            var actual = Assert.Throws<DifficultyNotFoundException>(() => service.GetQuestionByCategoryIdAndDifficulty(categoryId, difficulty, start, count));

        }

        [Fact, TestPriority(10)]
        public void UpdateQuestionShouldThrowException()
        {
            var mockRepo = new Mock<IQuestionBankRepository>();
            Questions question = new Questions
            {
                _id = 9000,
                QuestionString = "Who is the captain of FCBarcelona",
                CorrectOption = "Messi",
                Difficulty = 1,
                MarkedOption = null,
                Option1 = "Pique",
                Option2 = "Roberto",
                Option3 = "Messi",
                Option4 = "Ter Stegen"
            };
           
            Questions question1 = null;
            mockRepo.Setup(repo => repo.UpdateQuestion(question)).Returns(true);
            mockRepo.Setup(repo => repo.GetQuestion(question)).Returns(question1);
            var service = new QuestionBankService(mockRepo.Object, null);

            var actual = Assert.Throws<QuestionNotFoundException>(() => service.UpdateQuestion(question));
            
        }

        [Fact, TestPriority(11)]
        public void GetNextQuestionShouldThrowException()
        {
            var mockRepo = new Mock<IQuestionBankRepository>();
            Questions question = new Questions

            {
                QuestionString = "Who is the captain of FCBarcelona",
                CorrectOption = "Messi",
                Difficulty = 1,
                MarkedOption = null,
                Option1 = "Pique",
                Option2 = "Roberto",
                Option3 = "Messi",
                Option4 = "Ter Stegen"
            };
            int categoryId = 4;
            mockRepo.Setup(repo => repo.GetNextQuestion(question, categoryId)).Returns(question);
            var service = new QuestionBankService(mockRepo.Object, null);

            var actual = Assert.Throws<QuestionNotAttempedException>(() => service.GetNextQuestion(question, categoryId));

        }

        [Fact, TestPriority(11)]
        public void SkipQuestionShouldThrowException()
        {
            var mockRepo = new Mock<IQuestionBankRepository>();
            var mockRepoCategory = new Mock<ICategoryRepository>(); 

            Questions question = new Questions

            {
                _id = -1,
                QuestionString = "Who is the captain of FCBarcelona",
                CorrectOption = "Messi",
                Difficulty = 1,
                MarkedOption = null,
                Option1 = "Pique",
                Option2 = "Roberto",
                Option3 = "Messi",
                Option4 = "Ter Stegen"
            };
            int categoryId = 4;
            Questions question1 = null;
            mockRepo.Setup(repo => repo.GetQuestion(question)).Returns(question1);
            var service = new QuestionBankService(mockRepo.Object, mockRepoCategory.Object);

            var actual = Assert.Throws<QuestionNotFoundException>(() => service.SkipQuestion(question, categoryId));

        }

        [Fact, TestPriority(11)]
        public void SkipQuestionShouldCategoryNotFoundException()
        {
            var mockRepo = new Mock<IQuestionBankRepository>();
            var mockRepoCategory = new Mock<ICategoryRepository>();

            Questions question = new Questions

            {
                _id = 8500,
                QuestionString = "Who is the captain of FCBarcelona",
                CorrectOption = "Messi",
                Difficulty = 3,
                MarkedOption = null,
                Option1 = "Pique",
                Option2 = "Roberto",
                Option3 = "Messi",
                Option4 = "Ter Stegen"
            };
            int categoryId = 4;
            Category category = null;
            mockRepo.Setup(repo => repo.GetQuestion(question)).Returns(question);
            mockRepoCategory.Setup(repo => repo.GetCategoryById(categoryId)).Returns(category);
            var service = new QuestionBankService(mockRepo.Object, mockRepoCategory.Object);

            var actual = Assert.Throws<CategoryNotFoundException>(() => service.SkipQuestion(question, categoryId));

        }


        [Fact, TestPriority(12)]
        public void DeleteQuestionShouldThrowException()
        {
            Questions question = new Questions
            {
                QuestionString = "Who is the captain of FCBarcelona",
                CorrectOption = "Messi",
                Difficulty = 1,
                MarkedOption = null,
                Option1 = "Pique",
                Option2 = "Roberto",
                Option3 = "Messi",
                Option4 = "Ter Stegen"
            };
            var mockRepo = new Mock<IQuestionBankRepository>();
            Questions question1 = null;
            mockRepo.Setup(repo => repo.GetQuestion(question)).Returns(question1);
            mockRepo.Setup(repo => repo.DeleteQuestion(question, 5)).Returns(true);
            var service = new QuestionBankService(mockRepo.Object, null);

            var actual = Assert.Throws<QuestionNotFoundException>(() => service.DeleteQuestion(question, 5));
        }
        #endregion
    }
}
