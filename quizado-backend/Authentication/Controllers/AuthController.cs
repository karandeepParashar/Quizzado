using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Authentication.Models;
using Authentication.Services;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Newtonsoft.Json;
using Authentication.Exceptions;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using Authentication.Common;
using System.Security.Principal;

namespace Authentication.Controllers
{
    [AuthExceptionHandler]  //Exception Filder that catches all exceptions and throws status codes accordingly
    [Route("auth")]  //Base route for any endpoint
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IUserService _service;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _repository;


        public AuthController(IUserService service, IConfiguration configuration, IUserRepository repository)
        {
            _service = service;
            _configuration = configuration;
            _repository = repository;
        }


        /// <summary>
        /// Post request to register a new user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>

        [HttpPost("register")]

        public IActionResult Register([FromBody]User user)
        {
            return StatusCode(201, _service.RegisterUser(user));
        }
      
        /// <summary>
        /// Post request to verify user with otp
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>

        [HttpPost("verify")]
        public IActionResult Verify([FromBody]User user)
        {
            _service.VerifyOtp(user.Email, user.Otp);
            var _user = _repository.FindUserById(user.Email);
            return Ok(this.GetJWTToken(_user));
        }

        /// <summary>
        /// Post request to resend otp on mail
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>

        [HttpPost("resend")]
        public IActionResult Resend([FromBody]User user)
        {
            var _user = _service.ResendOtp(user.Email);
            return StatusCode(201, _user);
        }

        /// <summary>
        /// Post request to send new password on mail
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>

        [HttpPost("forgot")]
        public IActionResult ForgotPassword([FromBody]User user)
        {
            return StatusCode(201, _service.Forgotpassword(user.Email));
        }

        /// <summary>
        /// Post request to check referral code
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>

        [HttpPost("referral")]
        public IActionResult CheckReferral([FromBody]User user)
        {
            return StatusCode(201, _service.ReferralService(user));
        }

        /// <summary>
        /// Post request to login an existing user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>

        [HttpPost("login")]

        public IActionResult Login([FromBody]User user)
        {
            string email = user.Email;
            string password = user.Password;
            User _user = _service.Login(email, password);
            return Ok(this.GetJWTToken(_user));
        }


        /// <summary>
        /// Generating Jwt token for user authentication
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>

        private string GetJWTToken(User user)
        {
            var claims = new[]
            {
                new Claim("Email", user.Email),
                new Claim("Role", user.Role),
                new Claim("isVerified", user.IsVerified.ToString()),
                new Claim("Referral", user.Referral),
                //new Claim("Expiry", DateTime.UtcNow.AddDays(1).ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("authserver_secret_to_validate_token"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken();
            if (user.Role == "Admin")
            {
                token = new JwtSecurityToken(
                    issuer: "AuthServer",
                    audience: "jwtclient",
                    claims: claims,
                    expires: DateTime.UtcNow.AddMonths(1),
                    signingCredentials: creds
                );
            }

            else
            {
                token = new JwtSecurityToken(
                    issuer: "AuthServer",
                    audience: "jwtclient",
                    claims: claims,
                    expires: DateTime.UtcNow.AddDays(1),
                    signingCredentials: creds
                );
            }


            var response = new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            };
            return JsonConvert.SerializeObject(response);
        }

        
    }
}
