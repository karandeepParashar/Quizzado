using Authentication.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Test.InfraSetup
{
    [ExcludeFromCodeCoverage]
    public class AuthDbFixture:IDisposable
    {
        public UserDbContext context;
        public AuthDbFixture()
        {
            var options = new DbContextOptionsBuilder<UserDbContext>()
                .UseInMemoryDatabase(databaseName: "AuthDB")
                .Options;
            context = new UserDbContext(options);
           
            //context.Users.Add(new User {
            //    Email = "asthagupta259@gmail.com",
            //    Password = "asdfg",
            //    Role = "User",
            //    isVerified = false,
            //    otp = 0
            //});
           
            context.SaveChanges();
        }
        public void Dispose()
        {
            context = null;
        }
    }
}
