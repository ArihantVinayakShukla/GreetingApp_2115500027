using System;
using ModelLayer.Model;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using Microsoft.Extensions.Logging;

namespace RepositoryLayer.Service
{
    public class GreetingRL : IGreetingRL
    {
        private readonly GreetingDbContext _context;
        private readonly ILogger<GreetingRL> _logger;

        public GreetingRL(GreetingDbContext context, ILogger<GreetingRL> logger)
        {
            _context = context;
            _logger = logger;
        }

        public string PersonalizedGreeting(RequestModel request)
        {
            _logger.LogInformation("PersonalizedGreeting method called.");

            if (!string.IsNullOrWhiteSpace(request.FirstName) && !string.IsNullOrWhiteSpace(request.LastName))
            {
                string fullGreeting = "Hello, " + request.FirstName + " " + request.LastName + "!";
                _logger.LogInformation($"Generated personalized greeting: {fullGreeting}");
                return fullGreeting;
            }

            if (!string.IsNullOrWhiteSpace(request.FirstName))
            {
                string firstNameGreeting = "Hello, " + request.FirstName + "!";
                _logger.LogInformation($"Generated first-name greeting: {firstNameGreeting}");
                return firstNameGreeting;
            }

            if (!string.IsNullOrWhiteSpace(request.LastName))
            {
                string lastNameGreeting = "Hello, " + request.LastName + "!";
                _logger.LogInformation($"Generated last-name greeting: {lastNameGreeting}");
                return lastNameGreeting;
            }

            _logger.LogWarning("No valid name provided. Returning default greeting.");
            return "Hello, World!";
        }

        public GreetingEntity SaveGreeting(GreetingEntity greeting)
        {
            try
            {
                _logger.LogInformation("Saving greeting to database.");

                _context.Greetings.Add(greeting);
                _context.SaveChanges();

                _logger.LogInformation("Greeting saved successfully.");
                return greeting;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while saving greeting: {ex.Message}");
                throw;
            }
        }
    }
}
