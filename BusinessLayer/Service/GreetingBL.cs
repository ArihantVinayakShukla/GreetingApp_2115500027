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
    }
}
