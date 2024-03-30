using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace ApiGatewayMail
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("ocelot.json").Build();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudiences = new[] { "reghost", "mailhost" },

                    IssuerSigningKey = new RsaSecurityKey(RsaTools.GetPublicKey())
                };
            });


            builder.Services.AddOcelot(configuration);

            builder.Services.AddSwaggerForOcelot(configuration);

            var app = builder.Build();

            app.UseSwagger();

            app.UseSwaggerForOcelotUI(opts =>
            {
                opts.PathToSwaggerGenerator = "/swagger/docs";

            }).UseOcelot().Wait();

            app.Run();
        }
    }
}
