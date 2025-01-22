using AnimeList.DatabaseLogic;
using AnimeList.Helper;
using AnimeList.Models;
using AnimeList.Models.dto;
using System.Diagnostics;
using System.Web.Http;

namespace AnimeList.Controllers
{
    /// <summary>
    /// Controller to handle operations related to user-created anime lists.
    /// </summary>
    [RoutePrefix("api/AnimeLists")]
    public class ListController : ApiController
    {
        private DLLST01 _objDLLST01;

        /// <summary>
        /// Initializes a new instance of the ListController class.
        /// </summary>
        ListController()
        {
            _objDLLST01 = new DLLST01();
        }

        /// <summary>
        /// Creates a new anime list for a user.
        /// </summary>
        /// <param name="objDTOLST01">The DTO containing details of the anime list.</param>
        /// <returns>A response indicating the result of the operation.</returns>
        [HttpPost, Route("CreateList"), JwtAuth]
        public IHttpActionResult CreateList([FromBody] DTOLST01 objDTOLST01)
        {
            _objDLLST01.Type = enmType.Add;
            _objDLLST01.preSave(objDTOLST01);
            Response res = _objDLLST01.Validate();
            if (res.IsError)
            {
                return BadRequest(res.message);
            }
            res = _objDLLST01.Save();
            return Ok(res);
        }

        /// <summary>
        /// Retrieves all lists created by a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A response containing the user's anime lists.</returns>
        [HttpGet, Route("GetUserList"), JwtAuth]
        public IHttpActionResult GetUserList([FromUri] int userId)
        {
            Response res = _objDLLST01.GetUserList(userId);
            if (res.IsError)
            {
                return BadRequest(res.message);
            }
            return Ok(res);
        }

        /// <summary>
        /// Deletes an anime list by its ID.
        /// </summary>
        /// <param name="id">The ID of the list to be deleted.</param>
        /// <returns>A response indicating the result of the operation.</returns>
        [HttpDelete, Route("DeleteList/{id:int}"), JwtAuth]
        public IHttpActionResult DeleteList([FromUri] int id)
        {
            _objDLLST01.Type = enmType.Delete;
            _objDLLST01.preDelete(id);
            Response res = _objDLLST01.Validate();
            Debug.WriteLine(res.IsError);
            if (res.IsError)
            {
                return BadRequest(res.message);
            }
            res = _objDLLST01.Save();
            return Ok(res);
        }
    }
}
