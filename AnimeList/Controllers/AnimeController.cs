using AnimeList.DatabaseLogic;
using AnimeList.Helper;
using AnimeList.Models;
using AnimeList.Models.dto;
using OfficeOpenXml;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Web.Http;
using System.Web;

namespace AnimeList.Controllers
{
    /// <summary>
    /// Controller to handle Anime-related API operations.
    /// </summary>
    [RoutePrefix("api/Anime")]
    public class AnimeController : ApiController
    {
        private DLANI01 _objDLANI01;

        /// <summary>
        /// Initializes a new instance of the AnimeController class.
        /// </summary>
        public AnimeController()
        {
            _objDLANI01 = new DLANI01();
        }

        /// <summary>
        /// Retrieves a list of all animes.
        /// </summary>
        /// <returns>A response containing a list of animes.</returns>
        [HttpGet, Route("GetAllAnime"), JwtAuth]
        public IHttpActionResult GetAllAnime()
        {
            Response res = _objDLANI01.GetAllAnime();
            if (res.IsError)
            {
                return BadRequest(res.message);
            }
            return Ok(res);
        }
        /// <summary>
        /// Retrieves a anime By id.
        /// </summary>
        /// <returns>A response containing a anime.</returns>
        [HttpGet, Route("GetAnimeByID/{id:int}"), JwtAuth]
        public IHttpActionResult GetAnimeByID(int id)
        {
            Response res = _objDLANI01.GetAnimeByID(id);
            if (res.IsError)
            {
                return BadRequest(res.message);
            }
            return Ok(res);
        }
        /// <summary>
        /// Generates the excel sheet of animes
        /// </summary>
        /// <returns>A response containing a Excel sheet.</returns>
        [HttpGet, Route("GetAnimeSheet")]
        public HttpResponseMessage GetAnimeSheet()
        {
            Response res = _objDLANI01.GetAnimeSheet();
            if (res.IsError)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, res.message);
            }
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Animes");
                worksheet.Cells["A1"].LoadFromDataTable(res.data, true);
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                string filePath = Path.Combine(Path.GetTempPath(), "Animes.xlsx");

                // Save the Excel file to disk
                var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                
                    package.SaveAs(fileStream);


                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StreamContent(fileStream)
                };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = "Animes.xlsx"
                };
                //File.Delete(filePath);

                return response;

                //using (var memoryStream = new MemoryStream())
                //{
                //    package.SaveAs(memoryStream);
                //    memoryStream.Position = 0;

                //    // Create the HTTP response with the Excel file
                //    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
                //    {
                //        Content = new ByteArrayContent(memoryStream.ToArray())
                //    };
                //    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                //    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                //    {
                //        FileName = "Animes.xlsx"
                //    };
                //    return response;
                //}
            }
            
        }
        /// <summary>
        /// Searches for animes based on the provided prefix.
        /// </summary>
        /// <param name="prefix">The prefix to search for.</param>
        /// <returns>A response containing a list of matching animes.</returns>
        [HttpGet, Route("SearchAnime/{prefix:alpha}"), JwtAuth]
        public IHttpActionResult SearchAnime(string prefix)
        {
            Response res = _objDLANI01.SearchAnime(prefix);
            if (res.IsError)
            {
                return BadRequest(res.message);
            }
            return Ok(res);
        }

        /// <summary>
        /// Adds a new anime to the database.
        /// </summary>
        /// <param name="objDTOANI01">The anime data transfer object containing anime details.</param>
        /// <returns>A response indicating the result of the operation.</returns>
        [HttpPost, Route("AddAnime"), JwtAuth("admin")]
        public IHttpActionResult AddAnime([FromBody] DTOANI01 objDTOANI01)
        {
            _objDLANI01.Type = enmType.Add;
            _objDLANI01.preSave(objDTOANI01);
            Response res = _objDLANI01.Validate();
            if (res.IsError)
            {
                return BadRequest(res.message);
            }
            res = _objDLANI01.Save();
            return Ok(res);
        }

        /// <summary>
        /// Updates an existing anime in the database.
        /// </summary>
        /// <param name="objDTOANI01">The anime data transfer object containing updated anime details.</param>
        /// <returns>A response indicating the result of the operation.</returns>
        [HttpPut, Route("UpdateAnime"), JwtAuth("admin")]
        public IHttpActionResult UpdateAnime([FromBody] DTOANI01 objDTOANI01)
        {
            _objDLANI01.Type = enmType.Edit;
            _objDLANI01.preSave(objDTOANI01);
            Response res = _objDLANI01.Validate();
            if (res.IsError)
            {
                return BadRequest(res.message);
            }
            res = _objDLANI01.Save();
            return Ok(res);
        }

        /// <summary>
        /// Deletes an anime from the database by ID.
        /// </summary>
        /// <param name="id">The ID of the anime to delete.</param>
        /// <returns>A response indicating the result of the operation.</returns>
        [HttpDelete, Route("RemoveAnime/{id:int}"), JwtAuth("admin")]
        public IHttpActionResult DeleteAnime([FromUri] int id)
        {
            _objDLANI01.Type = enmType.Delete;
            _objDLANI01.preDelete(id);
            Response res = _objDLANI01.Validate();
            if (res.IsError)
            {
                return BadRequest(res.message);
            }
            res = _objDLANI01.Save();
            return Ok(res);
        }
    }
}
