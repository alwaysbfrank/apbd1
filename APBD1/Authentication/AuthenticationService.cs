using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using APBD1.DAL;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace APBD1.Authentication
{
    public class AuthenticationService
    {
        private readonly JwtAuthDao _dao;
        private readonly IConfiguration _config;

        public AuthenticationService(JwtAuthDao dao, IConfiguration config)
        {
            _dao = dao;
            _config = config;
        }

        public FullToken Login(string index, string password)
        {
            var user = _dao.Find(index);
            if (user.Password == null || !CheckPassword(password, user))
            {
                throw new ValidationException();
            }

            return CreateToken(user);
        }

        public void SetPassword(string user, string password)
        {
            _dao.SetPassword(password, user);
        }

        private static bool CheckPassword(string password, User user)
        {
            if (user.Password == null)
            {
                return true;
            }
            
            if (password == null)
            {
                return false;
            }

            var hasher = new PasswordHasher(password, user.Salt);
            
            return hasher.GetHashed().Equals(user.Password);
        }
        
        private static IEnumerable<Claim> GetClaims(User user)
        {
            var result = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, user.IndexNumber)
            };

            if (user.Roles == null)
            {
                throw new ValidationException();
            }
            
            if (user.Roles.Contains("admin"))
            {
                result.Add(new Claim(ClaimTypes.Role, "admin"));
            }

            if (user.Roles.Contains("student"))
            {
                result.Add(new Claim(ClaimTypes.Role, "student"));
            }

            return result;
        }

        private FullToken CreateToken(User user)
        {
            var claims = GetClaims(user);
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token =  new JwtSecurityToken(
                issuer: _config["TokenIssuer"],
                audience: "Students",
                claims: claims,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: creds);

            var refreshToken = Guid.NewGuid().ToString();
            
            _dao.SetRefreshToken(refreshToken, user.IndexNumber);

            return new FullToken
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken
            };
        }

        public FullToken RefreshToken(string index, string refreshToken)
        {
            var user = _dao.Find(index);

            if (!AreRefreshTokensEqual(refreshToken, user.RefreshToken))
            {
                throw new ValidationException();
            }

            return CreateToken(user);
        }

        private static bool AreRefreshTokensEqual(string one, string other)
        {
            return one != null
                   && other != null
                   && one.Equals(other);
        }
    }
}