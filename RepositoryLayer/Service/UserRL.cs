using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Hashing;
using RepositoryLayer.Interface;
using ModelLayer.DTO;
using RepositoryLayer.Helper;
using Microsoft.Extensions.Configuration;

namespace RepositoryLayer.Service
{
    public class UserRL : IUserRL
    {
        private readonly GreetingDbContext _context;
        private readonly Password_Hash _passwordHash;
        private readonly JwtHelper _jwtHelper;
        private readonly EmailService _emailService;
        private readonly ResetTokenHelper _resetTokenHelper;
        private readonly IConfiguration _configuration;

        public UserRL(GreetingDbContext context, JwtHelper jwtHelper, EmailService emailService, ResetTokenHelper resetTokenHelper, IConfiguration configuration)
        {
            _context = context;
            _passwordHash = new Password_Hash();
            _jwtHelper = jwtHelper;
            _emailService = emailService;
            _resetTokenHelper = resetTokenHelper;
            _configuration = configuration;
        }

        public UserDTO Register(RegisterDTO registerDTO)
        {
            string hashedPassword = _passwordHash.HashPassword(registerDTO.Password);

            var userEntity = new UserEntity
            {
                FirstName = registerDTO.FirstName,
                LastName = registerDTO.LastName,
                Email = registerDTO.Email,
                PasswordHash = hashedPassword
            };

            _context.Users.Add(userEntity);
            _context.SaveChanges();

            return new UserDTO
            {
                FirstName = userEntity.FirstName,
                LastName = userEntity.LastName,
                Email = userEntity.Email,
            };
        }

        public string Login(LoginDTO loginDTO)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == loginDTO.Email);
            if (user == null || !_passwordHash.VerifyPassword(loginDTO.Password, user.PasswordHash))
            {
                return null; 
            }

            string token = _jwtHelper.GenerateToken(user.UserId, user.Email);

            return token;
        }

        public bool ForgotPassword(string email)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if(user == null)
            {
                return false;
            }

            string token = _resetTokenHelper.GeneratePasswordResetToken(user.UserId, user.Email);

            string baseUrl = _configuration["Application:BaseUrl"];

            bool emailSent = _emailService.SendPasswordResetEmail(email, token, baseUrl);

            return emailSent;
        }

        public bool ResetPassword(string token, string newPassword)
        {
            if (!_resetTokenHelper.ValidatePasswordResetToken(token, out string email))
            {
                return false;
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                return false;
            }

            var passwordHasher = new Password_Hash();
            string hashedPassword = passwordHasher.HashPassword(newPassword);

            user.PasswordHash = hashedPassword;

            _context.SaveChanges();
            return true;
        }
    }
}
