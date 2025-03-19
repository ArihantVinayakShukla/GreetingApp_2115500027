using BusinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ModelLayer.DTO;
using ModelLayer.Model;
using RepositoryLayer.Entity;
using System;

namespace HelloGreetingApplication.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserBL _userBL;
        private readonly ILogger<UserController> _logger;

        // Constructor with logger
        public UserController(IUserBL userBL, ILogger<UserController> logger)
        {
            _userBL = userBL;
            _logger = logger;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="registerDTO">User registration details</param>
        /// <returns>returns the registered user details</returns>
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDTO registerDTO)
        {
            try
            {
                _logger.LogInformation("POST request received to register user with email: {Email}", registerDTO.Email);

                var result = _userBL.Register(registerDTO);

                var response = new ResponseModel<object>
                {
                    Success = result != null,
                    Message = result != null ? "User registered successfully" : "Registration failed",
                    Data = result
                };

                if (result != null)
                {
                    _logger.LogInformation("User with email {Email} registered successfully", registerDTO.Email);
                }
                else
                {
                    _logger.LogWarning("User registration failed for email: {Email}", registerDTO.Email);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error during user registration for email {Email}: {ErrorMessage}", registerDTO.Email, ex.Message);
                return BadRequest(new ResponseModel<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        /// <summary>
        /// Logs in the user
        /// </summary>
        /// <param name="loginDTO">User login credentials</param>
        /// <returns>returns the logged in user details</returns>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDTO loginDTO)
        {
            try
            {
                _logger.LogInformation("POST request received for login with email: {Email}", loginDTO.Email);

                var result = _userBL.Login(loginDTO);

                if (result == null)
                {
                    _logger.LogWarning("Login failed for email: {Email} - Invalid credentials", loginDTO.Email);
                    return Unauthorized(new ResponseModel<object>
                    {
                        Success = false,
                        Message = "Invalid email or password",
                        Data = null
                    });
                }

                _logger.LogInformation("User with email {Email} logged in successfully", loginDTO.Email);

                var response = new ResponseModel<object>
                {
                    Success = true,
                    Message = "Login successful",
                    Data = result
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error during login attempt for email {Email}: {ErrorMessage}", loginDTO.Email, ex.Message);
                return BadRequest(new ResponseModel<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        /// <summary>
        /// Requests a password reset by sending a reset email to the provided email address.
        /// </summary>
        /// <param name="email">user email</param>
        /// <returns>a password reset email </returns>
        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword([FromBody] string email)
        {
            try
            {
                _logger.LogInformation("POST request received for forgot-password with email: {Email}", email);

                var result = _userBL.ForgotPassword(email);

                var response = new ResponseModel<object>
                {
                    Success = result,
                    Message = result ? "Password reset email sent" : "Email not found",
                    Data = null
                };

                if (!result)
                {
                    _logger.LogWarning("Password reset failed for email: {Email} - Email not found", email);
                    return NotFound(response);
                }

                _logger.LogInformation("Password reset email sent for email: {Email}", email);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error during forgot-password request for email {Email}: {ErrorMessage}", email, ex.Message);
                return BadRequest(new ResponseModel<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        /// <summary>
        /// Resets the user's password
        /// </summary>
        /// <param name="resetPasswordDTO">email and newPassword</param>
        /// <returns>Success Message if successful</returns>
        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordDTO resetPasswordDTO)
        {
            try
            {
                _logger.LogInformation("POST request received for reset-password with email: {Email}", resetPasswordDTO.Email);

                var result = _userBL.ResetPassword(resetPasswordDTO.Email, resetPasswordDTO.NewPassword);

                var response = new ResponseModel<object>
                {
                    Success = result,
                    Message = result ? "Password reset successfully" : "User not found",
                    Data = null
                };

                if (!result)
                {
                    _logger.LogWarning("Password reset failed for email: {Email} - User not found", resetPasswordDTO.Email);
                    return NotFound(response);
                }

                _logger.LogInformation("Password reset successfully for email: {Email}", resetPasswordDTO.Email);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error during password reset for email {Email}: {ErrorMessage}", resetPasswordDTO.Email, ex.Message);
                return BadRequest(new ResponseModel<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }
    }
}
