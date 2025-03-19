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
        UserDTO Register(RegisterDTO registerDTO);
        UserDTO Login(LoginDTO loginDTO);
        bool ForgotPassword(string email);
        bool ResetPassword(string email, string newPassword);
    }
}
