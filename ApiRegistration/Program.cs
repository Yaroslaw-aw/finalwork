using ApiRegistration.AuthorizationModel;
using ApiRegistration.Db;
using ApiRegistration.Dto;
using ApiRegistration.Mapping;
using ApiRegistration.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace ApiRegistration
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigurationBuilder? config = new ConfigurationBuilder();
            config.AddJsonFile("appsettings.json");
            IConfigurationRoot? configuration = config.Build();

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSingleton<RsaTools>();
            builder.Services.AddSingleton<Redis>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserAuthenticationService, UserAuthenticationService>();
            builder.Services.AddMemoryCache();
            builder.Services.AddAutoMapper(typeof(MappingProfile));


            string? postgresConnection = configuration.GetConnectionString("db");
            builder.Services.AddDbContext<UserContext>(options => options.LogTo(Console.WriteLine).UseNpgsql(postgresConnection));

            builder.Services.AddSwaggerGen(opt =>
            {
                opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "Token",

                    Scheme = "bearer"
                });

                opt.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });

            builder.Services.AddStackExchangeRedisCache(options =>
            {                
                options.Configuration = "localhost:6379";
            });

            string? identityUrl = builder.Configuration["Jwt:Issuer"];
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudiences = new[] { "reghost", "mailhost"},

                    IssuerSigningKey = new RsaSecurityKey(RsaTools.GetPublicKey())
                };
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
