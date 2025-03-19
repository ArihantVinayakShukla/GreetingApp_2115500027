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

        public GreetingEntity SaveGreeting(int userId, string message)
        {
            try
            {
                _logger.LogInformation($"Saving greeting for User ID: {userId}");

                var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
                if (user == null)
                {
                    _logger.LogWarning($"User with ID {userId} not found.");
                    return null;
                }

                var greeting = new GreetingEntity
                {
                    UserId = userId,
                    Message = message,
                };

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

        public GreetingEntity GetGreetingsById(int userId, int id)
        {
            try
            {
                _logger.LogInformation($"Fetching greeting ID {id} for User ID {userId}");

                var greeting = _context.Greetings.FirstOrDefault(g => g.Id == id && g.UserId == userId);

                if (greeting == null)
                {
                    _logger.LogWarning($"Greeting ID {id} not found for User ID {userId}.");
                }

                return greeting;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while fetching greeting ID {id}: {ex.Message}");
                throw;
            }
        }

        public List<GreetingEntity> GetAllGreetings( int userId)
        {
            try
            {
                _logger.LogInformation($"Fetching all greetings for User ID {userId}");

                var greetings = _context.Greetings.Where(g => g.UserId == userId).ToList();

                if (greetings.Count == 0)
                {
                    _logger.LogWarning("No greetings found for this user.");
                }

                return greetings;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while fetching greetings: {ex.Message}");
                throw;
            }
        }

        public GreetingEntity EditGreetings(int userId, int id, string message)
        {
            try
            {
                _logger.LogInformation($"Editing greeting ID {id} for User ID {userId}");

                var greeting = _context.Greetings.FirstOrDefault(g => g.Id == id && g.UserId == userId);
                if (greeting != null)
                {
                    greeting.Message = message;
                    _context.SaveChanges();
                    _logger.LogInformation("Greeting updated successfully.");
                    return greeting;
                }

                _logger.LogWarning($"Greeting ID {id} not found for User ID {userId}.");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while editing greeting ID {id}: {ex.Message}");
                throw;
            }
        }

        public bool DeleteGreetingMessage(int userId, int id)
        {
            try
            {
                _logger.LogInformation($"Deleting greeting ID {id} for User ID {userId}");

                var greeting = _context.Greetings.FirstOrDefault(g => g.Id == id && g.UserId == userId);
                if (greeting != null)
                {
                    _context.Greetings.Remove(greeting);
                    _context.SaveChanges();
                    _logger.LogInformation("Greeting deleted successfully.");
                    return true;
                }

                _logger.LogWarning($"Greeting ID {id} not found for User ID {userId}.");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while deleting greeting ID {id}: {ex.Message}");
                throw;
            }
        }

    }
}
