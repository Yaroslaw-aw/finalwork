
using ApiMailServer.Db;
using ApiMailServer.Mapping;
using ApiMailServer.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ApiMailServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigurationBuilder? config = new ConfigurationBuilder();
            config.AddJsonFile("appsettings.json");
            IConfigurationRoot? configuration = config.Build();

            string? postgresConnection = configuration.GetConnectionString("db");
            builder.Services.AddDbContext<MessageContext>(options => options.LogTo(Console.WriteLine).UseNpgsql(postgresConnection));

            builder.Services.AddScoped<IServerRepository, ServerRepository>();
            builder.Services.AddAutoMapper(typeof(MappingProfile));


            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
