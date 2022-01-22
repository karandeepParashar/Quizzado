using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Authentication.Models;
using Authentication.Services;
using LeaderBoard.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using QuestionBank.Model;
using Xunit;

namespace Test.InfraSetup
{
    [ExcludeFromCodeCoverage]
    public class AuthWebApplicationFactory<TStartup> : WebApplicationFactory<Authentication.Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Create a new service provider.
                var serviceProvider = new ServiceCollection()
                    .AddEntityFrameworkInMemoryDatabase()
                    .BuildServiceProvider();
                // Add a database context (KeepNoteContext) using an in-memory database for testing.
                services.AddDbContext<UserDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryAuthDB");
                    options.UseInternalServiceProvider(serviceProvider);
                });
                // Build the service provider.
                var sp = services.BuildServiceProvider();
                // Create a scope to obtain a reference to the database contexts
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var authDb = scopedServices.GetRequiredService<UserDbContext>();
                    var logger = scopedServices.GetRequiredService<ILogger<AuthWebApplicationFactory<TStartup>>>();
                    // Ensure the database is created.
                    authDb.Database.EnsureCreated();
                    try
                    {
                        // Seed the database with some specific test data.
                        authDb.Users.Add(new Authentication.Models.User
                        { 
                            Email = "gupta.krishna71@gmail.com",
                            Password = "10123275227535025112076721815011724923924316613017811310419223411675442451422243555197",
                            IsVerified = true,
                            Referral = "abc1224",
                            Role = "User",
                            Otp = 123456,
                            
                        });
                        authDb.Users.Add(new Authentication.Models.User
                        {
                            Email = "Tester2@gmail.com",
                            Password = "2491052531901292913810211311113711536103992012242920175200812057226155865890240",
                            Role = "Admin",
                            IsVerified = true,
                            Otp = 123456
                        });

                        authDb.Users.Add(new Authentication.Models.User
                        {
                            Email = "Tester3@gmail.com",
                            Password = "2491052531901292913810211311113711536103992012242920175200812057226155865890240",
                            Role = "User",
                            IsVerified = false,
                            Otp = 123456
                        });
                        authDb.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An error occurred seeding the " +
                                            "database with test messages. Error: {ex.Message}");
                    }
                }
            });
        }
    }
    [ExcludeFromCodeCoverage]
    public class QuestionBankWebApplicationFactory<TStartup> : WebApplicationFactory<QuestionBank.Startup>
    {
        private IConfiguration configuration;
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                services.AddSingleton<IConfiguration>(configuration);
                services.AddScoped<QuestionsContext>();

                // Build the service provider.
                var sp = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database contexts
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var context = scopedServices.GetRequiredService<QuestionsContext>();
                    var logger = scopedServices.GetRequiredService<ILogger<QuestionBankWebApplicationFactory<TStartup>>>();

                    try
                    {
                        // Seed the database with some specific test data.
                        context.Question.DeleteMany(Builders<Questions>.Filter.Empty);

                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An error occurred seeding the " +
                                            "database with test messages. Error: {ex.Message}");
                    }
                }
            });
        }
    }
    [ExcludeFromCodeCoverage]
    public class CategoryWebApplicationFactory<TStartup> : WebApplicationFactory<QuestionBank.Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.AddScoped<CategoryContext>();

                // Build the service provider.
                var sp = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database contexts
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var context = scopedServices.GetRequiredService<CategoryContext>();
                    var logger = scopedServices.GetRequiredService<ILogger<CategoryWebApplicationFactory<TStartup>>>();

                    try
                    {
                        // Seed the database with some specific test data.
                        context.Category.DeleteMany(Builders<Category>.Filter.Empty);

                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An error occurred seeding the " +
                                            "database with test messages. Error: {ex.Message}");
                    }
                }
            });
        }
    }
    [ExcludeFromCodeCoverage]
    public class LeaderboardWebApplicationFactory<TStartup> : WebApplicationFactory<LeaderBoard.Startup>
    {
        private IConfigurationRoot configuration;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                services.AddSingleton<IConfiguration>(configuration);
                services.AddScoped<LeaderBoard.Model.IUserRepository,LeaderBoard.Model.UserRepository>();
                services.AddScoped<LeaderBoard.Service.IUserService, LeaderBoard.Service.UserService>();
                // Build the service provider.
                var sp = services.BuildServiceProvider();
                //Create a scope to obtain a reference to the database contexts
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    //var context = scopedServices.GetRequiredService<LeaderBoard.Model.UserRepository>();
                    //var logger = scopedServices.GetRequiredService<ILogger<LeaderboardWebApplicationFactory<TStartup>>>();
                    scopedServices.GetRequiredService<IConfiguration>().Bind(configuration);
                    try
                    {
                        
                    }
                    catch (Exception ex)
                    {
                        //logger.LogError(ex, "An error occurred seeding the " +
                           //                 "database with test messages. Error: {ex.Message}");
                    }
                }
            });
        }
    }
    [ExcludeFromCodeCoverage]
    [CollectionDefinition("Auth API")]
    public class DbCollection : ICollectionFixture<AuthWebApplicationFactory<Authentication.Startup>>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
