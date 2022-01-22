using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeaderBoard.Aspect;
using LeaderBoard.Common;
using LeaderBoard.Model;
using LeaderBoard.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaderBoard.Controllers
{
    [Authorize]
    [UserExceptionHandlerAttribute]
    [Route("api/[controller]")]
    [ApiController]
    public class LeaderboardController : ControllerBase
    {
        private readonly IUserService userService;
        public LeaderboardController(IUserService userService)
        {
            this.userService = userService;
        }

        /// <summary>
        /// GET request for leaderboard
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{categoryId}")]
        public ActionResult<string> GetLeaderboard(long categoryId)
        {
            return Ok(userService.GetLeaderBoard(categoryId));
        }

        /// <summary>
        /// Post category
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{categoryId}")]
        public ActionResult<string> InsertCategory(long categoryId)
        {
            return Ok(userService.InsertNewCategory(categoryId));
        }
    }
}
