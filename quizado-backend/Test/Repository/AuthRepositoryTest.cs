using Authentication.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Test.InfraSetup;
using Xunit;

namespace Test.Repository
{
    [ExcludeFromCodeCoverage]
    [TestCaseOrderer("Test.PriorityOrderer", "Test")]
    public class AuthRepositoryTest : IClassFixture<AuthDbFixture>
    {
        private readonly IUserRepository repository;
        public AuthRepositoryTest(AuthDbFixture fixture)
        {
            repository = new UserRepository(fixture.context);
        }

        [Fact, TestPriority(1)]
        public void RegisterUserShouldReturnUser()
        {
            User user = new User
            {
                Email = "Tester2@gmail.com",
                Password = "asdfg",
                Role = "Admin",
                IsVerified = true,
                Otp = 0,
                Referral = "asth1234"
            };
          
            var actual = repository.Register(user);
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<User>(actual);
            Assert.Equal(user, actual);
        }

        [Fact, TestPriority(2)]
        public void ToShaShouldReturnString()
        {
            User user = new User
            {
                Email = "Tester2@gmail.com",
                Password = "asdfg",
                Role = "Admin",
                IsVerified = true,
                Otp = 0,
                Referral = "asth1234"
            };

            var actual = repository.ToSHA256(user.Password);
            Assert.NotNull(actual);
            Assert.Equal("2491052531901292913810211311113711536103992012242920175200812057226155865890240", actual);
        }


        [Fact, TestPriority(3)]
        public void GenerateReferralReturnString()
        {
            User user = new User
            {
                Email = "Tester2@gmail.com",
                Password = "asdfg",
                Role = "Admin",
                IsVerified = true,
                Otp = 0,
                Referral = ""
            };

            var actual = repository.GenerateReferral();
            Assert.NotNull(actual);
        }

        [Fact, TestPriority(4)]
        public void FindUserByIdShouldSuccess()
        {
            var user = repository.FindUserById("Tester2@gmail.com");

            Assert.IsAssignableFrom<User>(user);
            Assert.Equal("User", user.Role);
        }
        
        [Fact, TestPriority(7)]
         public void FindUserByReferralShouldSuccess()
        {
            User _user1 = new User
            {
                Email = "Tester2@gmail.com",
                Password = "asdfg",
                Role = "Admin",
                IsVerified = true,
                Otp = 0,
                Referral = "asth4567"
            };
            User _user2 = new User
            {
                Email = "Tester3@gmail.com",
                Password = "asdfg",
                Role = "User",
                IsVerified = true,
                Otp = 0,
                Referral = "asth4567"
            };
            var user = repository.FindUserByReferral(_user1);

            Assert.IsAssignableFrom<User>(_user2);
            Assert.Equal("User", _user2.Role);
        }

        [Fact, TestPriority(6)]
        public void ResendUserShouldReturnUser()
        {
            var user = repository.FindUserById("Tester2@gmail.com");

            var actual = repository.Resend(user);
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<User>(actual);
            Assert.Equal(user.Email, actual.Email);
            Assert.Equal("14470151501621514621981356614222415648192532224381774811542733166124102212396", repository.ToSHA256(actual.Password));
            Assert.Equal(user.Role, actual.Role);
            Assert.Equal(user.IsVerified, actual.IsVerified);
            Assert.Equal(user.Otp, actual.Otp);
        }


        [Fact, TestPriority(5)]
        public void VerifyMailUserShouldSuccess()
        {
            var user = repository.FindUserById("Tester2@gmail.com");

            string email = user.Email;
            int Otp = user.Otp ;
            var actual = repository.VerifyMail(email, Otp);

            Assert.True(actual);
        }

        [Fact, TestPriority(8)]
        public void VerifyMailUserShouldFail()
        {
            var user = repository.FindUserById("Tester@gmail.com");
            Assert.Null(user);
        }

        [Fact, TestPriority(9)]
        public void LoginShouldReturnUser()
        {
            var actual = repository.Login("Tester2@gmail.com", "asdfg");
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<User>(actual);
            Assert.Equal("Tester2@gmail.com", actual.Email);
            Assert.Equal("14470151501621514621981356614222415648192532224381774811542733166124102212396", repository.ToSHA256(actual.Password));

        }

        [Fact, TestPriority(10)]
        public void UpdatePasswordShouldReturnUser()
        {
            var user = repository.FindUserById("Tester2@gmail.com");

            var actual = repository.UpdatePassword(user);
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<User>(actual);
            Assert.Equal(user.Email, actual.Email);
            Assert.Equal(user.Role, actual.Role);
            Assert.Equal(user.IsVerified, actual.IsVerified);
            Assert.Equal(user.Otp, actual.Otp);
        }

    }
}

