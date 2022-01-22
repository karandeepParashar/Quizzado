using Authentication.Models;
using LeaderBoard.Model;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using Test.InfraSetup;
using Xunit;
using User = Authentication.Models.User;

namespace Test.Controller
{
    [ExcludeFromCodeCoverage]
    [Collection("Auth API")]
    [TestCaseOrderer("Test.PriorityOrderer", "Test")]
    public class UserControllerTest : IClassFixture<LeaderboardWebApplicationFactory<LeaderBoard.Startup>>
    {
        #region Setup
        private readonly HttpClient _client, _authclient;

        public UserControllerTest(LeaderboardWebApplicationFactory<LeaderBoard.Startup> factory, AuthWebApplicationFactory<Authentication.Startup> authFactory)
        {
            //calling Auth API to get JWT
            User user = new User { Email = "gupta.krishna71@gmail.com", Password = "qwerty" };
            _authclient = authFactory.CreateClient();
            HttpRequestMessage request = new HttpRequestMessage();
            MediaTypeFormatter formatter = new JsonMediaTypeFormatter();

            // The endpoint or route of the controller action.
            var httpResponse = _authclient.PostAsync<User>("/auth/login", user, formatter);
            httpResponse.Wait();
            // Deserialize and examine results.
            var stringResponse = httpResponse.Result.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<TokenModel>(stringResponse.Result);

            _client = factory.CreateClient();
            //Attaching token in request header
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {response.Token}");
        }
        private void Reset()
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            LeaderBoard.Model.UserRepository u = new LeaderBoard.Model.UserRepository(configuration);
            u.ResetDB();
        }

        #endregion
        #region Positive Test Case
        [Fact]
        public async void TestRegisterUser()
        {
            try
            {
                LeaderBoard.Model.User user = new LeaderBoard.Model.User();
                user.Email = "tester@gmail.com";
                user.Name = "Tester";
                user.PhoneNumber = "1234567890";
                user.ProfilePic = "Encrypted Image";
                user.Coins = 10;

                HttpRequestMessage request = new HttpRequestMessage();
                MediaTypeFormatter formatter = new JsonMediaTypeFormatter();

                // The endpoint or route of the controller action.
                var httpResponse = await _client.PostAsync<LeaderBoard.Model.User>("/api/user", user, formatter);

                var stringResponse = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<LeaderBoard.Model.User>(stringResponse);
                Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
            }
            finally
            {
                Reset();
            }
        }

        [Fact]
        public async void TestGetUserDetails()
        {
            try
            {
                LeaderBoard.Model.User user = new LeaderBoard.Model.User();
                user.Email = "tester@gmail.com";
                user.Name = "Tester";
                user.PhoneNumber = "1234567890";
                user.ProfilePic = "Encrypted Image";
                user.Coins = 10;

                HttpRequestMessage request = new HttpRequestMessage();
                MediaTypeFormatter formatter = new JsonMediaTypeFormatter();

                // The endpoint or route of the controller action.
                var httpResponse = await _client.PostAsync<LeaderBoard.Model.User>("/api/user", user, formatter);
                var httpResponse2 = await _client.GetAsync("/api/user/tester@gmail.com");
                var stringResponse = await httpResponse2.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<LeaderBoard.Model.User>(stringResponse);
                //Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
                Assert.Equal(HttpStatusCode.OK, httpResponse2.StatusCode);
                Assert.IsAssignableFrom<LeaderBoard.Model.User>(response);
            }
            finally
            {
                Reset();
            }
        }

        [Fact]
        public async void TestUpdateUserDetails()
        {
            try
            {
                LeaderBoard.Model.User user = new LeaderBoard.Model.User();
                user.Email = "tester@gmail.com";
                user.Name = "Tester";
                user.PhoneNumber = "1234567890";
                user.ProfilePic = "Encrypted Image";
                user.Coins = 10;

                HttpRequestMessage request = new HttpRequestMessage();
                MediaTypeFormatter formatter = new JsonMediaTypeFormatter();

                // The endpoint or route of the controller action.
                var httpResponse = await _client.PostAsync<LeaderBoard.Model.User>("/api/user", user, formatter);
                user.Name = "Unit Tester";
                HttpRequestMessage request2 = new HttpRequestMessage();
                MediaTypeFormatter formatter2 = new JsonMediaTypeFormatter();
                var httpResponse2 = await _client.PutAsync<LeaderBoard.Model.User>("/api/user", user, formatter2);
                var stringResponse = await httpResponse2.Content.ReadAsStringAsync();
                LeaderBoard.Model.User response = (LeaderBoard.Model.User)JsonConvert.DeserializeObject<LeaderBoard.Model.User>(stringResponse);
                Assert.Equal(user.Name, response.Name);
            }
            finally
            {
                Reset();
            }
        }

