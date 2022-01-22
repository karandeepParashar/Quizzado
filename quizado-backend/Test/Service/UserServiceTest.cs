using LeaderBoard.Exceptions;
using LeaderBoard.Model;
using LeaderBoard.Service;
using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Xunit;

namespace Test.Service
{
    [ExcludeFromCodeCoverage]
    [TestCaseOrderer("Test.PriorityOrderer", "Test")]
    public class UserServiceTest
    {
        Mock<IUserRepository> mockRepo;
        private readonly UserService userService;
        public UserServiceTest()
        {
            mockRepo = new Mock<IUserRepository>();
            userService = new UserService(mockRepo.Object);
        }
        #region Postive Test Cases
        [Fact]
        public void TestGetUserDetails() {
            String Name = "Tester";
            String Email = "Tester@gmail.com";
            string PhoneNumber = "1234567890";
            string ProfilePic = "EncryptImage";
            long Coins = 10;
            string categoryId = "_SCORE_1";
            long score = 15;
            Dictionary<string, object> userDetails = new Dictionary<string, object>();
            userDetails.Add("Name", (Object)Name);
            userDetails.Add("Email", (Object)Email);
            userDetails.Add("PhoneNumber", (Object)PhoneNumber);
            userDetails.Add("ProfilePic", (Object)ProfilePic);
            userDetails.Add("Coins", (Object)Coins);
            userDetails.Add("_SCORE_1", (Object)score);

            mockRepo.Setup(repo => repo.GetUserByEmail("Tester@gmail.com")).Returns(true);
            mockRepo.Setup(repo => repo.GetUserDetails("Tester@gmail.com")).Returns(userDetails);

            User user = userService.GetUserDetails("Tester@gmail.com");
            Assert.Equal(Name.ToLower(), user.Name);
            Assert.Equal(Email.ToLower(), user.Email);
            Assert.Equal(PhoneNumber.ToLower(), user.PhoneNumber);
            Assert.Equal(ProfilePic, user.ProfilePic);
            Assert.Equal(score, user.Score[0].Score);
        }
        [Fact]
        public void TestRegisterUserDetails()
        {
            User user = new User();
            user.Email = "Tester@gmail.com";
            user.Name = "Tester";
            user.PhoneNumber = "1234567890";
            user.ProfilePic = "EN";
            mockRepo.Setup(repo => repo.GetUserByEmail(user.Email)).Returns(false);
            mockRepo.Setup(repo => repo.RegisterUser(user)).Returns(true);
            User expectedUser = userService.RegisterUser(user);
            Assert.Equal(user, expectedUser);
        }
        [Fact]
        public void TestUpdateCoins()
        {
            String Email = "Tester@gmail.com";
            int coins = 10;
            mockRepo.Setup(repo => repo.GetUserByEmail("Tester@gmail.com")).Returns(true);
            mockRepo.Setup(repo => repo.UpdateCoins("Tester@gmail.com", coins)).Returns(coins);
            long expectedResult = userService.UpdateCoins(Email, coins);
            Assert.Equal(expectedResult, coins);
        }
        [Fact]
        public void TestUpdateUserDetails()
        {
            User user = new User();
            user.Email = "Tester@gmail.com";
            user.Name = "Tester";
            user.PhoneNumber = "1234567890";
            user.ProfilePic = "EN";
            mockRepo.Setup(repo => repo.GetUserByEmail(user.Email)).Returns(true);
            mockRepo.Setup(repo => repo.UpdateUserDetails(user)).Returns(true);
            User expectedUser = userService.UpdateUserDetails(user);
            Assert.Equal(user, expectedUser);
        }

        [Fact]
        public void TestAddScore()
        {
            String Email = "tester@gmail.com";
            long categoryId = 1;
            mockRepo.Setup(repo => repo.GetUserByEmail(Email)).Returns(true);
            mockRepo.Setup(repo => repo.GetCategory(categoryId)).Returns(true);
            mockRepo.Setup(repo => repo.AddScore(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<long>())).Returns(true);
            Boolean expectedResult = userService.AddScore(Email, categoryId, 90);
            Assert.True(expectedResult);
        }

        [Fact]
        public void TestGetLeaderboard() {
            long categoryId = 1;
            List<LeaderBoard.Model.Leaderboard> leader = new List<LeaderBoard.Model.Leaderboard>();
            LeaderBoard.Model.Leaderboard record = new LeaderBoard.Model.Leaderboard();
            record.Email = "tester@gmail.com";
            record.Rank = 1;
            CategoryScore score = new CategoryScore();
            score.CategoryId = 1;
            score.Score = 100;
            record.CategoryScore = score;
            mockRepo.Setup(repo => repo.GetCategory(categoryId)).Returns(true);
            mockRepo.Setup(repo => repo.GetLeaderBoard(categoryId)).Returns(leader);
            List<LeaderBoard.Model.Leaderboard> actualList = userService.GetLeaderBoard(categoryId);
            Assert.Equal(leader, actualList);
        }
        [Fact]
        public void TestInsertNewCategory() 
        {
            long categoryId = 5;
            mockRepo.Setup(repo => repo.GetCategory(categoryId)).Returns(false);
            mockRepo.Setup(repo => repo.InsertNewCategory(categoryId)).Returns(true);
            Assert.True(userService.InsertNewCategory(categoryId));
        }

