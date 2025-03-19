using System;
using System.IO;
using System.Reflection;
using BusinessLayer.Interface;
using BusinessLayer.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;
using RepositoryLayer.Context;
using RepositoryLayer.Interface;
using RepositoryLayer.Middleware;
using RepositoryLayer.Service;

var logger = LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();
logger.Info("Starting the application...");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    builder.Services.AddScoped<IGreetingRL, GreetingRL>();
    builder.Services.AddScoped<IGreetingBL, GreetingBL>();
    builder.Services.AddControllers();

    builder.Services.AddEndpointsApiExplorer();

    // Configure Swagger to include XML documentation
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    builder.Services.AddSwaggerGen(options =>
    {
        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath);
        }
        else
        {
            Console.WriteLine($"? Warning: XML documentation file not found at {xmlPath}");
        }

        // Add security definition for future JWT auth (optional)
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                new string[] {}
            }
        });
    });

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<GreetingDbContext>(options => options.UseSqlServer(connectionString));

    var app = builder.Build();

    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseMiddleware<GlobalExceptionMiddleware>();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex, "Application stopped due to an exception.");
    throw;
}
finally
{
    LogManager.Shutdown();
}