        [Fact]
        public async void TestAddScore()
        {
            try
            {
                LeaderBoard.Model.User user = new LeaderBoard.Model.User();
                user.Email = "tester@gmail.com";
                user.Name = "Tester";
                user.PhoneNumber = "1234567890";
                user.ProfilePic = "Encrypted Image";
                user.Coins = 10;
                HttpRequestMessage request = new HttpRequestMessage();
                MediaTypeFormatter formatter = new JsonMediaTypeFormatter();

                // The endpoint or route of the controller action.
                var httpResponse = await _client.PostAsync<LeaderBoard.Model.User>("/api/user", user, formatter);
                var httpResponse2 = await _client.PostAsync<Boolean>("/api/user/tester@gmail.com/1/90", false, formatter);
                var stringResponse = await httpResponse2.Content.ReadAsStringAsync();
                bool response = JsonConvert.DeserializeObject<Boolean>(stringResponse);
                Assert.True(response);
            }
            finally
            {
                Reset();
            }
        }

        [Fact]
        public async void TestGetRank()
        {
            try
            {
                LeaderBoard.Model.User user = new LeaderBoard.Model.User();
                user.Email = "tester@gmail.com";
                user.Name = "Tester";
                user.PhoneNumber = "1234567890";
                user.ProfilePic = "Encrypted Image";
                user.Coins = 10;
                HttpRequestMessage request = new HttpRequestMessage();
                MediaTypeFormatter formatter = new JsonMediaTypeFormatter();

                // The endpoint or route of the controller action.
                var httpResponse = await _client.PostAsync<LeaderBoard.Model.User>("/api/user", user, formatter);
                user.Email = "tester2@gmail.com";
                var httpResponse2 = await _client.PostAsync<LeaderBoard.Model.User>("/api/user", user, formatter);
                var httpResponse3 = await _client.PostAsync<Boolean>("/api/user/tester@gmail.com/1/90", false, formatter);
                var httpResponse4 = await _client.PostAsync<Boolean>("/api/user/tester2@gmail.com/1/100", false, formatter);
                var httpResponse5 = await _client.GetAsync("/api/user/tester@gmail.com/1");
                var httpResponse6 = await _client.GetAsync("/api/user/tester2@gmail.com/1");
                var stringResponse = await httpResponse5.Content.ReadAsStringAsync();
                var stringResponse2 = await httpResponse6.Content.ReadAsStringAsync();
                long? response = JsonConvert.DeserializeObject<long?>(stringResponse);
                long? response2 = JsonConvert.DeserializeObject<long?>(stringResponse2);
                Assert.Equal(1, response2);
                Assert.Equal(2, response);
            }
            finally
            {
                Reset();
            }
        }

        [Fact]
        public async void TestUpdateCoins()
        {
            try
            {
                LeaderBoard.Model.User user = new LeaderBoard.Model.User();
                user.Email = "tester@gmail.com";
                user.Name = "Tester";
                user.PhoneNumber = "1234567890";
                user.ProfilePic = "Encrypted Image";
                user.Coins = 10;
                HttpRequestMessage request = new HttpRequestMessage();
                MediaTypeFormatter formatter = new JsonMediaTypeFormatter();

                // User Created
                var httpResponse = await _client.PostAsync<LeaderBoard.Model.User>("/api/user", user, formatter);

                var httpResponse2 = await _client.PutAsync("/api/user/coins/tester@gmail.com/20", 0, formatter);
                var stringResponse = await httpResponse2.Content.ReadAsStringAsync();
                long response = JsonConvert.DeserializeObject<long>(stringResponse);
                Assert.Equal(20, response);
            }
            finally
            {
                Reset();
            }
        }
        #endregion

