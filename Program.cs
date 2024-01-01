
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TicketAPI.CloudStorage;

namespace TicketAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var Configuration = builder.Configuration;

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddSingleton<ICloudStorage, GoogleCloudStorage>();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var key = Encoding.ASCII.GetBytes(Configuration["Jwt:Key"]); // Your JWT secret key
            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });


            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: "LocalHostOrigins",
                                  builder =>
                                  {
                                      builder.WithOrigins("http://localhost:3000")
                                             .AllowAnyHeader()
                                             .AllowAnyMethod();
                                  });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("LocalHostOrigins");
            app.UseAuthentication();
            app.UseAuthorization();
            

            app.MapControllers();

            app.Run();
        }
    }
}
