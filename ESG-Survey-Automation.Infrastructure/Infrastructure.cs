using ESG_Survey_Automation.Infrastructure.FileStorage;
using ESG_Survey_Automation.Infrastructure.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ESG_Survey_Automation.Domain.ConfigModels;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ESG_Survey_Automation.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;
using ESG_Survey_Automation.Infrastructure.Swagger;

namespace ESG_Survey_Automation.Infrastructure
{
    public static class Infrastructure
    {
        public static void AddInfrastructure(this IServiceCollection services)
        {
            var builder = services.BuildServiceProvider();
            var conf = builder.GetService<IConfiguration>();
            services.AddDbContext<ESGSurveyContext>(o => o.UseSqlServer(conf.GetConnectionString("DefaultConnection")));
            //GCPLoggerProvider provider = new(conf);
            GCPConfig _config = new(conf);
            //services.AddLogging(options => options.ClearProviders().AddProvider(provider));
            services.AddSingleton<IFileStorage, GCPFileStorage>();
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = conf.GetValue<string>("JwtOptions:Issuer"),
                    ValidAudience = conf.GetValue<string>("JwtOptions:Audience"),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(conf.GetValue<string>("JwtOptions:SecurityKey"))),
                    ValidateLifetime = true
                };
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ESG Survey Automation", Version = "v1" });

                c.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme.",
                });
                c.OperationFilter<AuthOperationFilter>();
            });
            services.AddSwaggerGenNewtonsoftSupport();
        }

        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder builder)
        {
            builder.UseSwagger();
            var conf = builder.ApplicationServices.GetService<IConfiguration>();
            GCPConfig _config = new(conf);
            builder.UseSwaggerUI(c =>
            {
                c.OAuthUsePkce();
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ESG Survey Automation V1");
                c.OAuthClientId(_config.ClientId);
                c.OAuthAppName("ESG Survey Automation");
                c.OAuthScopeSeparator(" ");
                c.OAuthScopes(["openid", "profile", "email"]);
            });
            return builder;
        }
    }
}

