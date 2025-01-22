using AnimeList.DatabaseLogic;
using AnimeList.Helper;
using AnimeList.Models.dto;
using AnimeList.Models;
using System;
using System.Diagnostics;
using System.Web.Http;

namespace AnimeList.Controllers
{
    /// <summary>
    /// Controller to manage anime lists and their operations.
    /// </summary>
    [RoutePrefix("ListAnimes")]
    public class ListAnimesController : ApiController {

        private DLANL01 _objDLANL01;

        /// <summary>
        /// Initializes a new instanc
        /// e of the ListAnimesController class.
        /// </summary>
        public ListAnimesController()
        {
            _objDLANL01 = new DLANL01();
        }

        /// <summary>
        /// Retrieves all animes in a specific list.
        /// </summary>
        /// <param name="id">The ID of the anime list.</param>
        /// <returns>A response containing the list of animes.</returns>
        [HttpGet, Route("GetListAnimes/{id:int}"), JwtAuth]
        public IHttpActionResult GetAlLAnimeInList(int id)
        {
            Response res = _objDLANL01.GetListAnimes(id);
            if (res.IsError)
            {
                return BadRequest(res.message);
            }
            return Ok(res);
        }

        /// <summary>
        /// Adds an anime to a list.
        /// </summary>
        /// <param name="objDTOANL01">The DTO containing the anime and list details.</param>
        /// <returns>A response indicating the result of the operation.</returns>
        [HttpPost, Route("AddAnimeToList"), JwtAuth]
        public IHttpActionResult CreateList([FromBody] DTOANL01 objDTOANL01)
        {
            _objDLANL01.Type = enmType.Add;
            _objDLANL01.preSave(objDTOANL01);
            Response res = _objDLANL01.Validate();
            if (res.IsError)
            {
                return BadRequest(res.message);
            }
            res = _objDLANL01.Save();
            return Ok(res);
        }

        /// <summary>
        /// Updates the status of an anime in a list.
        /// </summary>
        /// <param name="objDTOANL01">The DTO containing the updated status and anime details.</param>
        /// <returns>A response indicating the result of the operation.</returns>
        [HttpPut, Route("UpdateStatus"), JwtAuth]
        public IHttpActionResult UpdateStatus([FromBody] DTOANL01 objDTOANL01)
        {
            _objDLANL01.Type = enmType.Edit;
            _objDLANL01.preSave(objDTOANL01);
            Response res = _objDLANL01.Validate();
            if (res.IsError)
            {
                return BadRequest(res.message);
            }
            res = _objDLANL01.Save();
            return Ok(res);
        }

        /// <summary>
        /// Deletes an anime from a specific list.
        /// </summary>
        /// <param name="listId">The ID of the list from which the anime will be removed.</param>
        /// <param name="aniId">The ID of the anime to be removed.</param>
        /// <returns>A response indicating the result of the operation.</returns>
        [HttpDelete, Route("DeleteAnimeFromList"), JwtAuth]
        public IHttpActionResult DeleteList([FromUri] int listId, [FromUri] int aniId)
        {
            _objDLANL01.Type = enmType.Delete;
            _objDLANL01.preDelete(listId, aniId);
            Response res = _objDLANL01.Validate();
            Debug.WriteLine(res.IsError);
            if (res.IsError)
            {
                return BadRequest(res.message);
            }
            res = _objDLANL01.Save();
            return Ok(res);
        }
    }
}
