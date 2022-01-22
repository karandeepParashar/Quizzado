using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeaderBoard.Common;
using LeaderBoard.Model;
using LeaderBoard.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LeaderBoard.Aspect;

namespace LeaderBoard.Controllers
{
    [Authorize]
    [TypeFilter(typeof(LoggerAspect))]
    [UserExceptionHandlerAttribute]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        /// <summary>
        /// POST request to add score
        /// </summary>
        /// <param name="Email"></param>
        /// <param name="categoryName"></param>
        /// <param name="categoryScore"></param>
        /// <param name="QA"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{Email}/{categoryId}/{categoryScore}")]
        public ActionResult<IEnumerable<string>> AddScore(string Email, long categoryId, long categoryScore)
        {
            return Ok(userService.AddScore(Email, categoryId, categoryScore));
        }

        /// <summary>
        /// GET Rank for User and category
        /// </summary>
        /// <param name="Email"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Email}/{categoryId}")]
        public ActionResult<string> GetRank(string Email, long categoryId)
        {
            return Ok(userService.GetRank(Email, categoryId));
        }

        /// <summary>
        /// Get User Details
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Email}")]
        public ActionResult<string> GetUserDetails(string Email)
        {
            return Ok(userService.GetUserDetails(Email));
        }

        /// <summary>
        /// Register User
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public ActionResult<string> RegisterUser([FromBody]User user)
        {
            return Ok(userService.RegisterUser(user));
        }

        /// <summary>
        /// Update User Details
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("")]
        public ActionResult<string> UpdateUserDetails([FromBody]User user)
        {
            return Ok(userService.UpdateUserDetails(user));
        }

        /// <summary>
        /// Register User
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("coins/{Email}/{Coins}")]
        public ActionResult<string> UpdateCoin(string Email, long Coins)
        {
            return Ok(userService.UpdateCoins(Email, Coins));
        }
    }
}