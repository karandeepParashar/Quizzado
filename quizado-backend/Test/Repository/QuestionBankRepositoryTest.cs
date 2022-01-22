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
    public class QuestionBankRepositoryTest : IClassFixture<QuestionBankDbFixture>
    {
        #region Setup
        private IQuestionBankRepository repository;
        public QuestionBankRepositoryTest(QuestionBankDbFixture _fixture)
        {
            repository = new QuestionBankRepository(_fixture.context, _fixture.categoryContext);
        }
        #endregion

        #region Postive Test Cases
        [Fact(Skip = "Covered in Controller Test Cases"), TestPriority(1)]
        public void AddQuestionShouldReturnQuestion()
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

            var actual = repository.AddQuestion(question, 4);
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<Questions>(actual);
            Assert.Equal(question, actual);

        }
        
        [Fact(Skip = "Covered in Controller Test Cases"), TestPriority(2)]
        public void GetQuestionByCategoryAndDifficultyMustReturnQuestionsList()
        {
            var actual = repository.GetQuestionByCategoryIdAndDifficulty(1, 1, 1, 50);
            Assert.NotNull(actual);
            Assert.NotEmpty(actual);
            Assert.IsAssignableFrom<List<Questions>>(actual);
            Assert.Equal(50, actual.Count);
        }

        [Fact(Skip = "Covered in Controller Test Cases"), TestPriority(3)]
        public void UpdateQuestionShouldReturnTrue()
        {
            Questions question = new Questions
            {
                _id = 8400,
                QuestionString = "Who is the captain of FCBarcelona",
                CorrectOption = "Messi",
                Difficulty = 3,
                MarkedOption = null,
                Option1 = "Pique",
                Option2 = "Puyol",
                Option3 = "Messi",
                Option4 = "Ter Stegen"
            };

            var actual = repository.UpdateQuestion(question);
            Assert.True(actual);
        }

        [Fact(Skip = "Covered in Controller Test Cases"), TestPriority(4)]
        public void GetFirstQuestionShouldReturnQuestion()
        {
            int categoryId = 3;
            var actual = repository.GetFirstQuestion(categoryId);
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<Questions>(actual);
            Assert.Equal(1, actual.Difficulty); // Check that difficulty of First Question is 1
        }

        [Fact(Skip = "Covered in Controller Test Cases"), TestPriority(5)]
        public void GetNextQuestionShouldReturnQuestionWithMoreDifficuly()
        {
            Questions question = new Questions
            {
                QuestionString = "Who is the captain of FCBarcelona",
                CorrectOption = "Messi",
                Difficulty = 2,
                MarkedOption = "Messi",
                Option1 = "Pique",
                Option2 = "Roberto",
                Option3 = "Messi",
                Option4 = "Ter Stegen"
            };

            var actual = repository.GetNextQuestion(question, 4);
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<Questions>(actual);
            Assert.Equal(question.Difficulty + 1, actual.Difficulty);
        }

        [Fact(Skip = "Covered in Controller Test Cases"), TestPriority(6)]
        public void GetNextQuestionShouldReturnQuestionWithLessDifficuly()
        {
            Questions question = new Questions
            {
                QuestionString = "Who is the captain of FCBarcelona",
                CorrectOption = "Messi",
                Difficulty = 2,
                MarkedOption = "Piques",
                Option1 = "Pique",
                Option2 = "Roberto",
                Option3 = "Messi",
                Option4 = "Ter Stegen"
            };

            var actual = repository.GetNextQuestion(question, 4);
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<Questions>(actual);
            Assert.Equal(question.Difficulty - 1, actual.Difficulty);
        }

        [Fact(Skip = "Covered in Controller Test Cases"), TestPriority(6)]
        public void SkipQuestionShouldReturnQuestionWithSameDifficuly()
        {
            Questions question = new Questions
            {
                _id = 8500,
                QuestionString = "Who is the captain of FCBarcelona",
                CorrectOption = "Messi",
                Difficulty = 3,
                MarkedOption = "Piques",
                Option1 = "Pique",
                Option2 = "Roberto",
                Option3 = "Messi",
                Option4 = "Ter Stegen"
            };

            var actual = repository.SkipQuestion(question, 4);
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<Questions>(actual);
            Assert.Equal(question.Difficulty, actual.Difficulty);
        }

        [Fact(Skip = "Covered in Controller Test Cases"), TestPriority(7)]
        public void DeleteQuestionShouldReturnTrue()
        {
            Questions question = new Questions
            {
                _id = 8400,
                Difficulty = 3,
            };

            var actual = repository.DeleteQuestion(question, 4);
            Assert.True(actual);
        }

        #endregion

        #region Negative Test Cases
        [Fact(Skip = "Covered in Controller Test Cases"), TestPriority(8)]
        public void GetQuestionMustReturnNull()
        {
            Questions question = new Questions
            {
                _id = 12323132,
                QuestionString = "Who is the captain of FCBarcelona",
                CorrectOption = "Messi",
                Difficulty = 1,
                MarkedOption = null,
                Option1 = "Pique",
                Option2 = "Roberto",
                Option3 = "Messi",
                Option4 = "Ter Stegen"
            };

            var actual = repository.GetQuestion(question);
            Assert.Null(actual);
        }

        [Fact(Skip = "Covered in Controller Test Cases"), TestPriority(9)]
        public void GetQuestionByCategoryAndDifficultyShouldReturnEmptyList()
        {
            var actual = repository.GetQuestionByCategoryIdAndDifficulty(1, 1, 1000, 1);
            Assert.NotNull(actual);
            Assert.Equal(new List<Questions> { }, actual);
        }
        #endregion
    }
}
