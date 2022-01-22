using Authentication.Models;
using Newtonsoft.Json;
using QuestionBank.Model;
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
    public class QuestionBankControllerTest : IClassFixture<QuestionBankWebApplicationFactory<QuestionBank.Startup>>
    {
        #region Setup
        private readonly HttpClient _client, _authclient;
        int genratedQuestionId;
        public QuestionBankControllerTest(QuestionBankWebApplicationFactory<QuestionBank.Startup> factory, AuthWebApplicationFactory<Authentication.Startup> authFactory)
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
        #endregion

        #region Positive Test Case

        [Fact, TestPriority(1)]
        public async Task AddQuestionShouldSuccess()
        {
            Questions question = new Questions
            {
                QuestionString = "Who is the highest scorer of FCBarcelona",
                CorrectOption = "Messi",
                Difficulty = 1,
                MarkedOption = null,
                Option1 = "Pique",
                Option2 = "Roberto",
                Option3 = "Messi",
                Option4 = "Ter Stegen"
            };
            
            HttpRequestMessage request = new HttpRequestMessage();
            MediaTypeFormatter formatter = new JsonMediaTypeFormatter();

            // The endpoint or route of the controller action.
            var httpResponse = await _client.PostAsync<Questions>("/api/questionbank/4", question, formatter);

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<Questions>(stringResponse);
            genratedQuestionId = response._id;
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
            Assert.IsAssignableFrom<Questions>(response);

        }

        [Fact, TestPriority(1)]
        public async Task AddQuestionWithMediumDificultyShouldSuccess()
        {
            Questions question = new Questions
            {
                QuestionString = "Who is the highest scorer of FCBarcelona",
                CorrectOption = "Messi",
                Difficulty = 2,
                MarkedOption = null,
                Option1 = "Pique",
                Option2 = "Roberto",
                Option3 = "Messi",
                Option4 = "Ter Stegen"
            };

            HttpRequestMessage request = new HttpRequestMessage();
            MediaTypeFormatter formatter = new JsonMediaTypeFormatter();

            // The endpoint or route of the controller action.
            var httpResponse = await _client.PostAsync<Questions>("/api/questionbank/4", question, formatter);

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<Questions>(stringResponse);
            genratedQuestionId = response._id;
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
            Assert.IsAssignableFrom<Questions>(response);

        }

        [Fact, TestPriority(1)]
        public async Task AddQuestionWithHardDificultyShouldSuccess()
        {
            Questions question = new Questions
            {
                QuestionString = "Who is the highest scorer of FCBarcelona",
                CorrectOption = "Messi",
                Difficulty = 3,
                MarkedOption = null,
                Option1 = "Pique",
                Option2 = "Roberto",
                Option3 = "Messi",
                Option4 = "Ter Stegen"
            };

            HttpRequestMessage request = new HttpRequestMessage();
            MediaTypeFormatter formatter = new JsonMediaTypeFormatter();

            // The endpoint or route of the controller action.
            var httpResponse = await _client.PostAsync<Questions>("/api/questionbank/4", question, formatter);

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<Questions>(stringResponse);
            genratedQuestionId = response._id;
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
            Assert.IsAssignableFrom<Questions>(response);

        }

        [Fact, TestPriority(3)]
        public async Task UpdateQuestionShouldSuccess()
        {
            Questions question = new Questions
            {
                _id = 8500,
                QuestionString = "Who is the highest scorer of FCBarcelona",
                CorrectOption = "Messi",
                Difficulty = 3,
                MarkedOption = null,
                Option1 = "Pique",
                Option2 = "Roberto",
                Option3 = "Messi",
                Option4 = "Ter Stegen"
            };

            HttpRequestMessage request = new HttpRequestMessage();
            MediaTypeFormatter formatter = new JsonMediaTypeFormatter();

            // The endpoint or route of the controller action.
            var httpResponse = await _client.PutAsync<Questions>("/api/questionbank/update", question, formatter);

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<Questions>(stringResponse);
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        }

        [Fact, TestPriority(5)]
        public async Task GetQuestionsByCategoryAndDifficultyShouldSuccess()
        {
            int categoryId = 4, start = 1, count = 5;
            int difficulty = 1;

            // The endpoint or route of the controller action.
            var httpResponse = await _client.GetAsync($"/api/questionbank/{categoryId}/{difficulty}/{start}/{count}");

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<List<Questions>>(stringResponse);
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
            Assert.IsAssignableFrom<List<Questions>>(response);
        }

        [Fact, TestPriority(7)]
        public async Task GetNextQuestionShouldSuccessWithHigherDifficulty()
        {
            Questions question = new Questions
            {
                _id = 8500,
                QuestionString = "Who is the highest scorer of FCBarcelona",
                CorrectOption = "Messi",
                Difficulty = 2,
                MarkedOption = "Messi",
                Option1 = "Pique",
                Option2 = "Roberto",
                Option3 = "Messi",
                Option4 = "Ter Stegen"
            };
            int categoryId = 4;

            HttpRequestMessage request = new HttpRequestMessage();
            MediaTypeFormatter formatter = new JsonMediaTypeFormatter();

            // The endpoint or route of the controller action.
            var httpResponse = await _client.PostAsync<Questions>($"/api/questionbank/NextQuestion/{categoryId}", question, formatter);

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<Questions>(stringResponse);
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
            Assert.Equal(3, response.Difficulty);
        }

        [Fact, TestPriority(7)]
        public async Task GetNextQuestionShouldSuccessWithLowerDifficulty()
        {
            Questions question = new Questions
            {
                _id = 8500,
                QuestionString = "Who is the highest scorer of FCBarcelona",
                CorrectOption = "Messi",
                Difficulty = 2,
                MarkedOption = "sdgdsg",
                Option1 = "Pique",
                Option2 = "Roberto",
                Option3 = "Messi",
                Option4 = "Ter Stegen"
            };
            int categoryId = 4;

            HttpRequestMessage request = new HttpRequestMessage();
            MediaTypeFormatter formatter = new JsonMediaTypeFormatter();

            // The endpoint or route of the controller action.
            var httpResponse = await _client.PostAsync<Questions>($"/api/questionbank/NextQuestion/{categoryId}", question, formatter);

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<Questions>(stringResponse);
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
            Assert.Equal(1, response.Difficulty);
        }

        [Fact, TestPriority(9)]
        public async Task GetFirstQuestionShouldSuccess()
        {
            int categoryId = 4;
            // The endpoint or route of the controller action.
            var httpResponse = await _client.GetAsync($"/api/questionbank/NextQuestion/{categoryId}");

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<Questions>(stringResponse);
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
            Assert.IsAssignableFrom<Questions>(response);
        }

        [Fact, TestPriority(11)]
        public async Task DeleteQuestionShouldSuccess()
        {
            Questions question = new Questions
            {
                _id = 8500,
                QuestionString = "Who is the highest scorer of FCBarcelona",
                CorrectOption = "Messi",
                Difficulty = 3,
                MarkedOption = null,
                Option1 = "Pique",
                Option2 = "Roberto",
                Option3 = "Messi",
                Option4 = "Ter Stegen"
            };

            HttpRequestMessage request = new HttpRequestMessage();
            MediaTypeFormatter formatter = new JsonMediaTypeFormatter();

            // The endpoint or route of the controller action.
            var httpResponse = await _client.PutAsync<Questions>("/api/questionbank/delete/4", question, formatter);

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<Questions>(stringResponse);
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        }

        #endregion

        #region Negative Test Case

        [Fact, TestPriority(2)]
        public async Task AddQuestionShouldReturnNotFound()
        {
            Questions question = new Questions
            {
                QuestionString = "Who is the highest scorer of FCBarcelona",
                CorrectOption = "Messi",
                Difficulty = 3,
                MarkedOption = null,
                Option1 = "Pique",
                Option2 = "Roberto",
                Option3 = "Messi",
                Option4 = "Ter Stegen"
            };
            
            HttpRequestMessage request = new HttpRequestMessage();
            MediaTypeFormatter formatter = new JsonMediaTypeFormatter();

            // The endpoint or route of the controller action.
            var httpResponse = await _client.PostAsync<Questions>("/api/questionbank/10", question, formatter);

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            //var response = JsonConvert.DeserializeObject<Question>(stringResponse);
            Assert.Equal(HttpStatusCode.NotFound, httpResponse.StatusCode);
        }

        [Fact, TestPriority(4)]
        public async Task UpdateQuestionShouldReturnNotFound()
        {
            Questions question = new Questions
            {
                _id = 211211,
                QuestionString = "Who is the captain of FC Barcelona",
                CorrectOption = "Messi",
                Difficulty = 1,
                MarkedOption = null,
                Option1 = "Pique",
                Option2 = "Roberto",
                Option3 = "Messi",
                Option4 = "Ter Stegen"
            };
           

            HttpRequestMessage request = new HttpRequestMessage();
            MediaTypeFormatter formatter = new JsonMediaTypeFormatter();

            // The endpoint or route of the controller action.
            var httpResponse = await _client.PutAsync<Questions>("/api/questionbank/update", question, formatter);

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            //var response = JsonConvert.DeserializeObject<Question>(stringResponse);
            Assert.Equal(HttpStatusCode.NotFound, httpResponse.StatusCode);
        }

        [Fact, TestPriority(6)]
        public async Task GetQuestionsByCategoryAndDifficultyShouldReturnNotFound()
        {
            int categoryId = 10, start = 1, count = 5;
            int difficulty = 1;

            // The endpoint or route of the controller action.
            var httpResponse = await _client.GetAsync($"/api/questionbank/{categoryId}/{difficulty}/{start}/{count}");

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            //var response = JsonConvert.DeserializeObject<List<Question>>(stringResponse);
            Assert.Equal(HttpStatusCode.NotFound, httpResponse.StatusCode);
        }

        [Fact, TestPriority(8)]
        public async Task GetNextQuestionShouldThrowBadRequest()
        {
            Questions question = new Questions
            {
                QuestionString = "Who is the highest scorer of FCBarcelona",
                CorrectOption = "Messi",
                Difficulty = 1,
                MarkedOption = null,
                Option1 = "Pique",
                Option2 = "Roberto",
                Option3 = "Messi",
                Option4 = "Ter Stegen"
            };
            int categoryId = 4;

            HttpRequestMessage request = new HttpRequestMessage();
            MediaTypeFormatter formatter = new JsonMediaTypeFormatter();

            // The endpoint or route of the controller action.
            var httpResponse = await _client.PostAsync<Questions>($"/api/questionbank/NextQuestion/{categoryId}", question, formatter);

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            //var response = JsonConvert.DeserializeObject<Question>(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
        }

        [Fact, TestPriority(10)]
        public async Task GetFirstQuestionShouldReturnNotFound()
        {
            int categoryId = 10;
            // The endpoint or route of the controller action.
            var httpResponse = await _client.GetAsync($"/api/questionbank/NextQuestion/{categoryId}");

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            //var response = JsonConvert.DeserializeObject<Question>(stringResponse);
            Assert.Equal(HttpStatusCode.NotFound, httpResponse.StatusCode);
        }

        [Fact, TestPriority(12)]
        public async Task DeleteQuestionShouldReturnNotFound()
        {
            Questions question = new Questions
            {
                _id = 100000,
                QuestionString = "Who is the highest scorer of FCBarcelona",
                CorrectOption = "Messi",
                Difficulty = 1,
                MarkedOption = null,
                Option1 = "Pique",
                Option2 = "Roberto",
                Option3 = "Messi",
                Option4 = "Ter Stegen"
            };

            HttpRequestMessage request = new HttpRequestMessage();
            MediaTypeFormatter formatter = new JsonMediaTypeFormatter();

            // The endpoint or route of the controller action.
            var httpResponse = await _client.PutAsync<Questions>("/api/questionbank/delete/19", question, formatter);

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            //var response = JsonConvert.DeserializeObject<Question>(stringResponse);
            Assert.Equal(HttpStatusCode.NotFound, httpResponse.StatusCode);
        }
        #endregion
    }
}

