using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;

namespace APIoauth;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAuthorization(options =>
        {
            //options.AddPolicy("Admin", policy => policy.RequireClaim("role", "admin"));
        });

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
             .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
             {
                 //options.Authority = "https://login.microsoftonline.com/f16d2553-936a-4c74-b725-065c573983d4/v2.0";
                 //options.ClaimsIssuer = "https://login.microsoftonline.com/f16d2553-936a-4c74-b725-065c573983d4/v2.0";
                 options.MetadataAddress = "https://login.microsoftonline.com/f16d2553-936a-4c74-b725-065c573983d4/v2.0/.well-known/openid-configuration";
                 options.Audience = "api://e1fc76f2-b9ef-4778-8921-1673309ede90";

                 //options.RequireHttpsMetadata = false;
                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     //ValidateAudience = false,
                     ValidateIssuer = false,

                     ClockSkew = TimeSpan.Zero,
                     LifetimeValidator = (before, expires, token, param) =>
                     {
                         return true;
                     }
                 };
             });

        //builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        //    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));


        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthentication();
        app.UseAuthorization();


        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        app.MapGet("/weatherforecast", (HttpContext httpContext) =>
        {
            var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = summaries[Random.Shared.Next(summaries.Length)]
                })
                .ToArray();
            return forecast;
        })
        .WithName("GetWeatherForecast").RequireAuthorization();

        app.Run();
    }

}
