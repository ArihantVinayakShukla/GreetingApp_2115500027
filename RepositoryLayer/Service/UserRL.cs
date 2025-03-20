using System;
using System.Linq;
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
        private readonly RedisCacheHelper _redisCacheHelper;

        public UserRL(GreetingDbContext context, JwtHelper jwtHelper, EmailService emailService,
                      ResetTokenHelper resetTokenHelper, IConfiguration configuration,
                      RedisCacheHelper redisCacheHelper)
        {
            _context = context;
            _passwordHash = new Password_Hash();
            _jwtHelper = jwtHelper;
            _emailService = emailService;
            _resetTokenHelper = resetTokenHelper;
            _configuration = configuration;
            _redisCacheHelper = redisCacheHelper;
        }

        public async Task<UserDTO> Register(RegisterDTO registerDTO)
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
            await _context.SaveChangesAsync();

            var userDTO = new UserDTO
            {
                FirstName = userEntity.FirstName,
                LastName = userEntity.LastName,
                Email = userEntity.Email,
            };

            await _redisCacheHelper.SetCacheAsync($"user:{userEntity.Email}", userDTO);

            return userDTO;
        }

        public async Task<string> Login(LoginDTO loginDTO)
        {
            var cachedUser = await _redisCacheHelper.GetCacheAsync<UserDTO>($"user:{loginDTO.Email}");
            UserEntity user;

            if (cachedUser != null)
            {
                user = _context.Users.FirstOrDefault(u => u.Email == cachedUser.Email);
            }
            else
            {
                user = _context.Users.FirstOrDefault(u => u.Email == loginDTO.Email);

                if (user != null)
                {
                    await _redisCacheHelper.SetCacheAsync($"user:{user.Email}", new UserDTO
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email
                    });
                }
            }

            if (user == null || !_passwordHash.VerifyPassword(loginDTO.Password, user.PasswordHash))
            {
                return null;
            }

            return _jwtHelper.GenerateToken(user.UserId, user.Email);
        }

        public async Task<bool> ForgotPassword(string email)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                return false;
            }

            string token = _resetTokenHelper.GeneratePasswordResetToken(user.UserId, user.Email);
            string baseUrl = _configuration["Application:BaseUrl"];

            bool emailSent = _emailService.SendPasswordResetEmail(email, token, baseUrl);

            if (emailSent)
            {
                await _redisCacheHelper.SetCacheAsync($"resetToken:{email}", token);
            }

            return emailSent;
        }

        public async Task<bool> ResetPassword(string token, string newPassword)
        {
            string cachedToken = await _redisCacheHelper.GetCacheAsync<string>($"resetToken:{token}");

            if (cachedToken == null || !_resetTokenHelper.ValidatePasswordResetToken(token, out string email))
            {
                return false;
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                return false;
            }

            string hashedPassword = _passwordHash.HashPassword(newPassword);
            user.PasswordHash = hashedPassword;

            await _context.SaveChangesAsync();

            await _redisCacheHelper.RemoveCacheAsync($"resetToken:{token}");

            return true;
        }
    }
}
