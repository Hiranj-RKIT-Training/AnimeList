using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Web;

namespace AnimeList.Helper
{
    /// <summary>
    /// Helper class to generate JWT tokens.
    /// </summary>
    public class JWTHelper
    {
        /// <summary>
        /// Generates a JWT token for the given email and role.
        /// </summary>
        /// <param name="email">User's email</param>
        /// <param name="role">User's role (e.g., Admin, User)</param>
        /// <returns>JWT token as a string</returns>
        public static string GenerateToken(string email, string role)
        {
            // Retrieve the secret key from the application configuration (in AppSettings)
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(ConfigurationSettings.AppSettings["JwtKey"]));

            // Create signing credentials using the symmetric key and the desired algorithm (AES128 with CBC mode and HMACSHA256)
            var creds = new SigningCredentials(key, SecurityAlgorithms.Aes128CbcHmacSha256);

            // Create the claims to embed in the JWT token
            List<Claim> jwtClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, email), // Email claim
                new Claim(ClaimTypes.Role, role),   // Role claim
            };

            // Create the JWT token with the specified claims, expiration, and signing credentials
            JwtSecurityToken token = new JwtSecurityToken(
                claims: jwtClaims,                       // Claims to embed
                expires: DateTime.Now.AddDays(1),        // Expiration date (1 day from now)
                signingCredentials: creds               // Signing credentials
            );

            // Return the generated JWT token as a string
            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
            return jwtToken;
        }
    }
}
