using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuestionBank.Aspect;
using QuestionBank.Common;
using QuestionBank.Exceptions;
using QuestionBank.Model;
using QuestionBank.Service;

namespace QuestionBank.Controllers
{
    [Authorize] //Authorization filter to validate JWT Token before serving any request
    [CustomerExceptionHandler] //Exception Filder that catches all exceptions and throws status codes accordingly
    [Route("api/[controller]")] //Base route for any endpoint
    [ApiController]
    public class QuestionBankController : ControllerBase
    {
        private readonly IQuestionBankService service;
        public QuestionBankController(IQuestionBankService questionBankService)
        {
            service = questionBankService;
        }

        //Get  Request to get all questions
        [HttpGet]
        public IActionResult GetQuestionBank()
        {
            return Ok(service.GetQuestionBank());
        }

        //Post Request to add a question
        [HttpPost]
        [Route("{categoryId}")]
        public IActionResult AddQuestion([FromBody]Questions question, int categoryId)
        {
            return Ok(service.AddQuestion(question, categoryId));
        }

        //Put Request to delete a question
        [HttpPut]
        [Route("delete/{categoryId}")]
        public IActionResult DeleteQuestion([FromBody]Questions question, int categoryId)
        {
            service.DeleteQuestion(question, categoryId);
            return Ok();
        }

        //Put Request to update a question
        [HttpPut]
        [Route("update")]
        public IActionResult UpdateQuestion([FromBody]Questions question)
        {
            service.UpdateQuestion(question);
            return Ok();
        }

        //Get Request to get list of questions based on category, difficulty and in a range (start, count)
        [HttpGet]
        [Route("{categoryId}/{difficulty}/{start}/{recordsCount}")]
        public IActionResult GetQuestionsByCategoryAndDifficulty(int categoryId, int difficulty, int start, int recordsCount)
        {
            return Ok(service.GetQuestionByCategoryIdAndDifficulty(categoryId, difficulty, start, recordsCount));
        }

        //Post Request to get next adaptive question of quiz based on whether the response was correct or not
        [HttpPost]
        [Route("NextQuestion/{categoryId}")]
        public IActionResult GetNextQuestion([FromBody]Questions question, int categoryId)
        {
            return Ok(service.GetNextQuestion(question, categoryId));
        }

        //Get Request to get first question of a category with lowest difficulty
        [HttpGet]
        [Route("NextQuestion/{categoryId}")]
        public IActionResult GetFirstQuestion(int categoryId)
        {
            return Ok(service.GetFirstQuestion(categoryId));
        }

        [HttpPost]
        [Route("SkipQuestion/{categoryId}")]
        public IActionResult SkipQuestion([FromBody]Questions question,int categoryId)
        {
            return Ok(service.SkipQuestion(question, categoryId));
        }

        [HttpPost]
        [Route("AddMultipleQuestion/{categoryId}")]
        public IActionResult AddMultipleQuestion([FromBody]List<Questions> questions, int categoryId)
        {
            return Ok(service.AddMultipleQuestions(questions, categoryId));
        }
    }
}