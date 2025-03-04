using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Model;
using RepositoryLayer.Interface;

namespace RepositoryLayer.Service
{
    public class GreetingRL : IGreetingRL
    {
        public string PersonalizedGreeting(RequestModel request)
        {
            if (!string.IsNullOrWhiteSpace(request.FirstName) && !string.IsNullOrWhiteSpace(request.LastName))
                return "Hello, " + request.FirstName + " " + request.LastName + "!";

            if (!string.IsNullOrWhiteSpace(request.FirstName))
                return "Hello, " + request.FirstName + "!";

            if (!string.IsNullOrWhiteSpace(request.LastName))
                return "Hello, " + request.LastName + "!";

            return "Hello, World!";

        }
    }
}
