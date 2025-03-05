using BusinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ModelLayer.Model;
using RepositoryLayer.Entity;
using System.Collections.Generic;

namespace HelloGreetingApplication.Controllers
{
    /// <summary>
    /// Class providing API for hitting endpoints
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class HelloGreetingController : ControllerBase
    {
        private readonly ILogger<HelloGreetingController> _logger;
        private static Dictionary<string, string> keyValueStore = new Dictionary<string, string>();
        private readonly IGreetingBL _greetingBL;

        public HelloGreetingController(ILogger<HelloGreetingController> logger, IGreetingBL greetingBL)
        {
            _logger = logger;
            _greetingBL = greetingBL;
        }

        /// <summary>
        /// Get method to get the greeting message
        /// </summary>
        /// <returns>Hello to Greeting App API endpoint Hit</returns>
        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInformation("GET request received at /hellogreeting");

            ResponseModel<string> responseModel = new ResponseModel<string>
            {
                Message = "Hello to Greeting App API endpoint Hit",
                Success = true,
                Data = "Hello, World!"
            };

            return Ok(responseModel);
        }

        /// <summary>
        /// Add method to add the new key-value pair
        /// </summary>
        /// <param name="requestModel">Key-value pair</param>
        /// <returns>Request received successfully and stored</returns>
        [HttpPost]
        public IActionResult Post(RequestModel requestModel)
        {
            _logger.LogInformation($"POST request received: Key = {requestModel.key}, Value = {requestModel.value}");

            keyValueStore[requestModel.key] = requestModel.value;

            return Ok(new ResponseModel<string>
            {
                Success = true,
                Message = "Request received successfully and stored",
                Data = $"Key: {requestModel.key}, Value: {requestModel.value}"
            });
        }

        /// <summary>
        /// Update and existing key-value pair
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns>Updated Successfully</returns>
        [HttpPut]
        public IActionResult Put(RequestModel requestModel)
        {
            if (keyValueStore.ContainsKey(requestModel.key))
            {
                _logger.LogInformation($"PUT request updating key {requestModel.key}");

                keyValueStore[requestModel.key] = requestModel.value;

                return Ok(new ResponseModel<string>
                {
                    Success = true,
                    Message = "Updated Successfully",
                    Data = $"Key: {requestModel.key}, New Value: {requestModel.value}"
                });
            }

            _logger.LogWarning($"PUT request failed: Key {requestModel.key} not found");
            return NotFound(new ResponseModel<string>
            {
                Success = false,
                Message = "Key not found"
            });
        }

        /// <summary>
        /// Modify an existing key-value pair if exists
        /// </summary>
        /// <param name="requestModel">key-value pair</param>
        /// <returns>value patched successfully</returns>
        [HttpPatch]
        public IActionResult Patch(RequestModel requestModel)
        {
            if (keyValueStore.ContainsKey(requestModel.key))
            {
                _logger.LogInformation($"PATCH request modifying key {requestModel.key}");

                keyValueStore[requestModel.key] = requestModel.value;

                return Ok(new ResponseModel<string>
                {
                    Success = true,
                    Message = "Value patched successfully",
                    Data = $"Key: {requestModel.key}, Updated Value: {requestModel.value}"
                });
            }

            _logger.LogWarning($"PATCH request failed: Key {requestModel.key} not found");
            return NotFound(new ResponseModel<string>
            {
                Success = false,
                Message = "Key not found"
            });
        }

        /// <summary>
        /// Delete and existing key-value pair if found
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>key removed</returns>
        [HttpDelete("{key}")]
        public IActionResult Delete(string key)
        {
            if (keyValueStore.ContainsKey(key))
            {
                _logger.LogInformation($"DELETE request removing key {key}");

                keyValueStore.Remove(key);

                return Ok(new ResponseModel<string>
                {
                    Success = true,
                    Message = "Deleted successfully",
                    Data = $"Key: {key} removed"
                });
            }

            _logger.LogWarning($"DELETE request failed: Key {key} not found");
            return NotFound(new ResponseModel<string>
            {
                Success = false,
                Message = "Key not found"
            });
        }