        #region Negative Test Cases
        [Fact]
        public async void TestRegisterUserUserAlreadyExists()
        {
            try
            {
                LeaderBoard.Model.User user = new LeaderBoard.Model.User();
                user.Email = "tester@gmail.com";
                user.Name = "Tester";
                user.PhoneNumber = "1234567890";
                user.ProfilePic = "Encrypted Image";
                user.Coins = 10;

                HttpRequestMessage request = new HttpRequestMessage();
                MediaTypeFormatter formatter = new JsonMediaTypeFormatter();

                // The endpoint or route of the controller action.
                var httpResponse = await _client.PostAsync<LeaderBoard.Model.User>("/api/user", user, formatter);
                var httpResponse2 = await _client.PostAsync<LeaderBoard.Model.User>("/api/user", user, formatter);
                var stringResponse = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<LeaderBoard.Model.User>(stringResponse);
                Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
                Assert.Equal(HttpStatusCode.Conflict, httpResponse2.StatusCode);
            }
            finally
            {
                Reset();
            }
        }
        [Fact]
        public async void TestGetUserDetailsUserNotFound()
        {
            var httpResponse = await _client.GetAsync("/api/user/tester@gmail.com");
            Assert.Equal(HttpStatusCode.NotFound, httpResponse.StatusCode);
        }
        [Fact]
        public async void TestUpdateUserDetailsUserNotFound()
        {
            try
            {
                LeaderBoard.Model.User user = new LeaderBoard.Model.User();
                user.Email = "tester@gmail.com";
                user.Name = "Tester";
                user.PhoneNumber = "1234567890";
                user.ProfilePic = "Encrypted Image";
                user.Coins = 10;
                user.Name = "Unit Tester";
                HttpRequestMessage request2 = new HttpRequestMessage();
                MediaTypeFormatter formatter2 = new JsonMediaTypeFormatter();
                var httpResponse2 = await _client.PutAsync<LeaderBoard.Model.User>("/api/user", user, formatter2);
                Assert.Equal(HttpStatusCode.NotFound, httpResponse2.StatusCode);
            }
            finally
            {
                Reset();
            }
        }

        [Fact]
        public async void TestAddScoreUserNotFound()
        {
            MediaTypeFormatter formatter = new JsonMediaTypeFormatter();
            var httpResponse2 = await _client.PostAsync<Boolean>("/api/user/tester@gmail.com/1/90", false, formatter);
            Assert.Equal(HttpStatusCode.NotFound, httpResponse2.StatusCode);
        }

        [Fact]
        public async void TestAddScoreCategoryNotFound()
        {
            MediaTypeFormatter formatter = new JsonMediaTypeFormatter();
            var httpResponse2 = await _client.PostAsync<Boolean>("/api/user/start/7/90", false, formatter);
            Assert.Equal(HttpStatusCode.NotFound, httpResponse2.StatusCode);
        }

        [Fact]
        public async void TestGetRankQuizNotAttempted()
        {
            try
            {
                LeaderBoard.Model.User user = new LeaderBoard.Model.User();
                user.Email = "tester@gmail.com";
                user.Name = "Tester";
                user.PhoneNumber = "1234567890";
                user.ProfilePic = "Encrypted Image";
                user.Coins = 10;
                HttpRequestMessage request = new HttpRequestMessage();
                MediaTypeFormatter formatter = new JsonMediaTypeFormatter();

                // The endpoint or route of the controller action.
                var httpResponse = await _client.PostAsync<LeaderBoard.Model.User>("/api/user", user, formatter);
                var httpResponse2 = await _client.GetAsync("/api/user/tester@gmail.com/1");
                var stringResponse = await httpResponse2.Content.ReadAsStringAsync();
                long? response = JsonConvert.DeserializeObject<long?>(stringResponse);
                Assert.Null(response);
            }
            finally
            {
                Reset();
            }
        }

        [Fact]
        public async void TestGetRankUserNotFound()
        {
            var httpResponse2 = await _client.GetAsync("/api/user/tester@gmail.com/1");
            Assert.Equal(HttpStatusCode.NotFound, httpResponse2.StatusCode);
        }

        [Fact]
        public async void TestUpdateCoinsUserNotFound()
        {
            MediaTypeFormatter formatter = new JsonMediaTypeFormatter();
            var httpResponse2 = await _client.PutAsync("/api/user/coins/tester@gmail.com/20", 0, formatter);
            Assert.Equal(HttpStatusCode.NotFound, httpResponse2.StatusCode);
        }
        #endregion
    }
}
