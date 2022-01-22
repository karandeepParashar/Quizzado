using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuestionBank.Exceptions;
using QuestionBank.Model;
using QuestionBank.Repository;

namespace QuestionBank.Service
{
    public class QuestionBankService : IQuestionBankService
    {
        private readonly IQuestionBankRepository repository;
        private readonly ICategoryRepository categoryRepository;
        public QuestionBankService(IQuestionBankRepository questionBankRepository, ICategoryRepository categoryRepository)
        {
            repository = questionBankRepository;
            this.categoryRepository = categoryRepository;
        }

        /**
         * RETURNS - List of Questions
         * PARAMS - 1.CategoryId 2.Difficulty 3.Start 4.Count 
         * Checks whether the category id exists
         * Checkes whether the difficulty level exists
         * If all validations are valid then call the repo function to execute the logic of get question from 
         * a category and difficulty within a range
         */
        public List<Questions> GetQuestionByCategoryIdAndDifficulty(int categoryId, int difficulty, int start, int count)
        {
            if(categoryRepository.GetCategoryById(categoryId) == null)
            {
                throw new CategoryNotFoundException($"Category with id: {categoryId} not found");
            }
            if( difficulty > 3 || difficulty < 1 )
            {
                throw new DifficultyNotFoundException($"{difficulty} is an invalid Difficulty Level");
            }
            return repository.GetQuestionByCategoryIdAndDifficulty(categoryId, difficulty, start, count);
        }

        /**
         * RETURNS - List of Questions
         * PARAMS - None
         * Calls the repo function that return all question
         */
        public List<Questions> GetQuestionBank()
        {
            return repository.GetQuestionBank();
        }

        /**
         * RETURNS - Question
         * PARAMS - 1.Question 2.CategoryId
         * Checks whether the category id exists
         * If all validations are valid then call the repo function to execute the logic to add question
         */
        public Questions AddQuestion(Questions question, int categoryId)
        {
            if(categoryRepository.GetCategoryById(categoryId) == null)
            {
                throw new CategoryNotFoundException($"Category with id {categoryId} does not exist");
            }
            return repository.AddQuestion(question, categoryId);
        }

        public List<Questions> AddMultipleQuestions(List<Questions> questions, int categoryId)
        {
            if(categoryRepository.GetCategoryById(categoryId) == null)
            {
                throw new CategoryNotFoundException($"Category with id {categoryId} does not exist");
            }
            if(questions.Count == 0)
            {
                throw new InvalidRequestBodyException("Questions array shoul not be empty"); 
            }
            return repository.AddMultipleQuestions(questions, categoryId);
        }


        /**
         * RETURNS - Question
         * PARAMS - 1.Question 2.CategoryId
         * Checks whether the category id exists
         * Checks if the marked option is NOT empty or null
         * Checks if the question exists
         * If all validations are valid then call the repo function to execute the logic to get next question of quiz
         */
        public Questions GetNextQuestion(Questions question, int categoryId)
        {
            if (String.IsNullOrEmpty(question.MarkedOption))
            {
                throw new QuestionNotAttempedException("Marked option is not present");
            }
            if (repository.GetQuestion(question) == null)
            {
                throw new QuestionNotFoundException($"Question with id: {question._id} does not exists");
            }
            if (categoryRepository.GetCategoryById(categoryId) == null)
            {
                throw new CategoryNotFoundException($"Category with id {categoryId} does not exist");
            }
            return repository.GetNextQuestion(question, categoryId);
        }

        /**
         * RETURNS - Question
         * PARAMS - 1.Question 2.CategoryId
         * Checks whether the category id exists
         * If all validations are valid then call the repo function to execute the logic to get first question of quiz
         */
        public Questions GetFirstQuestion(int categoryId)
        {
            if (categoryRepository.GetCategoryById(categoryId) == null)
            {
                throw new CategoryNotFoundException($"Category with id: {categoryId} not found");
            }
            return repository.GetFirstQuestion(categoryId);
        }

        /**
         * RETURNS - Question
         * PARAMS - 1.Question 2.CategoryId
         * Checks whether the question to be updated exists or not
         * If all validations are valid then call the repo function to execute the logic to update question
         */
        public bool UpdateQuestion(Questions question)
        {
            if (repository.GetQuestion(question) == null)
            {
                throw new QuestionNotFoundException($"Question with id: {question._id} does not exists");
            }
            return repository.UpdateQuestion(question);
        }

        /**
         * RETURNS - Question
         * PARAMS - 1.Question 2.CategoryId
         * Checks whether the question to be delted exists or not
         * Checks whther the catogry exists
         * If all validations are valid then call the repo function to execute the logic to delete question
         */
        public bool DeleteQuestion(Questions question, int categoryId)
        {
            if (repository.GetQuestion(question) == null)
            {
                throw new QuestionNotFoundException($"Question with id: {question._id} does not exists");
            }
            if( categoryRepository.GetCategoryById(categoryId) == null)
            {
                throw new CategoryNotFoundException($"Category with id {categoryId} does not exist");
            }
            return repository.DeleteQuestion(question, categoryId);
        }

        public Questions SkipQuestion(Questions question, int categoryId)
        {
            if (repository.GetQuestion(question) == null)
            {
                throw new QuestionNotFoundException($"Question with id: {question._id} does not exists");
            }
            if (categoryRepository.GetCategoryById(categoryId) == null)
            {
                throw new CategoryNotFoundException($"Category with id {categoryId} does not exist");
            }
            return repository.SkipQuestion(question, categoryId);
        }
    }
}
