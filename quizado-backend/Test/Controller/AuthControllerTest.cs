using Authentication;
using Authentication.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using Test.InfraSetup;
using Xunit;

namespace Test.Controller
{
    [ExcludeFromCodeCoverage]
    [Collection("Auth API")]
    [TestCaseOrderer("Test.PriorityOrderer", "Test")]
    public class AuthControllerTest : IClassFixture<AuthWebApplicationFactory<Authentication.Startup>>
    {
        #region Setup
        private readonly HttpClient _client;
        public AuthControllerTest(AuthWebApplicationFactory<Authentication.Startup> factory)
        {
            _client = factory.CreateClient();
        }
        #endregion


        #region Positive Tests

        [Fact, TestPriority(1)]
        public async Task RegisterUserShouldReturnUser()
        {
            User user = new User
            {
                Email = "Tester4@gmail.com",
                Password = "asdfg",
                Role = "User",
                IsVerified = false,
                Otp = 123456,
                Referral = "asdf1234"
            
            };
            MediaTypeFormatter formatter = new JsonMediaTypeFormatter();

            var httpResponse = await _client.PostAsync<User>("/auth/register", user, formatter);

            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var userDetails = JsonConvert.DeserializeObject<User>(stringResponse);
            Assert.Equal("Tester4@gmail.com", userDetails.Email);
        }


        [Fact, TestPriority(2)]
        public async Task VerifyShouldReturnToken()
        {
            User user = new User
            {
                Email = "Tester2@gmail.com",
                Password = "asdfg",
                Role = "Admin",
                IsVerified = false,
                Otp = 123456
            };
           
            MediaTypeFormatter formatter = new JsonMediaTypeFormatter();

            // The endpoint or route of the controller action.
            var httpResponse = await _client.PostAsync<User>("/auth/verify", user, formatter);

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK , httpResponse.StatusCode);

        }

        [Fact, TestPriority(6)]
        public async Task ResendOtpShouldReturnUser()
        {
            User user = new User
            {
                Email = "Tester2@gmail.com",
                Password = "asdfg",
                Role = "Admin",
                IsVerified = false,
                Otp = 0,
                Referral = ""

            };
            HttpRequestMessage request = new HttpRequestMessage();
            MediaTypeFormatter formatter = new JsonMediaTypeFormatter();

            // The endpoint or route of the controller action.
            var httpResponse = await _client.PostAsync<User>("/auth/resend", user, formatter);

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var userDetails = JsonConvert.DeserializeObject<User>(stringResponse);
            Assert.Equal(HttpStatusCode.Created, httpResponse.StatusCode);
            Assert.Equal("Tester2@gmail.com", userDetails.Email);
            Assert.Equal("Admin", userDetails.Role);
        }



        [Fact, TestPriority(4)]
        public async Task ForgOtpasswordShouldReturnUser()
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
            HttpRequestMessage request = new HttpRequestMessage();
            MediaTypeFormatter formatter = new JsonMediaTypeFormatter();

