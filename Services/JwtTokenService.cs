using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Lab4API.Models;
using Microsoft.IdentityModel.Tokens;

namespace Lab4API.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        //pulls in both our configuration and our context
        //so we can use them in our methods
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        public JwtTokenService(IConfiguration configuration, ApplicationDbContext context)
        {
            //fetches using dependency injection
            //this is how we get the configuration and context
            _configuration = configuration;
            _context = context;
        }

        //inherits from the interface, we use an interface
        //for the purpose of testing and to keep our code clean
        public string GenerateToken(User user)
        {
            //this code will generate a token for the user
            //we will use this token to authenticate the user

            //we will frist grab all suer roles so we can store this in the claims

            var userRoles = _context.UserRoles
                .Where(ur => ur.UserId == user.Id)
                .Select(ur => ur.Role.Name)
                .ToList();
                

            var claims = new List<Claim>
            {
                //these are the claims we will use to identify the user
                //the claims represent the user and their roles
                //we could also choose to use the user's email or other information
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            //add each one of the user roles to claims so they can later be used for authorisation
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            //we will use the key from the configuration file
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            //this is the algorithm we will use to sign the token, which shouldn't be reversable
            //the algorithm is HmacSha256
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                //Issuer and audience are in the configuration file
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                //claims are the claims we created above
                claims: claims,
                notBefore: DateTime.Now,
                //expires is set to 30 minutes from now
                expires: DateTime.Now.AddMinutes(30),
                //signing credentials are the key and algorithm we created above
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}