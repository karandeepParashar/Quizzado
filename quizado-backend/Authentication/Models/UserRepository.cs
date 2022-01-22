using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.Models
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _context;
        public UserRepository(UserDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// FindUserById returns a user whose email id is passed
        /// It returns null if the user is not present
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public User FindUserById(string Email)
        {
            var _user = _context.Users.FirstOrDefault(u => u.Email == Email);
            return _user;
        }

        /// <summary>
        /// FindUserByReferral returns user whose referral string is passed.
        /// It returns null if the user is not found.
        /// </summary>
        /// <param name="referral"></param>
        /// <returns></returns>
        /// 
        public User FindUserByReferral(User user)
        {
            var _user = _context.Users.FirstOrDefault(u => u.Referral == user.Referral && u.Email!=user.Email);
            return _user;
        }

        /// <summary>
        /// Encrypts the user entered password using SHA256
        /// Register adds the new user to database and returns the user object.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>

        public User Register(User user)
        {
            user.Password = ToSHA256(user.Password);
            user.IsVerified = false;
            user.Role = "User";
            user.Referral = "";
            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }

        /// <summary>
        /// VerifyMail checks whether the user has entered the correct otp or not.
        /// If the otp is correct, it generates a referral code for that user, changes value of isVerified to true and updates the database record of that user. It returns the value true.
        /// If the otp entered is incorrect, it returns the value false.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="otp"></param>
        /// <returns></returns>

        public bool VerifyMail(string email, int otp)
        {
            var _user = _context.Users.FirstOrDefault(u => u.Email == email && u.Otp == otp);

            if (_user != null)
            {
                string referral = GenerateReferral();
                _user.IsVerified = true;
                _user.Referral = referral;
                _context.Users.Update(_user);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Resend sends the random generated otp again to the user's email id and updates the databse record of the user.
        /// It returns the value of the updated user.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>

        public User Resend(User user)
        {
            var _user = FindUserById(user.Email);
            _user.Otp = user.Otp;
            _context.Users.Update(_user);
           _context.SaveChanges();
            return _user;           
        }

        /// <summary>
        /// UpdatePassword searches the database for the user based on the user's email and updates his password after encrypting it with SHA 256
        /// It returns the updated user.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
      
        public User UpdatePassword(User user)
        {
            var _user = _context.Users.FirstOrDefault(u => u.Email == user.Email);
            _user.Password = user.Password;
            _user.Password = ToSHA256(_user.Password);
            _context.Users.Update(_user);
            _context.SaveChanges();
            return _user;
        }

        /// <summary>
        /// UpdateDb searches the database for the user based on the user's email and updates his otp
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>


        public void UpdateDb(User user)
        {
            var _user = _context.Users.FirstOrDefault(u => u.Email == user.Email);
            _user.Otp = user.Otp;
            _context.Users.Update(_user);
            _context.SaveChanges();
        }


        /// <summary>
        /// Login searches the databse for the email id and password.
        /// If user is found, it returns the user.
        /// otherwise it returns null.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns></returns>

        public User Login(string userId, string password)
        {
            var _user = _context.Users.FirstOrDefault(u => u.Email == userId && u.Password == ToSHA256(password));
            return _user;
        }

        /// <summary>
        /// ToSHA256 encrypts the user password using SHA 256 method. It returns the encrypted string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>

        public string ToSHA256(string value)
        {
            SHA256 sha256 = SHA256.Create();
            byte[] hashData = sha256.ComputeHash(Encoding.Default.GetBytes(value));

            StringBuilder returnValue = new StringBuilder();
            for (int i = 0; i < hashData.Length; i++)
            {
                returnValue.Append(hashData[i].ToString());
            }
            return returnValue.ToString();
        }

        /// <summary>
        /// GenerateReferral returns a random 8 character string that acts as a referral code for the particular user.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>

        public string GenerateReferral()
        {

            Random random = new Random();
            string characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            StringBuilder result = new StringBuilder(4);
            for (int i = 0; i < 4; i++)
            {
                result.Append(characters[random.Next(characters.Length)]);
            }
             var x = result.ToString();
            Random random_num = new Random();
            string num = random_num.Next(0000, 9999).ToString();
            string referral = x + num;
            return referral;
        }

    }
}