        /// <summary>
        /// Get a greeting message using business layer
        /// </summary>
        /// <returns>Greeting Message</returns>
        [HttpGet("greet")]
        public IActionResult GetGreeting()
        {
            _logger.LogInformation("GET request received at /hellogreeting/greet");

            string greetingMessage = _greetingBL.GetGreeting();

            ResponseModel<string> responseModel = new ResponseModel<string>
            {
                Success = true,
                Message = "Greeting message retrieved successfully",
                Data = greetingMessage
            };

            return Ok(responseModel);
        }

        /// <summary>
        /// Get a personalized greeting message
        /// </summary>
        /// <param name="requestModel">Request model containing user details</param>
        /// <returns>Personalized greeting message</returns>
        [HttpPost("personalizedGreeting")]

        public IActionResult GetPersonalizedGreeting([FromBody] RequestModel requestModel)
        {
            _logger.LogInformation($"POST request received at /hellogreeting/greetPersonalized for key: {requestModel.key}");

            string message = _greetingBL.PersonalizedGreeting(requestModel);

            _logger.LogInformation($"Personalized greeting generated: {message}");

            return Ok(new ResponseModel<string>
            {
                Success = true,
                Message = "Personalized greeting retrieved successfully",
                Data = message
            });
        }

        /// <summary>
        /// save a greeting in the database
        /// </summary>
        /// <param name="message">string containing a greeting message</param>
        /// <returns>saved greeting</returns>
        [HttpPost("saveGreeting")]

        public IActionResult SaveGreeting([FromBody] string message)
        {
            _logger.LogInformation("POST request received at /hellogreeting/saveGreeting");

            if (string.IsNullOrWhiteSpace(message))
            {
                _logger.LogWarning("SaveGreeting received an empty message.");
                return BadRequest(new ResponseModel<string>
                {
                    Success = false,
                    Message = "Message cannot be empty."
                });
            }

            try
            {
                var savedGreeting = _greetingBL.SaveGreeting(message);
                _logger.LogInformation("Greeting saved successfully.");

                return Ok(new ResponseModel<GreetingEntity>
                {
                    Success = true,
                    Message = "Greeting saved successfully",
                    Data = savedGreeting
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error saving greeting: {ex.Message}");
                return StatusCode(500, new ResponseModel<string>
                {
                    Success = false,
                    Message = "An error occurred while saving the greeting."
                });
            }
        }

        /// <summary>
        /// Fetch greetings by id
        /// </summary>
        /// <param name="id">Id of the greeting message</param>
        /// <returns>greeting message with the corresponding id</returns>
        [HttpGet("{id}")]
        public IActionResult GetGreetingsById(int id)
        {
            _logger.LogInformation($"GET request received for greeting ID: {id}");

            try
            {
                var greeting = _greetingBL.GetGreetingsById(id);

                if (greeting == null)
                {
                    _logger.LogWarning($"Greeting with ID {id} not found.");
                    return NotFound(new ResponseModel<string>
                    {
                        Success = false,
                        Message = $"Greeting with ID {id} not found."
                    });
                }

                _logger.LogInformation($"Greeting with ID {id} retrieved successfully.");
                return Ok(new ResponseModel<GreetingEntity>
                {
                    Success = true,
                    Message = "Greeting retrieved successfully.",
                    Data = greeting
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving greeting with ID {id}: {ex.Message}");
                return StatusCode(500, new ResponseModel<string>
                {
                    Success = false,
                    Message = "An error occurred while retrieving the greeting."
                });
            }
        }

        /// <summary>
        /// Get all greeting from the database
        /// </summary>
        /// <returns>List of greeting messages</returns>
        [HttpGet("all")]
        public IActionResult GetAllGreetings()
        {
            _logger.LogInformation("GET request received at /hellogreeting/all to fetch all greetings");

            try
            {
                List<GreetingEntity> greetings = _greetingBL.GetAllGreetings();

                if (greetings == null || greetings.Count == 0)
                {
                    _logger.LogWarning("No greetings found in the database.");
                    return NotFound(new ResponseModel<string>
                    {
                        Success = false,
                        Message = "No greetings found."
                    });
                }

                _logger.LogInformation($"Retrieved {greetings.Count} greetings successfully.");
                return Ok(new ResponseModel<List<GreetingEntity>>
                {
                    Success = true,
                    Message = "Greetings retrieved successfully.",
                    Data = greetings
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving greetings: {ex.Message}");
                return StatusCode(500, new ResponseModel<string>
                {
                    Success = false,
                    Message = "An error occurred while retrieving greetings."
                });
            }
        }


    }
}
