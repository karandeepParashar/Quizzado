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
    public class CategoryControllerTest : IClassFixture<CategoryWebApplicationFactory<QuestionBank.Startup>>
    {
        #region Setup
        private readonly HttpClient _client, _authclient;
        public CategoryControllerTest(CategoryWebApplicationFactory<QuestionBank.Startup> factory, AuthWebApplicationFactory<Authentication.Startup> authFactory)
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
        public async Task GetCategoriesShouldSuccess()
        {
            HttpRequestMessage request = new HttpRequestMessage();
            MediaTypeFormatter formatter = new JsonMediaTypeFormatter();

            // The endpoint or route of the controller action.
            var httpResponse = await _client.GetAsync("/api/category/");

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<List<Category>>(stringResponse);
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
            Assert.IsAssignableFrom<List<Category>>(response);
        }

        [Fact, TestPriority(2)]
        public async Task GetCategoryByIdShouldSuccess()
        {
            HttpRequestMessage request = new HttpRequestMessage();
            MediaTypeFormatter formatter = new JsonMediaTypeFormatter();

            // The endpoint or route of the controller action.
            var httpResponse = await _client.GetAsync("/api/category/1");

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<Category>(stringResponse);
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
            Assert.IsAssignableFrom<Category>(response);
        }

        [Fact, TestPriority(3)]
        public async Task AddShouldSuccess()
        {
            Category category = new Category
            {
                Name = "TestCategory",
                ImageUrl = "TestUrl"
            };
            HttpRequestMessage request = new HttpRequestMessage();
            MediaTypeFormatter formatter = new JsonMediaTypeFormatter();

            // The endpoint or route of the controller action.
            var httpResponse = await _client.PostAsync<Category>("/api/category/", category, formatter);

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<Category>(stringResponse);
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
            Assert.IsAssignableFrom<Category>(response);
        }

        [Fact, TestPriority(4)]
        public async Task UpdateCategoryShouldSuccess()
        {
            Category category = new Category
            {
                _id = 1,
                Name = "TestCategory1",
                ImageUrl = "TestUrl"
            };
            HttpRequestMessage request = new HttpRequestMessage();
            MediaTypeFormatter formatter = new JsonMediaTypeFormatter();

            // The endpoint or route of the controller action.
            var httpResponse = await _client.PostAsync<Category>("/api/category/", category, formatter);

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            //var response = JsonConvert.DeserializeObject<Category>(stringResponse);
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        }


        #endregion

    }
}
