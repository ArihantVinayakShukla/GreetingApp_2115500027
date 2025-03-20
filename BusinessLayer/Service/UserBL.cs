using BusinessLayer.Interface;
using ModelLayer.DTO;
using RepositoryLayer.Interface;
using System;
using System.Threading.Tasks;

namespace BusinessLayer.Service
{
    public class UserBL : IUserBL
    {
        private readonly IUserRL _userRL;

        public UserBL(IUserRL userRL)
        {
            _userRL = userRL;
        }

        public async Task<UserDTO> Register(RegisterDTO registerDTO)
        {
            try
            {
                var userDTO = await _userRL.Register(registerDTO);

                if (userDTO == null)
                {
                    throw new Exception("Registration failed.");
                }

                return userDTO;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in registration process: " + ex.Message);
            }
        }

        public async Task<string> Login(LoginDTO loginDTO)
        {
            try
            {
                var token = await _userRL.Login(loginDTO);

                if (string.IsNullOrEmpty(token))
                {
                    throw new Exception("Invalid email or password.");
                }

                return token;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in login process: " + ex.Message);
            }
        }

        public async Task<bool> ForgotPassword(string email)
        {
            try
            {
                return await _userRL.ForgotPassword(email);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in forgot password process: " + ex.Message);
            }
        }

        public async Task<bool> ResetPassword(string token, string newPassword)
        {
            try
            {
                return await _userRL.ResetPassword(token, newPassword);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in resetting password: " + ex.Message);
            }
        }
    }
}
