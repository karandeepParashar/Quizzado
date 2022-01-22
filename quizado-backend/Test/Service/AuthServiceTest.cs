using System;
using System.Collections.Generic;
using System.Text;
using Authentication.Models;
using Authentication.Services;
using Moq;
using Xunit;
using Microsoft.Extensions.Configuration;
using Authentication.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace Test.Service
{
    [ExcludeFromCodeCoverage]
    [TestCaseOrderer("Test.PriorityOrderer", "Test")]
    public class AuthServiceTest
    {
        #region Positive tests
        [Fact, TestPriority(1)]
        public void RegisterUserShouldReturnUser()
        {
            var mockRepo = new Mock<IUserRepository>(); 
            var config = new ConfigurationBuilder()
           .AddJsonFile("appsettings.json")
           .Build();
            var clientId = config["SENDGRID_API_KEY"];
            
            User user = new User
            {
                Email = "Tester2@gmail.com",
                Password = "asdfg",
                Role = "Admin",
                IsVerified = false,
                Otp = 0,
                Referral = ""
            };
            
            mockRepo.Setup(repo => repo.Register(user)).Returns(user);
            var service = new UserService(mockRepo.Object, config);
            var actual = service.RegisterUser(user);

            Assert.NotNull(actual);
            Assert.IsAssignableFrom<User>(actual);
            Assert.Equal(user, actual);

        }

        [Fact, TestPriority(2)]
        public void VerifyOtpShouldReturnTrue()
        {
            var mockRepo = new Mock<IUserRepository>();
            var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
            var clientId = config["SENDGRID_API_KEY"];

            User user = new User
            {
                Email = "Tester2@gmail.com",
                Password = "asdfg",
                Role = "Admin",
                IsVerified = false,
                Otp = 0,
                Referral = ""
            };
            string Email = "Tester2@gmail.com";

            mockRepo.Setup(repo => repo.FindUserById(Email)).Returns(user);
            mockRepo.Setup(repo => repo.VerifyMail(Email,user.Otp)).Returns(true);
            var service = new UserService(mockRepo.Object, config);
            var actual = service.VerifyOtp(user.Email, user.Otp);
            Assert.True(actual);
        }


        [Fact, TestPriority(3)]
        public void LoginShouldReturnUser()
        {
            var mockRepo = new Mock<IUserRepository>();
            var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
            var clientId = config["SENDGRID_API_KEY"];

            User user = new User
            {
                Email = "Tester2@gmail.com",
                Password = "asdfg",
                Role = "Admin",
                IsVerified = true,
                Otp = 0,
                Referral = ""
            };

            string Email = "Tester2@gmail.com";
            string Password = "asdfg";
            mockRepo.Setup(repo => repo.Login(Email,Password)).Returns(user);
            var service = new UserService(mockRepo.Object, config);
            var actual = service.Login(user.Email, user.Password);

            Assert.NotNull(actual);
            Assert.IsAssignableFrom<User>(actual);
            Assert.Equal(user, actual);

        }

        [Fact, TestPriority(4)]
        public void ResendOtpShouldReturnUser()
        {
            var mockRepo = new Mock<IUserRepository>();
            var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
            var clientId = config["SENDGRID_API_KEY"];

            User user = new User
            {
                Email = "Tester2@gmail.com",
                Password = "asdfg",
                Role = "Admin",
                IsVerified = true,
                Otp = 0,
                Referral = ""
            };

            mockRepo.Setup(repo => repo.FindUserById(user.Email)).Returns(user);
            mockRepo.Setup(repo => repo.Resend(user)).Returns(user);
            var service = new UserService(mockRepo.Object, config);
            var actual = service.ResendOtp(user.Email);

            Assert.NotNull(actual);
            Assert.IsAssignableFrom<User>(actual);
            Assert.Equal(user, actual);

        }

        [Fact, TestPriority(5)]
        public void ForgOtpasswordShouldReturnUser()
        {
            var mockRepo = new Mock<IUserRepository>();
            var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
            var clientId = config["SENDGRID_API_KEY"];

            User user = new User
            {
                Email = "Tester2@gmail.com",
                Password = "asdfg",
                Role = "Admin",
                IsVerified = true,
                Otp = 0,
                Referral = ""
            };

            mockRepo.Setup(repo => repo.FindUserById(user.Email)).Returns(user);
            mockRepo.Setup(repo => repo.UpdatePassword(user)).Returns(user);
            var service = new UserService(mockRepo.Object, config);
            var actual = service.Forgotpassword(user.Email);

            Assert.NotNull(actual);
            Assert.IsAssignableFrom<User>(actual);
            Assert.Equal(user, actual);

        }



        [Fact, TestPriority(6)]
        public void PostMessageShouldReturnOtp()
        {
            var mockRepo = new Mock<IUserRepository>();
            var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
            var clientId = config["SENDGRID_API_KEY"];

            User user = new User
            {
                Email = "Tester2@gmail.com",
                Password = "asdfg",
                Role = "Admin",
                IsVerified = true,
                Otp = 0,
                Referral = ""
            };

            var service = new UserService(mockRepo.Object, config);
            var actual = service.PostPassword(user);

            Assert.NotNull(actual);

        }

        [Fact, TestPriority(7)]
        public void PostPasswordShouldReturnString()
        {
            var mockRepo = new Mock<IUserRepository>();
            var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
            var clientId = config["SENDGRID_API_KEY"];

            User user = new User
            {
                Email = "Tester2@gmail.com",
                Password = "asdfg",
                Role = "Admin",
                IsVerified = true,
                Otp = 0,
                Referral = ""
            };

            var service = new UserService(mockRepo.Object, config);
            var actual = service.PostPassword(user);
            Assert.NotNull(actual);

        }



        [Fact, TestPriority(8)]
        public void ReferralServiceShouldReturnUser()
        {
            var mockRepo = new Mock<IUserRepository>();
            var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
            var clientId = config["SENDGRID_API_KEY"];

            User user = new User
            {
                Email = "Tester2@gmail.com",
                Password = "asdfg",
                Role = "Admin",
                IsVerified = true,
                Otp = 0,
                Referral = "asth4567"
            };
            User user1 = new User
            {
                Email = "Tester3@gmail.com",
                Password = "asdfg",
                Role = "User",
                IsVerified = false,
                Otp = 123456,
                Referral = "asth4567"
            };

            mockRepo.Setup(repo => repo.FindUserById(user.Email)).Returns(user);
            mockRepo.Setup(repo => repo.FindUserByReferral(user)).Returns(user1);
            var service = new UserService(mockRepo.Object, config);
            var actual = service.ReferralService(user);

            Assert.NotNull(actual);
            Assert.IsAssignableFrom<User>(user1);
            Assert.Equal(user1, actual);

        }


        #endregion Positive tests

        #region Negative tests

        [Fact, TestPriority(9)]
        public void RegisterUserShouldThrowException()
        {
            var mockRepo = new Mock<IUserRepository>();
            var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
            var clientId = config["SENDGRID_API_KEY"];

            User user = new User
            {
                Email = "Tester2@gmail.com",
                Password = "asdfg",
                Role = "Admin",
                IsVerified = true,
                Otp = 0,
                Referral = ""
            };

            mockRepo.Setup(repo => repo.FindUserById(user.Email)).Returns(user);
            mockRepo.Setup(repo => repo.Register(user)).Returns(user);
            var service = new UserService(mockRepo.Object, config);
            var actual = Assert.Throws<UserAlreadyExistsException>(() => service.RegisterUser(user));

        }

        [Fact, TestPriority(10)]
        public void VerifyOtpShouldThrowException()
        {
            var mockRepo = new Mock<IUserRepository>();
            var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
            var clientId = config["SENDGRID_API_KEY"];

            User user = new User
            {
                Email = "Tester2@gmail.com",
                Password = "asdfg",
                Role = "Admin",
                IsVerified = false,
                Otp = 0,
                Referral = ""
            };

            mockRepo.Setup(repo => repo.FindUserById(user.Email)).Returns(user);
            mockRepo.Setup(repo => repo.VerifyMail(user.Email, user.Otp)).Returns(false);
            var service = new UserService(mockRepo.Object, config);
            var actual = Assert.Throws<OtpIncorrectException>(() => service.VerifyOtp(user.Email,user.Otp));

        }


        [Fact, TestPriority(11)]
        public void ForgOtpasswordShouldThrowExceptionIfUserIsNotFound()
        {
            var mockRepo = new Mock<IUserRepository>();
            var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
            var clientId = config["SENDGRID_API_KEY"];

            User user = new User
            {
                Email = "Tester7@gmail.com",
                Password = "asdfg",
                Role = "",
                IsVerified = false,
                Otp = 0,
                Referral = ""
            };

            User user1 = null;

            mockRepo.Setup(repo => repo.FindUserById(user.Email)).Returns(user1);
            //mockRepo.Setup(repo => repo.UpdatePassword(user)).Returns(user);
            var service = new UserService(mockRepo.Object, config);
            var actual = Assert.Throws<UserNotFoundException>(() => service.Forgotpassword(user.Email));

        }

        [Fact, TestPriority(12)]
        public void ResendShouldThrowExceptionIfUserIsNotFound()
        {
            var mockRepo = new Mock<IUserRepository>();
            var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
            var clientId = config["SENDGRID_API_KEY"];

            User user = new User
            {
                Email = "Tester7@gmail.com",
                Password = "asdfg",
                Role = "",
                IsVerified = false,
                Otp = 0,
                Referral = ""
            };

            User user1 = null;

            mockRepo.Setup(repo => repo.FindUserById(user.Email)).Returns(user1);
            //mockRepo.Setup(repo => repo.UpdatePassword(user)).Returns(user);
            var service = new UserService(mockRepo.Object, config);
            var actual = Assert.Throws<UserNotFoundException>(() => service.ResendOtp(user.Email));

        }


        [Fact, TestPriority(13)]
        public void ReferralServiceShouldThrowExceptionIfUserNotFound()
        {
            var mockRepo = new Mock<IUserRepository>();
            var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
            var clientId = config["SENDGRID_API_KEY"];

            User user = new User
            {
                Email = "Tester2@gmail.com",
                Password = "asdfg",
                Role = "Admin",
                IsVerified = false,
                Otp = 0,
                Referral = "asdw3456"
            };

            User user1 = null;

            mockRepo.Setup(repo => repo.FindUserByReferral(user)).Returns(user1);
           // mockRepo.Setup(repo => repo.Resend(user)).Returns(user);
            var service = new UserService(mockRepo.Object, config);
            var actual = Assert.Throws<UserNotFoundException>(() => service.ReferralService(user));

        }

        [Fact, TestPriority(14)]
        public void ReferralServiceShouldThrowExceptionIfInvalidReferral()
        {
            var mockRepo = new Mock<IUserRepository>();
            var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
            var clientId = config["SENDGRID_API_KEY"];

            User user = new User
            {
                Email = "Tester2@gmail.com",
                Password = "asdfg",
                Role = "Admin",
                IsVerified = false,
                Otp = 0,
                Referral = "asdw3456"
            };

            User user1 = new User
            {
                Email = "Tester3@gmail.com",
                Password = "asdfg",
                Role = "Admin",
                IsVerified = false,
                Otp = 0,
                Referral = "asth1234"
            };

            mockRepo.Setup(repo => repo.FindUserByReferral(user)).Returns(user1);
            // mockRepo.Setup(repo => repo.Resend(user)).Returns(user);
            var service = new UserService(mockRepo.Object, config);
            var actual = Assert.Throws<UserNotFoundException>(() => service.ReferralService(user));

        }


        [Fact, TestPriority(15)]
        public void LoginShouldThrowNotVerifiedException()
        {
            var mockRepo = new Mock<IUserRepository>();
            var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
            var clientId = config["SENDGRID_API_KEY"];

            User user = new User
            {
                Email = "Tester2@gmail.com",
                Password = "asdfg",
                Role = "Admin",
                IsVerified = false,
                Otp = 0,
                Referral = ""
            };

            mockRepo.Setup(repo => repo.FindUserById(user.Email)).Returns(user);
            mockRepo.Setup(repo => repo.Login(user.Email, user.Password)).Returns(user);
            var service = new UserService(mockRepo.Object, config);
            var actual = Assert.Throws<UserNotVerifiedException>(() => service.Login(user.Email, user.Password));

        }


        [Fact, TestPriority(16)]
        public void LoginShouldThrowNotFoundException()
        {
            var mockRepo = new Mock<IUserRepository>();
            var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
            var clientId = config["SENDGRID_API_KEY"];

            User user = new User
            {
                Email = "Tester4@gmail.com",
                Password = "asdfg",
                Role = "Admin",
                IsVerified = false,
                Otp = 0,
                Referral = ""
            };

            User user1 = null;

            mockRepo.Setup(repo => repo.FindUserById(user.Email)).Returns(user1);
            var service = new UserService(mockRepo.Object, config);
            var actual = Assert.Throws<UserNotFoundException>(() => service.Login(user.Email, user.Password));

        }

        #endregion Negative tests
    }
}
