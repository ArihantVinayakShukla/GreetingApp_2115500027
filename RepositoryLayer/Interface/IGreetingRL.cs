using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Model;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Interface
{
    public interface IGreetingRL
    {
        string PersonalizedGreeting(RequestModel request);

        GreetingEntity SaveGreeting(int userId, string message);

        GreetingEntity GetGreetingsById(int userId, int id);

        List<GreetingEntity> GetAllGreetings(int userId);

        GreetingEntity EditGreetings(int userId, int id, string message);

        bool DeleteGreetingMessage(int userId, int id);
    }
}