            // The endpoint or route of the controller action.
            var httpResponse = await _client.PostAsync<User>("/auth/forgot", user, formatter);

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var userDetails = JsonConvert.DeserializeObject<User>(stringResponse);
            Assert.Equal("Tester2@gmail.com", userDetails.Email);
            Assert.Equal("Admin", userDetails.Role);
            Assert.True(userDetails.IsVerified);
        }

        [Fact, TestPriority(5)]
        public async Task CheckReferralShouldReturnUser()
        {
            User user1 = new User
            {
                Email = "Tester2@gmail.com",
                Password = "asdfg",
                Role = "Admin",
                IsVerified = true,
                Otp = 0,
                Referral = "asth4567"

            };

            User user2 = new User
            {
                Email = "Tester3@gmail.com",
                Password = "asdfg",
                Role = "User",
                IsVerified = true,
                Otp = 0,
                Referral = "asth4567"

            };
            HttpRequestMessage request = new HttpRequestMessage();
            MediaTypeFormatter formatter = new JsonMediaTypeFormatter();

            // The endpoint or route of the controller action.
            var httpResponse = await _client.PostAsync<User>("/auth/referral", user1, formatter);

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var userDetails = JsonConvert.DeserializeObject<User>(stringResponse);
            Assert.Equal(HttpStatusCode.Created, httpResponse.StatusCode);

            Assert.IsAssignableFrom<User>(user2);

            //Assert.Equal("asthagupta259@gmail.com", userDetails.Email);
        }


        [Fact, TestPriority(3)]
        public async Task LoginShouldReturnToken()
        {
            User user = new User
            {
                Email = "Tester2@gmail.com",
                Password = "asdfg",
                Role = "Admin",
                IsVerified = true,
                Otp = 123456

            };
            HttpRequestMessage request = new HttpRequestMessage();
            MediaTypeFormatter formatter = new JsonMediaTypeFormatter();

            // The endpoint or route of the controller action.
            var httpResponse = await _client.PostAsync<User>("/auth/login", user, formatter);

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        }

        #endregion Positive Tests




        #region Negative Tests

        [Fact, TestPriority(7)]
        public async Task RegisterShouldReturnConflict()
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

            HttpRequestMessage request = new HttpRequestMessage();
            MediaTypeFormatter formatter = new JsonMediaTypeFormatter();

            // The endpoint or route of the controller action.
            var httpResponse = await _client.PostAsync<User>("/auth/register", user, formatter);

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            //var response = JsonConvert.DeserializeObject<Question>(stringResponse);
            Assert.Equal(HttpStatusCode.Conflict, httpResponse.StatusCode);
            Assert.Equal($"User Id {user.Email} already exists", stringResponse);
        }

        [Fact, TestPriority(8)]
        public async Task ForgOtpasswordShouldReturnNotFound()
        {
            User user = new User
            {
                Email = "Tester5@gmail.com",
                Password = "asdfg",
                Role = "Admin",
                IsVerified = true,
                Otp = 0,
                Referral = ""

            };

            HttpRequestMessage request = new HttpRequestMessage();
            MediaTypeFormatter formatter = new JsonMediaTypeFormatter();

            // The endpoint or route of the controller action.
            var httpResponse = await _client.PostAsync<User>("/auth/forgot", user, formatter);

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            //var response = JsonConvert.DeserializeObject<Question>(stringResponse);
            Assert.Equal(HttpStatusCode.NotFound, httpResponse.StatusCode);
            Assert.Equal("Invalid user id or password", stringResponse);
        }

        [Fact, TestPriority(9)]
        public async Task CheckReferralShouldReturnNotFound()
        {
            User user = new User
            {
                Email = "Tester5@gmail.com",
                Password = "asdfg",
                Role = "Admin",
                IsVerified = true,
                Otp = 0,
                Referral = "ser"

            };

            HttpRequestMessage request = new HttpRequestMessage();
            MediaTypeFormatter formatter = new JsonMediaTypeFormatter();

            // The endpoint or route of the controller action.
            var httpResponse = await _client.PostAsync<User>("/auth/referral", user, formatter);

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            //var response = JsonConvert.DeserializeObject<Question>(stringResponse);
            Assert.Equal(HttpStatusCode.NotFound, httpResponse.StatusCode);
            Assert.Equal("Invalid UserId or Password", stringResponse);
        }

        [Fact, TestPriority(10)]
        public async Task CheckReferralShouldReturnInvalid()
        {
            User user = new User
            {
                Email = "Tester2@gmail.com",
                Password = "asdfg",
                Role = "Admin",
                IsVerified = true,
                Otp = 0,
                Referral = "ser"

            };

            User user1 = new User
            {
                Email = "Tester3@gmail.com",
                Password = "asdfg",
                Role = "Admin",
                IsVerified = true,
                Otp = 0,
                Referral = "asth4567"

            };

            HttpRequestMessage request = new HttpRequestMessage();
            MediaTypeFormatter formatter = new JsonMediaTypeFormatter();

            // The endpoint or route of the controller action.
            var httpResponse = await _client.PostAsync<User>("/auth/referral", user, formatter);

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            //var response = JsonConvert.DeserializeObject<Question>(stringResponse);
            Assert.Equal(HttpStatusCode.NotFound, httpResponse.StatusCode);
            Assert.Equal("Invalid referral code", stringResponse);
        }

        [Fact, TestPriority(11)]
        public async Task LoginShouldReturnNotFound()
        {
            User user = new User
            {
                Email = "Tester5@gmail.com",
                Password = "asdfg",
                Role = "Admin",
                IsVerified = true,
                Otp = 0,
                Referral = ""

            };

            HttpRequestMessage request = new HttpRequestMessage();
            MediaTypeFormatter formatter = new JsonMediaTypeFormatter();

            // The endpoint or route of the controller action.
            var httpResponse = await _client.PostAsync<User>("/auth/login", user, formatter);

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            //var response = JsonConvert.DeserializeObject<Question>(stringResponse);
            Assert.Equal(HttpStatusCode.NotFound, httpResponse.StatusCode);
            Assert.Equal("Invalid user id or password", stringResponse);
        }

        [Fact, TestPriority(12)]
        public async Task LoginShouldReturnUnauthorised()
        {
            User user = new User
            {
                Email = "Tester3@gmail.com",
                Password = "asdfg",
                Role = "User",
                IsVerified = false,
                Otp = 0,
                Referral = ""

            };

            HttpRequestMessage request = new HttpRequestMessage();
            MediaTypeFormatter formatter = new JsonMediaTypeFormatter();

            // The endpoint or route of the controller action.
            var httpResponse = await _client.PostAsync<User>("/auth/login", user, formatter);

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            //var response = JsonConvert.DeserializeObject<Question>(stringResponse);
            Assert.Equal(HttpStatusCode.Unauthorized, httpResponse.StatusCode);
        }
        #endregion Negative Tests
    }
}