        [Fact]
        public void TestGetRank() 
        {
            string Email = "Tester@gmail.com";
            long categoryId = 1;
            mockRepo.Setup(repo => repo.GetUserByEmail("Tester@gmail.com")).Returns(true);
            mockRepo.Setup(repo => repo.GetRank(Email,categoryId)).Returns(1);
            Assert.Equal(1,userService.GetRank(Email,categoryId));          
        }
        #endregion

        #region Negative Test Cases
        [Fact]
        public void TestGetUserDetailsUserNotFound()
        {
            mockRepo.Setup(repo => repo.GetUserByEmail("Tester@gmail.com")).Returns(false);
            Assert.Throws<UserNotFoundException>(()=> userService.GetUserDetails("Tester@gmail.com"));
        }
        [Fact]
        public void TestRegisterUserDetailsUserAlreadyExists()
        {
            User user = new User();
            user.Email = "Tester@gmail.com";
            user.Name = "Tester";
            user.PhoneNumber = "1234567890";
            user.ProfilePic = "EN";
            mockRepo.Setup(repo => repo.GetUserByEmail(user.Email.ToLower())).Returns(true);
            Assert.Throws<UserAlreadyExistsException>(() => userService.RegisterUser(user));
        }

        [Fact]
        public void TestRegisterUserDetailsUserNotCreatedInQuery()
        {
            User user = new User();
            user.Email = "Tester@gmail.com";
            user.Name = "Tester";
            user.PhoneNumber = "1234567890";
            user.ProfilePic = "EN";
            mockRepo.Setup(repo => repo.GetUserByEmail(user.Email.ToLower())).Returns(false);
            mockRepo.Setup(repo => repo.RegisterUser(user)).Returns(false);
            Assert.Null(userService.RegisterUser(user));
        }
        [Fact]
        public void TestUpdateScoreUserNotFound()
        {
            String Email = "Tester@gmail.com";
            int coins = 10;
            mockRepo.Setup(repo => repo.GetUserByEmail("Tester@gmail.com")).Returns(false);
            Assert.Throws<UserNotFoundException>(() => userService.UpdateCoins(Email, coins));
        }
        
        [Fact]
        public void TestUpdateUserDetailsUserNotFound()
        {
            User user = new User();
            user.Email = "Tester@gmail.com";
            user.Name = "Tester";
            user.PhoneNumber = "1234567890";
            user.ProfilePic = "EN";
            mockRepo.Setup(repo => repo.GetUserByEmail(user.Email)).Returns(false);
            Assert.Throws<UserNotFoundException>(() => userService.UpdateUserDetails(user));
        }
        [Fact]
        public void TestAddScoreUserNotFound()
        {
            String Email = "tester@gmail.com";
            long categoryId = 0;
            mockRepo.Setup(repo => repo.GetUserByEmail(Email)).Returns(false);
            mockRepo.Setup(repo => repo.GetCategory(categoryId)).Returns(true);
            mockRepo.Setup(repo => repo.AddScore(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<long>())).Returns(true);
            Assert.Throws<UserNotFoundException>(() => userService.AddScore(Email, categoryId, 90));
        }

        [Fact]
        public void TestAddScoreQueryNotExecuted()
        {
            String Email = "tester@gmail.com";
            long categoryId = 0;
            mockRepo.Setup(repo => repo.GetUserByEmail(Email)).Returns(true);
            mockRepo.Setup(repo => repo.GetCategory(categoryId)).Returns(true);
            mockRepo.Setup(repo => repo.AddScore(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<long>())).Returns(false);
            Assert.False(userService.AddScore(Email, categoryId, 90));
        }

        [Fact]
        public void TestAddScoreCategoryNotFound()
        {
            String Email = "tester@gmail.com";
            long categoryId = 0;
            mockRepo.Setup(repo => repo.GetUserByEmail(Email)).Returns(true);
            mockRepo.Setup(repo => repo.GetCategory(categoryId)).Returns(false);
            mockRepo.Setup(repo => repo.AddScore(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<long>())).Returns(true);
            Assert.Throws<CategoryNotFoundException>(() => userService.AddScore(Email, categoryId, 90));
        }

        [Fact]
        public void TestGetLeaderboardCategoryNotFound()
        {
            long categoryId = 0;
            mockRepo.Setup(repo => repo.GetCategory(categoryId)).Returns(false);
            Assert.Throws<CategoryNotFoundException>(() => userService.GetLeaderBoard(categoryId));
        }
        [Fact]
        public void TestInsertNewCategoryCategoryAlreadyExists()
        {
            long categoryId = 5;
            mockRepo.Setup(repo => repo.GetCategory(categoryId)).Returns(true);
            Assert.Throws<CategoryAlreadyExistsException>(() => userService.InsertNewCategory(categoryId));
        }
        [Fact]
        public void TestGetRankUserNotFound()
        {
            string Email = "Tester@gmail.com";
            long categoryId = 1;
            mockRepo.Setup(repo => repo.GetUserByEmail("Tester@gmail.com")).Returns(false);
            Assert.Throws<UserNotFoundException>(() => userService.GetRank(Email,categoryId));
        }
        #endregion
    }
    }
