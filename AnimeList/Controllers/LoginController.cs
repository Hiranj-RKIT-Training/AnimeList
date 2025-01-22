using AnimeList.DatabaseLogic;
using AnimeList.Helper;
using AnimeList.Models;
using AnimeList.Models.dto;
using AnimeList.Models.enums;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace AnimeList.Controllers
{
    /// <summary>
    /// Controller for handling user authentication and management operations.
    /// </summary>
    [RoutePrefix("api/login")]
    public class LoginController : ApiController
    {
        private DLUSR01 _objDLUSR01;

        /// <summary>
        /// Initializes a new instance of the LoginController class.
        /// </summary>
        public LoginController()
        {
            _objDLUSR01 = new DLUSR01();
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="objDTOUSR01">The DTO containing user details for registration.</param>
        /// <returns>A response indicating the result of the registration process.</returns>
        [HttpPost, Route("SignUp"), AllowAnonymous]
        public IHttpActionResult SignUp([FromBody] DTOUSR01 objDTOUSR01)
        {
            _objDLUSR01.Type = enmType.Add;
            _objDLUSR01.preSave(objDTOUSR01);
            Response res = _objDLUSR01.Validate();
            if (res.IsError)
            {
                return BadRequest(res.message);
            }
            res = _objDLUSR01.Save();
            return Ok(res);
        }

        /// <summary>
        /// Authenticates a user and provides login functionality.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <returns>A response indicating the result of the login process.</returns>
        [HttpGet, Route("SignIn"), AllowAnonymous]
        public IHttpActionResult SignIn(string username, string password)
        {
            Response res = _objDLUSR01.login(username, password);
            return Ok(res);
        }

        /// <summary>
        /// Updates the details of an existing user.
        /// </summary>
        /// <param name="objDTOUSR01">The DTO containing updated user details.</param>
        /// <returns>A response indicating the result of the update operation.</returns>
        [HttpPut, Route("UpdateUser"), JwtAuth]
        public IHttpActionResult UpdateUser([FromBody] DTOUSR01 objDTOUSR01)
        {
            _objDLUSR01.Type = enmType.Edit;
            _objDLUSR01.preSave(objDTOUSR01);
            Response res = _objDLUSR01.Validate();
            if (res.IsError)
            {
                return BadRequest(res.message);
            }
            res = _objDLUSR01.Save();
            return Ok(res);
        }

        /// <summary>
        /// Deletes a user by their ID.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <returns>A response indicating the result of the delete operation.</returns>
        [HttpDelete, Route("DeleteUser/{id:int}"), JwtAuth]
        public IHttpActionResult DeleteUser([FromUri] int id)
        {
            _objDLUSR01.Type = enmType.Delete;
            _objDLUSR01.preDelete(id);
            Response res = _objDLUSR01.Validate();
            if (res.IsError)
            {
                return BadRequest(res.message);
            }
            res = _objDLUSR01.Save();
            return Ok(res);
        }

        /// <summary>
        /// Validates the JWT token and retrieves user details from the claims.
        /// </summary>
        /// <returns>The username and role of the authenticated user.</returns>
        [HttpGet, Route("Check"), JwtAuth("user", "admin")]
        public IHttpActionResult checkJwt()
        {
            var user = HttpContext.Current.User as ClaimsPrincipal;

            if (user == null || !user.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            // Extract the username and role from the claims
            var username = user.FindFirst(ClaimTypes.Email)?.Value;
            var role = user.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(role))
            {
                return BadRequest("Invalid token claims.");
            }

            // Return the username and role
            return Ok(new
            {
                Username = username,
                Role = role
            });
        }
    }
}
