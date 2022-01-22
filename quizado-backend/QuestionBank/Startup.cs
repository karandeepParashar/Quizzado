using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using QuestionBank.Model;
using QuestionBank.Repository;
using QuestionBank.Service;

namespace QuestionBank
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            this.ValidateToken(Configuration, services);
            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddScoped<QuestionsContext>();
            services.AddScoped<CategoryContext>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IQuestionBankRepository, QuestionBankRepository>();
            services.AddScoped<IQuestionBankService, QuestionBankService>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); 
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseCors(MyAllowSpecificOrigins);
            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseMvc();
        }

        private void ValidateToken(IConfiguration configuration, IServiceCollection services)
        {
            
            var audienceConfig = configuration.GetSection("Audience");
            var secretKey = audienceConfig["key"];
            var keyByteArray = Encoding.ASCII.GetBytes(secretKey);
            var signature = new SymmetricSecurityKey(keyByteArray);
            var tokenParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signature,

                ValidateIssuer = true,
                ValidIssuer = audienceConfig["issuer"],

                ValidateAudience = true,
                ValidAudience = audienceConfig["audience"],

                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero

            };
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o => {
                o.TokenValidationParameters = tokenParameters;
            });
        }
    }
}
