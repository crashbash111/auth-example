using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lab4API.Models;

namespace Lab4API.Services
{
    //we have used an interface here to keep our code clean
    //and to make it easier to test
    //we will use dependency injection to inject the interface into our controller
    //this way, we are not tightly coupled to the implementation
    //we can easily swap out the implementation for a different one where required
    public interface IJwtTokenService
    {
        //this is the method we will use to generate the token
        //it takes a user as a parameter and returns a string
        //the string is the token we will use to authenticate the user
        string GenerateToken(User user);
    }
}