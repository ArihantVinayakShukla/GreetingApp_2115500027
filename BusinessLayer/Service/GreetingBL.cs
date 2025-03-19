using System;
using BusinessLayer.Interface;
using ModelLayer.Model;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using Microsoft.Extensions.Logging;
using RepositoryLayer.Service;

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

        public GreetingEntity SaveGreeting(int userId, string message)
        {
            _logger.LogInformation($"Saving greeting for User ID {userId}");

            if (string.IsNullOrWhiteSpace(message))
            {
                _logger.LogWarning("Message cannot be empty.");
                throw new ArgumentException("Message cannot be null or empty.");
            }

            return _greetingRL.SaveGreeting(userId, message);
        }

        public GreetingEntity GetGreetingsById(int userId, int id)
        {
            _logger.LogInformation($"Fetching greeting ID {id} for User ID {userId}");
            return _greetingRL.GetGreetingsById(userId, id);
        }

        public List<GreetingEntity> GetAllGreetings(int userId)
        {
            try
            {
                _logger.LogInformation($"Fetching all greetings for User ID {userId}");
                return _greetingRL.GetAllGreetings(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while retrieving all greetings: {ex.Message}");
                throw; 
            }
        }

        public GreetingEntity EditGreetings(int userId, int id, string message)
        {
            try
            {
                _logger.LogInformation($"Editing greeting ID {id} for User ID {userId}");

                if (string.IsNullOrWhiteSpace(message))
                {
                    _logger.LogWarning("Message cannot be empty.");
                    throw new ArgumentException("Message cannot be null or empty.");
                }

                return _greetingRL.EditGreetings(userId, id, message);
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
                return _greetingRL.DeleteGreetingMessage(userId, id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while deleting greeting ID {id}: {ex.Message}");
                throw;
            }
        }

    }
}
