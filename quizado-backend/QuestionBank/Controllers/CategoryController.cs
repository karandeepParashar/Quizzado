using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuestionBank.Common;
using QuestionBank.Model;
using QuestionBank.Service;
using QuestionBank.Aspect;

namespace QuestionBank.Controllers
{
    [Authorize] //Authorization filter to validate JWT Token before serving any request
    [CustomerExceptionHandler] //Exception Filder that catches all exceptions and throws status codes accordingly
    [Route("api/[controller]")] //Base route for any endpoint
    [ApiController] 
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService service;
        public CategoryController(ICategoryService categoryService)
        {
            service = categoryService;
        }

        // Get Request to get all categories
        [HttpGet]
        public IActionResult GetCategories()
        {
            return Ok(service.GetCategories());
        }

        [HttpGet("admin")]
        public IActionResult GetCategoriesForAdmin()
        {
            return Ok(service.GetCategoriesForAdmin());
        }

        // Get Request to fetch category by Id
        [HttpGet("{categoryId}")]
        public IActionResult GetCategories(int categoryId)
        {
            return Ok(service.GetCategoryById(categoryId));
        }

        // Put Request to update category
        [HttpPut]
        public IActionResult UpdateCategory([FromBody]Category category)
        {
            service.UpdateCategory(category);
            return Ok();
        }

        // Post request to add a category
        [HttpPost]
        public IActionResult AddCategory([FromBody]Category category)
        {
            return Ok(service.AddCategory(category));
        }
    }
}