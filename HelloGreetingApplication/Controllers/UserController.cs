﻿using BusinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ModelLayer.DTO;
using ModelLayer.Model;
using System;
using System.Threading.Tasks;

namespace HelloGreetingApplication.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserBL _userBL;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserBL userBL, ILogger<UserController> logger)
        {
            _userBL = userBL;
            _logger = logger;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="registerDTO">User registration details</param>
        /// <returns>Returns the registered user details</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            try
            {
                _logger.LogInformation("POST request received to register user with email: {Email}", registerDTO.Email);

                var result = await _userBL.Register(registerDTO).ConfigureAwait(false);

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
        /// Logs in the user.
        /// </summary>
        /// <param name="loginDTO">User login credentials</param>
        /// <returns>Returns the logged-in user details</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            try
            {
                _logger.LogInformation("POST request received for login with email: {Email}", loginDTO.Email);

                var result = await _userBL.Login(loginDTO).ConfigureAwait(false);

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
        /// <param name="email">User email</param>
        /// <returns>A password reset email</returns>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            try
            {
                _logger.LogInformation("POST request received for forgot-password with email: {Email}", email);

                var result = await _userBL.ForgotPassword(email).ConfigureAwait(false);

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
        /// Resets the user's password.
        /// </summary>
        /// <param name="token">Password reset token</param>
        /// <param name="resetPasswordDTO">Email and new password</param>
        /// <returns>Success message if successful</returns>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromQuery] string token, [FromBody] ResetPasswordDTO resetPasswordDTO)
        {
            try
            {
                _logger.LogInformation("POST request received for reset-password with token: {Token}", token);

                var result = await _userBL.ResetPassword(token, resetPasswordDTO.newPassword).ConfigureAwait(false);

                var response = new ResponseModel<object>
                {
                    Success = result,
                    Message = result ? "Password reset successfully" : "User not found",
                    Data = null
                };

                if (!result)
                {
                    _logger.LogWarning("Password reset failed - Token might be invalid or user not found.");
                    return NotFound(response);
                }

                _logger.LogInformation("Password reset successfully for token: {Token}", token);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error during password reset attempt for token {Token}: {ErrorMessage}", token, ex.Message);
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
