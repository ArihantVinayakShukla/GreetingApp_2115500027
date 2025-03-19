using BusinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;
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

        public UserController(IUserBL userBL)
        {
            _userBL = userBL;
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
                var result = _userBL.Register(registerDTO);

                var response = new ResponseModel<object>
                {
                    Success = result != null,
                    Message = result != null ? "User registered successfully" : "Registration failed",
                    Data = result
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
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
                var result = _userBL.Login(loginDTO);

                if (result == null)
                {
                    return Unauthorized(new ResponseModel<object>
                    {
                        Success = false,
                        Message = "Invalid email or password",
                        Data = null
                    });
                }

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
                var result = _userBL.ForgotPassword(email);

                var response = new ResponseModel<object>
                {
                    Success = result,
                    Message = result ? "Password reset email sent" : "Email not found",
                    Data = null
                };

                if (!result)
                    return NotFound(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
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
        /// <returns>Success Message if succesfull</returns>
        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordDTO resetPasswordDTO)
        {
            try
            {
                var result = _userBL.ResetPassword(resetPasswordDTO.Email, resetPasswordDTO.NewPassword);

                var response = new ResponseModel<object>
                {
                    Success = result,
                    Message = result ? "Password reset successfully" : "User not found",
                    Data = null
                };

                if (!result)
                    return NotFound(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
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
