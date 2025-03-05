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

        public GreetingEntity GetGreetingsById(int id)
        {
            try
            {
                _logger.LogInformation($"Fetching greeting with ID: {id}");
                var greeting = _context.Greetings.FirstOrDefault(g => g.Id == id);

                if (greeting == null)
                {
                    _logger.LogWarning($"Greeting with ID {id} not found.");
                }

                return greeting;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while fetching greeting with ID {id}: {ex.Message}");
                throw;
            }
        }

        public List<GreetingEntity> GetAllGreetings()
        {
            try
            {
                _logger.LogInformation("Fetching all greetings from the database.");
                var greetings = _context.Greetings.ToList();

                if (greetings == null || greetings.Count == 0)
                {
                    _logger.LogWarning("No greetings found in the database.");
                }
                else
                {
                    _logger.LogInformation($"Retrieved {greetings.Count} greetings from the database.");
                }

                return greetings;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while fetching all greetings: {ex.Message}");
                throw;
            }
        }

        public GreetingEntity EditGreetings(int id, string message)
        {
            try
            {
                _logger.LogInformation($"Attempting to edit greeting with ID: {id}");

                var greeting = _context.Greetings.FirstOrDefault(g => g.Id == id);
                if (greeting != null)
                {
                    greeting.Message = message;
                    _context.SaveChanges();
                    _logger.LogInformation($"Greeting with ID: {id} updated successfully.");
                }
                else
                {
                    _logger.LogWarning($"Greeting with ID: {id} not found.");
                }

                return greeting;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while editing greeting with ID {id}: {ex.Message}");
                throw;
            }
        }

        public bool DeleteGreetingMessage(int id)
        {
            try
            {
                _logger.LogInformation($"Attempting to delete greeting with ID: {id}");
                var greeting = _context.Greetings.FirstOrDefault(g => g.Id == id);

                if (greeting != null)
                {
                    _context.Greetings.Remove(greeting);
                    _context.SaveChanges();
                    _logger.LogInformation($"Greeting with ID {id} deleted successfully.");
                    return true;
                }

                _logger.LogWarning($"Greeting with ID {id} not found.");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while deleting greeting with ID {id}: {ex.Message}");
                throw;
            }
        }

    }
}
