using Authentication.Models;
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

namespace Test.Controller
{
    [ExcludeFromCodeCoverage]
    [Collection("Auth API")]
    [TestCaseOrderer("Test.PriorityOrderer", "Test")]
    public class LeaderboardControllerTest : IClassFixture<LeaderboardWebApplicationFactory<LeaderBoard.Startup>>
    {
        #region Setup
        private readonly HttpClient _client, _authclient;

        public LeaderboardControllerTest(LeaderboardWebApplicationFactory<LeaderBoard.Startup> factory, AuthWebApplicationFactory<Authentication.Startup> authFactory)
        {
            //calling Auth API to get JWT
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
        public async void TestGetLeaderboard()
        {
            try
            {
                LeaderBoard.Model.User user = new LeaderBoard.Model.User();
                user.Email = "tester1@gmail.com";
                user.Name = "Tester";
                user.PhoneNumber = "1234567890";
                user.ProfilePic = "Encrypted Image";
                user.Coins = 10;
                HttpRequestMessage request = new HttpRequestMessage();
                MediaTypeFormatter formatter = new JsonMediaTypeFormatter();
                var createUser1 = await _client.PostAsync<LeaderBoard.Model.User>("/api/user", user, formatter);
                var addScoreUser1 = await _client.PostAsync<Boolean>("/api/user/tester1@gmail.com/1/10", false, formatter);
                user.Email = "tester2@gmail.com";
                var createUser2 = await _client.PostAsync<LeaderBoard.Model.User>("/api/user", user, formatter);
                var addScoreUser2 = await _client.PostAsync<Boolean>("/api/user/tester2@gmail.com/1/20", false, formatter);
                user.Email = "tester3@gmail.com";
                var createUser3 = await _client.PostAsync<LeaderBoard.Model.User>("/api/user", user, formatter);
                var addScoreUser3 = await _client.PostAsync<Boolean>("/api/user/tester3@gmail.com/1/30", false, formatter);
                user.Email = "tester4@gmail.com";
                var createUser4 = await _client.PostAsync<LeaderBoard.Model.User>("/api/user", user, formatter);
                var addScoreUser4 = await _client.PostAsync<Boolean>("/api/user/tester4@gmail.com/1/40", false, formatter);
                user.Email = "tester5@gmail.com";
                var createUser5 = await _client.PostAsync<LeaderBoard.Model.User>("/api/user", user, formatter);
                var addScoreUser5 = await _client.PostAsync<Boolean>("/api/user/tester5@gmail.com/1/50", false, formatter);
                user.Email = "tester6@gmail.com";
                var createUser6 = await _client.PostAsync<LeaderBoard.Model.User>("/api/user", user, formatter);
                var addScoreUser6 = await _client.PostAsync<Boolean>("/api/user/tester6@gmail.com/1/60", false, formatter);
                user.Email = "tester7@gmail.com";
                var createUser7 = await _client.PostAsync<LeaderBoard.Model.User>("/api/user", user, formatter);
                var addScoreUser7 = await _client.PostAsync<Boolean>("/api/user/tester7@gmail.com/1/70", false, formatter);
                user.Email = "tester8@gmail.com";
                var createUser8 = await _client.PostAsync<LeaderBoard.Model.User>("/api/user", user, formatter);
                var addScoreUser8 = await _client.PostAsync<Boolean>("/api/user/tester8@gmail.com/1/80", false, formatter);
                user.Email = "tester9@gmail.com";
                var createUser9 = await _client.PostAsync<LeaderBoard.Model.User>("/api/user", user, formatter);
                var addScoreUser9 = await _client.PostAsync<Boolean>("/api/user/tester9@gmail.com/1/90", false, formatter);
                user.Email = "tester10@gmail.com";
                var createUser10 = await _client.PostAsync<LeaderBoard.Model.User>("/api/user", user, formatter);
                var addScoreUser10 = await _client.PostAsync<Boolean>("/api/user/tester10@gmail.com/1/100", false, formatter);

                var getLeaedrboard = await _client.GetAsync("/api/leaderboard/1");
                var stringResponse = await getLeaedrboard.Content.ReadAsStringAsync();
                List<LeaderBoard.Model.Leaderboard> response = JsonConvert.DeserializeObject<List<LeaderBoard.Model.Leaderboard>>(stringResponse);

                for (int i = 0; i <10; i++)
                {
                    string email = "tester"+(10-i)+"@gmail.com";
                    Assert.Equal(email,response[i].Email);
                }
            }
            finally
            {
                Reset();
            }
        }
        [Fact]
        public async void TestAddCategory() 
        {
            try
            {
                HttpRequestMessage request = new HttpRequestMessage();
                MediaTypeFormatter formatter = new JsonMediaTypeFormatter();
                var getLeaedrboard = await _client.PostAsync<Boolean>("/api/leaderboard/6",false,formatter);
                var stringResponse = await getLeaedrboard.Content.ReadAsStringAsync();
                bool response = JsonConvert.DeserializeObject<Boolean>(stringResponse);
                Assert.True(response);
            }
            finally
            {
                Reset();
            }
        }
        #endregion

        #region Negative Test Cases
        [Fact]
        public async void TestGetLeaderboardCategoryNotFound() 
        {
            var getLeaderboard = await _client.GetAsync("/api/leaderboard/6");
            Assert.Equal(HttpStatusCode.NotFound, getLeaderboard.StatusCode);
        }
        [Fact]
        public async void TestAddCategoryAlreadyExists()
        {
            try
            {
                HttpRequestMessage request = new HttpRequestMessage();
                MediaTypeFormatter formatter = new JsonMediaTypeFormatter();
                var getLeaedrboard = await _client.PostAsync<Boolean>("/api/leaderboard/1", false, formatter);
                Assert.Equal(HttpStatusCode.Conflict,getLeaedrboard.StatusCode);
            }
            finally
            {
                Reset();
            }
        }
        #endregion
    }
}
