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

        GreetingEntity SaveGreeting(GreetingEntity greeting);

        GreetingEntity GetGreetingsById(int id);

        List<GreetingEntity> GetAllGreetings();

        GreetingEntity EditGreetings(int id, string message);

        bool DeleteGreetingMessage(int id);
    }
}
