using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Authentication.Exceptions;
using Authentication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Authentication.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly IConfiguration _configuration;

        public UserService(IUserRepository repo, IConfiguration configuration)
        {
            _repo = repo;
            _configuration = configuration;
        }

        /// <summary>
        /// RETURNS - User
        /// PARAMS - User - user to be added
        /// Checks whether user already exists in database
        /// If not, then sends a mail with the generated otp and calls repo function to execute logic.
        /// </summary>
        /// <param name="_user"></param>
        /// <returns></returns>

        public User RegisterUser(User _user)
        {
            var user = _repo.FindUserById(_user.Email);
            if (user == null)
            {
                int otp = PostMessage(_user).Result;
                _user.Otp = otp;
                return _repo.Register(_user);
            }
            else
            {
                throw new UserAlreadyExistsException($"User Id {_user.Email} already exists");
            }
        }

        /// <summary>
        /// RETURNS - boolean
        /// PARAMS - Emailid, Password
        /// Finds the user with email and password from the databse
        /// Checks whether otp entered is correct or not
        /// If correct, then calls repo function to verify the user and returns value true.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="otp"></param>
        /// <returns></returns>

        public bool VerifyOtp(string email, int otp)
        {
            if (_repo.VerifyMail(email, otp))
            {
                return true;
            }
            else
            {
                throw new OtpIncorrectException("Incorrect Otp");
            }
        }

        /// <summary>
        /// RETURNS - User
        /// PARAMS - emailid
        /// Finds the user in the databse according to the email given
        /// Calls the PostMessage method to resend a mail with a new generated otp
        /// Calls the repo function to execute logic.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>

        public User ResendOtp(string email)
        {
            var user = _repo.FindUserById(email);

            if (user != null)
            {
                int otp = PostMessage(user).Result;
                user.Otp = otp;

                return _repo.Resend(user);
            }
            else
            {
                throw new UserNotFoundException("Invalid user id or password");
            }
        }

        /// <summary>
        /// RETURNS - user
        /// PARAMS - email
        /// Checks whether user exists in databse according to the email entered.
        /// If user is found, send a mail with the new password and call repo to update details in database.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>

        public User Forgotpassword(string email)
        {
            var user = _repo.FindUserById(email);
            if (user!=null)
            {
                string password = PostPassword(user).Result;
                user.Password = password;
                return _repo.UpdatePassword(user);
            }
            else
            {
                throw new UserNotFoundException("Invalid user id or password");
            }
        }

        /// <summary>
        /// RETURNS - User
        /// PARAMS - referral
        /// Checks whether user having the referral code exists in the database or not
        /// If user is found, then return user object
        /// </summary>
        /// <param name="referral"></param>
        /// <returns></returns>

        public User ReferralService(User _user)
        {
            var user1 = _repo.FindUserById(_user.Email);
            if (user1 != null)
            {
                var user = _repo.FindUserByReferral(_user);
                if (user != null)
                {
                    return user;
                }
                else
                {
                    throw new ReferralNotFoundException("Invalid referral code");
                }
            }
            else
            {
                throw new UserNotFoundException("Invalid UserId or Password");
            }
            
        }

        /// <summary>
        /// RETURNS - User
        /// PARAMS - emailid, password
        /// Login calls the repo function to check whether user exists in database.
        /// If yes, then it checks whether user has been verified or not.
        /// If bothe validations hold, then it returns the user object.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns></returns>

        public User Login(string userId, string password)
        {
            var user = _repo.Login(userId, password);
            if (user != null)
            {
                if (user.IsVerified == true)
                {
                    return user;
                }
                else
                {
                    int otp = PostMessage(user).Result;
                    user.Otp = otp;
                    _repo.UpdateDb(user);
                    throw new UserNotVerifiedException("Email id has not been verified");
                }
            }
            else
            {
                throw new UserNotFoundException("Invalid user id or password");
            }
        }

        /// <summary>
        /// RETURNS - random generated otp
        /// PARAMS - User - user to whom mail has to be sent
        /// Generates a random 6 digit otp and using sendgrid, sends a mail to the user's email id.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>

        public async Task<int> PostMessage([FromBody]User user)
        {
                var apiKey = _configuration.GetSection("SENDGRID_API_KEY").Value;
                var client = new SendGridClient(apiKey);
                var from = new EmailAddress("quizado.engine@gmail.com", "Quizado Quiz Engine");
                var to = new EmailAddress(user.Email, "Receipient");

                Random random = new Random();
                int num = random.Next(100000, 999999);

                var subject = "Confirmation mail from Quizado ";

                var htmlContent = "Welcome " + user.Email + " to Quizado!<br> Your OTP for Email Confirmation is <strong>" + num + "</strong>";

                var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlContent);
                await client.SendEmailAsync(msg);
                return num;
           
        }

        /// <summary>
        /// RETURNS - random generated password
        /// PARAMS - User - user to whom mail has to be sent
        /// Generates a random 8 character password and using sendgrid, sends a mail to the user's email id.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>

        public async Task<string> PostPassword([FromBody]User user)
        {
            var apiKey = _configuration.GetSection("SENDGRID_API_KEY").Value;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("quizado.engine@gmail.com", "Quizado Quiz Engine");
            var to = new EmailAddress(user.Email, "Receipient");

            string _allowedChars = "0123456789abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ";
            Random randNum = new Random();
            char[] chars = new char[10];
            for (int i = 0; i < 10; i++)
            {
                chars[i] = _allowedChars[(int)((_allowedChars.Length) * randNum.NextDouble())];
            }

            var subject = "Password Reset mail from Quizado ";
           
               var htmlContent = "Your new password is: <strong>" + new string(chars) +"</strong>";
                user.Password = new string(chars);
           
            var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlContent);
            await client.SendEmailAsync(msg);
            return user.Password;
        }
    }
}
