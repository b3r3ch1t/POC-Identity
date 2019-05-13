using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Poc.Infra.CrossCuting.IoC;
using Poc.Infra.CrossCutting.Identity.Authorization;
using Poc.Infra.CrossCutting.Identity.Interfaces;
using Poc.Infra.CrossCutting.Identity.Middlaware;
using Poc.WebApi.Controllers;
using Poc.WebApi.Extensions;
using Poc.WebApi.Interfaces;
using Poc.WebApi.Middleware;
using Poc.WebApi.Services;
using Swashbuckle.AspNetCore.Swagger;
using System;

namespace Poc.WebApi
{
    public class Startup
    {

        private const string JwtIssue = "POC123";
        private readonly string _jwtAudience;
        private SecurityKey SigningKey => JwtTokenOptions.SigningKey;
        private readonly string _connectionString;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            Configuration = builder.Build();

            _jwtAudience = env.ApplicationName;

            _connectionString = Configuration["ConnectionStrings:poc"];
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            #region MediatR

            // Adding MediatR for Domain Events and Notifications
            services.AddMediatR(typeof(Startup));


            #endregion

            //Configurando o Identity
            services.ConfigureIdentity(_connectionString);
            services.AddOptions();

            #region IIS

            // inizio IIS options
            services.Configure<IISServerOptions>(options =>
            {
                options.AutomaticAuthentication = false;
            });
            services.Configure<IISOptions>(options =>
            {
                options.ForwardClientCertificate = false;
            });
            // fine IIS options

            #endregion

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            #region CORS

            services.AddCors(options =>
            {
                options.AddPolicy("AllowOrigin",
                    builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .AllowAnyHeader()
                    .Build());
            });
            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new CorsAuthorizationFilterFactory("AllowOrigin"));
            });

            #endregion

            #region Jwt

            //Configure JwtTokenOptions
            services.Configure<JwtTokenOptions>(options =>
            {
                options.Issuer = JwtIssue;
                options.Audience = _jwtAudience;

            });
            var tokenValidationParameters = new TokenValidationParameters
            {

                ValidateIssuer = true,
                ValidIssuer = JwtIssue,
                ValidateAudience = true,
                ValidAudience = _jwtAudience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = SigningKey,
                RequireExpirationTime = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(5)
            };


            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = tokenValidationParameters;
            });

            #endregion

            services.AddAutoMapperSetup();



            services.AddHealthChecks();

            services
              .AddSingleton(Configuration);

            // .NET Native DI Abstraction
            RegisterServices(services, Configuration);


            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "POC App Api (v1)",
                    Description = "POC App Api (v1)",
                    TermsOfService = "None",
                    Contact = new Contact
                    {
                        Name = "janmaru",
                        Email = string.Empty,
                        Url = ""
                    },
                    License = new License
                    {
                        Name = "Use under LICX",
                        Url = "https://example.com/license"
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            #region Erros

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error/500");

            }

            app.UseErrorHandling();

            app.UseStatusCodePagesWithReExecute("/Error/{0}");

            #endregion



            app.UseHealthChecks("/healthcheck");

            app.UseStaticFiles();
            app.UseMvc();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "POC App Api (v1)");
            });

            //app.UseCors(option => option.AllowAnyOrigin());
        }

        private static void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            // Adding dependencies from another layers (isolated from Presentation)
            NativeInjectorBootStrapper.RegisterServices(services, configuration);
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IBaseUserInterface, BaseUserInterface>();
        }


    }
}
