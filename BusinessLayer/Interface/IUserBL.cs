using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.DTO;
using RepositoryLayer.Entity;

namespace BusinessLayer.Interface
{
    public interface IUserBL
    {
        Task<UserDTO> Register(RegisterDTO registerDTO);
        Task<string> Login(LoginDTO loginDTO);
        Task<bool> ForgotPassword(string email);
        Task<bool> ResetPassword(string email, string newPassword);
    }
}
