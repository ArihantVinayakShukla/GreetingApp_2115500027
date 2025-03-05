using System;
using BusinessLayer.Interface;
using ModelLayer.Model;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using Microsoft.Extensions.Logging;

namespace BusinessLayer.Service
{
    public class GreetingBL : IGreetingBL
    {
        private readonly IGreetingRL _greetingRL;
        private readonly ILogger<GreetingBL> _logger;

        public GreetingBL(IGreetingRL greetingRL, ILogger<GreetingBL> logger)
        {
            _greetingRL = greetingRL;
            _logger = logger;
        }

        public string GetGreeting()
        {
            _logger.LogInformation("GetGreeting method called.");
            return "Hello World";
        }

        public string PersonalizedGreeting(RequestModel requestModel)
        {
            _logger.LogInformation("PersonalizedGreeting method called.");

            if (requestModel == null)
            {
                _logger.LogWarning("PersonalizedGreeting received a null RequestModel.");
                return "Invalid request!";
            }

            string greetingMessage = _greetingRL.PersonalizedGreeting(requestModel);
            _logger.LogInformation($"Generated personalized greeting: {greetingMessage}");

            return greetingMessage;
        }

        public GreetingEntity SaveGreeting(string message)
        {
            _logger.LogInformation("SaveGreeting method called.");

            if (string.IsNullOrWhiteSpace(message))
            {
                _logger.LogWarning("SaveGreeting received an empty or null message.");
                throw new ArgumentException("Message cannot be null or empty.");
            }

            GreetingEntity greeting = new GreetingEntity
            {
                Message = message
            };

            try
            {
                GreetingEntity savedGreeting = _greetingRL.SaveGreeting(greeting);
                _logger.LogInformation("Greeting saved successfully.");
                return savedGreeting;
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
                _logger.LogInformation($"Retrieving greeting with ID: {id} from the repository.");
                return _greetingRL.GetGreetingsById(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Business Layer while fetching greeting ID {id}: {ex.Message}");
                throw; // Rethrow the exception to let the Controller handle it
            }
        }

        public List<GreetingEntity> GetAllGreetings()
        {
            try
            {
                _logger.LogInformation("Fetching all greetings from the repository.");
                List<GreetingEntity> greetings = _greetingRL.GetAllGreetings();
                _logger.LogInformation($"Successfully retrieved {greetings.Count} greetings.");
                return greetings;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while retrieving all greetings: {ex.Message}");
                throw; // Rethrow the exception to let the Controller handle it
            }
        }


    }
}
