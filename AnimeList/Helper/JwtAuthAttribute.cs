using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Security;

namespace AnimeList.Helper
{
    /// <summary>
    /// Custom authorization filter for validating JWT tokens and user roles.
    /// </summary>
    public class JwtAuthAttribute : AuthorizationFilterAttribute
    {
        // Array to hold required roles that the user must have to access the resource
        private readonly string[] _roles;

        /// <summary>
        /// Constructor that accepts roles that are allowed to access the resource.
        /// </summary>
        /// <param name="roles">Roles that are allowed to access the resource.</param>
        public JwtAuthAttribute(params string[] roles)
        {
            _roles = roles;
        }

        /// <summary>
        /// Overrides the OnAuthorization method to validate JWT token and check user roles.
        /// </summary>
        /// <param name="actionContext">The HTTP action context.</param>
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            // Get the authorization header from the request
            var authHeader = actionContext.Request.Headers.Authorization;
            Debug.WriteLine(authHeader);

            // If no Authorization header or not a Bearer token, respond with Bad Request
            if (authHeader == null || authHeader.Scheme != "Bearer")
            {
                Debug.WriteLine("null header");
                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
                return;
            }

            // Extract the token from the Authorization header
            var token = authHeader.Parameter;

            try
            {
                // Validate the token using the JwtSecurityTokenHandler
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(ConfigurationSettings.AppSettings["JwtKey"]);

                // Set the validation parameters for token validation
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };

                // Validate and parse the token into a ClaimsPrincipal object
                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);

                // If principal is not null, set the current user in HttpContext
                if (principal != null)
                {
                    HttpContext.Current.User = principal;
                }

                Debug.WriteLine(principal);

                // Extract the ClaimsIdentity and check if the user is authenticated
                var identity = principal.Identity as ClaimsIdentity;

                // If identity is null or not authenticated, respond with Unauthorized
                if (identity == null || !identity.IsAuthenticated)
                {
                    Debug.WriteLine("null Identity");
                    actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
                }

                // Get the role claim from the identity
                var roleClaim = identity.FindFirst(ClaimTypes.Role)?.Value;

                // If roles are specified, check if the user's role is valid
                if (_roles.Length > 0 && !_roles.Contains(roleClaim))
                {
                    Debug.WriteLine("Forbidden: User does not have the required role.");
                    actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden);
                    return;
                }
            }
            catch
            {
                // If an exception occurs during token validation, return Unauthorized response
                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            }
        }
    }
}
