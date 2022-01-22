using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeaderBoard.Exceptions;
using LeaderBoard.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LeaderBoard.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        public UserService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }
        /// <summary>
        /// Add score and organize the database in order to rank
        /// </summary>
        /// <param name="Email">Email Address</param>
        /// <param name="categoryName">Category Name</param>
        /// <param name="categoryScore">Category Score</param>
        /// <param name="QA">Question Attempted</param>
        /// <returns></returns>
        public bool AddScore(string Email, long categoryId, long categoryScore)
        {
            if (userRepository.GetUserByEmail(Email) && userRepository.GetCategory(categoryId))
            {
                return userRepository.AddScore(Email, categoryId, categoryScore); ;
            }
            else
            {
                if (!userRepository.GetUserByEmail(Email))
                    throw new UserNotFoundException($"User with Email: \"{Email}\" does not exists.");
                else
                    throw new CategoryNotFoundException($"Category : \"{categoryId}\" Not Found");
            }
        }
        /// <summary>
        /// Method for fetching Leaderboard w.r.t category
        /// </summary>
        /// <param name="category">Category Name</param>
        /// <returns></returns>
        public List<Leaderboard> GetLeaderBoard(long categoryId)
        {
            if (userRepository.GetCategory(categoryId))
            {
                return userRepository.GetLeaderBoard(categoryId);
            }
            else
            {
                throw new CategoryNotFoundException($"Category : \"{categoryId}\" Not Found");
            }
        }
        /// <summary>
        /// Gets the Rank of the user based upon his category
        /// </summary>
        /// <param name="Email">Email Address</param>
        /// <param name="category">Cateogry Name</param>
        /// <returns>His rank if </returns>
        public long? GetRank(string Email, long categoryId)
        {
            if (!userRepository.GetUserByEmail(Email)) {
                throw new UserNotFoundException($"User with Email: \"{Email}\" does not exists.");
            }
            return userRepository.GetRank(Email, categoryId);
        }
        /// <summary>
        /// Method for fetching the user details and populating the user model from Neo4j
        /// </summary>
        /// <param name="email">Email Address</param>
        /// <returns>User Model Object populated with data from neo4j</returns>
        public User GetUserDetails(string email)
        {
            if (userRepository.GetUserByEmail(email))
            {
                Dictionary<string, object> userProperties = userRepository.GetUserDetails(email);
                User user = new User();
                foreach (string properties in userProperties.Keys)
                {
                    if (properties.StartsWith("_SCORE_"))
                    {
                        long categoryId = Int64.Parse(properties.Split("_")[2]);
                        CategoryScore quizScore = new CategoryScore();
                        quizScore.CategoryId = categoryId;
                        quizScore.Score = Int64.Parse(userProperties[properties].ToString());
                        user.Score.Add(quizScore);
                    }
                }
                user.Email = userProperties.GetValueOrDefault("Email").ToString().ToLower();
                user.Name = userProperties.GetValueOrDefault("Name").ToString().ToLower();
                user.PhoneNumber = userProperties.GetValueOrDefault("PhoneNumber").ToString().ToLower();
                user.ProfilePic = userProperties.GetValueOrDefault("ProfilePic").ToString();
                user.Coins = (long)userProperties.GetValueOrDefault("Coins");
                return user;
            }
            else {
                throw new UserNotFoundException($"User with Email: \"{email}\" does not exists.");
            }
        }
        /// <summary>
        /// Method that creates categoryScore, questionAttempted in START node and END node to initiate relationship based on new Category
        /// </summary>
        /// <param name="categoryName">Category Name</param>
        /// <returns></returns>
        public bool InsertNewCategory(long categoryId)
        {
            if (!userRepository.GetCategory(categoryId))
            {
                userRepository.InsertNewCategory(categoryId);
                return true;
            }
            else {
                throw new CategoryAlreadyExistsException($"Category \"{categoryId}\" already exists.");
            }
        }
        /// <summary>
        /// Method for creating a new user without category score or its details
        /// </summary>
        /// <param name="user">User Model Object to register him</param>
        /// <returns></returns>       
        public User RegisterUser(User user)
        {
            if (!userRepository.GetUserByEmail(user.Email.ToLower()))
            {
                user.Email = user.Email.ToLower();
                if (userRepository.RegisterUser(user))
                {
                    user.Email = user.Email.ToLower();
                    return user;
                }
                else
                    return null;
            }
            else {
                throw new UserAlreadyExistsException($"User with \"{user.Email}\" already exists.");
            }
        }
        /// <summary>
        /// Method for updating user details with email
        /// </summary>
        /// <param name="user">User Model Object to register him.</param>
        /// <returns></returns>
        public User UpdateUserDetails(User user)
        {
            if (userRepository.GetUserByEmail(user.Email))
            {
                userRepository.UpdateUserDetails(user);
                return user;
            }
            else
            {
                throw new UserNotFoundException($"User with Email: \"{user.Email}\" does not exists.");
            }
        }

        public long UpdateCoins(string Email, long coin)
        {
            if (userRepository.GetUserByEmail(Email))
            {
                return userRepository.UpdateCoins(Email, coin);
            }
            else
            {
                throw new UserNotFoundException($"User with Email: \"{Email}\" does not exists.");
            }
                
        }
    }
}
