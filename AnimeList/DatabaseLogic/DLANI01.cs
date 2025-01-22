using AnimeList.Helper;
using AnimeList.Models;
using AnimeList.Models.dto;
using AnimeList.Models.poco;
using CRUDwithDatabase.Helpers;
using ServiceStack.OrmLite;
using serviceStackDemo.Service;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;

namespace AnimeList.DatabaseLogic
{
    public class DLANI01 : IDataHandler<DTOANI01>
    {
        private Response _objResponse; // Response object to store the operation result
        private ANI01 _objANI01; // POCO object for Anime data
        private int _aniId; // Anime ID for identifying specific records

        public DLANI01()
        {
            _objResponse = new Response();
            _objANI01 = new ANI01();
        }

        // Type of the operation (Add, Edit, Delete)
        public enmType Type { get; set; }

        /// <summary>
        /// Retrieves all anime records from the database.
        /// </summary>
        /// <returns>Response containing all anime data.</returns>
        public Response GetAllAnime()
        {
            using (var db = DatabseHelper.GetConnectionFactory())
            {
                try
                {
                    // Fetch all anime records from the database
                    var results = db.Select<ANI01>();
                
                    _objResponse.IsError = false;
                    _objResponse.data = results;
                }
                catch (Exception ex)
                {
                    // Handle exceptions and return an error response
                    _objResponse.IsError = true;
                    _objResponse.message = ex.Message;
                }
            }
            return _objResponse;
        }
        public Response GetAnimeByID(int aniId)
        {
            using (var db = DatabseHelper.GetConnectionFactory())
            {
                try
                {
                    // Fetch all anime records from the database
                    List<ANI01> results = db.Select<ANI01>();
                    var objAni = from result in results
                            where result.I01F01 == aniId
                            select result;
                    _objResponse.IsError = false;
                    _objResponse.data = objAni;
                }
                catch (Exception ex)
                {
                    // Handle exceptions and return an error response
                    _objResponse.IsError = true;
                    _objResponse.message = ex.Message;
                }
            }
            return _objResponse;
        }
        public Response GetAnimeSheet()
        {
            using (var db = DatabseHelper.GetConnectionFactory())
            {
                try
                {
                    DataTable tblAniTable = new DataTable("Animes");
                    tblAniTable.Columns.Add("Anime ID");
                    tblAniTable.Columns.Add("Anime Title");
                    tblAniTable.Columns.Add("No of Seasons");
                    tblAniTable.Columns.Add("No of Episodes");
                    tblAniTable.Columns.Add("Anime Release Year");
                    // Fetch all anime records from the database
                    var results = db.Select<ANI01>();
                    foreach(ANI01 result in results)
                    {
                        DataRow dtRow = tblAniTable.NewRow();
                        dtRow["Anime ID"] = result.I01F01;
                        dtRow["Anime Title"] = result.I01F02;
                        dtRow["No of Seasons"] = result.I01F03;
                        dtRow["No of Episodes"] = result.I01F04;
                        dtRow["Anime Release Year"] = result.I01F05;
                        tblAniTable.Rows.Add(dtRow);
                    }
                    _objResponse.IsError = false;
                    _objResponse.data = tblAniTable;
                }
                catch (Exception ex)
                {
                    // Handle exceptions and return an error response
                    _objResponse.IsError = true;
                    _objResponse.message = ex.Message;
                }
            }
            return _objResponse;
        }

        /// <summary>
        /// Searches anime records based on a prefix in their name.
        /// </summary>
        /// <param name="prefix">Prefix to search for in anime names.</param>
        /// <returns>Response containing matching anime records.</returns>
        public Response SearchAnime(string prefix)
        {
            using (var db = DatabseHelper.GetConnectionFactory())
            {
                try
                {
                    // Fetch anime records that start with the given prefix
                    var results = db.Select<ANI01>(e => e.I01F02.StartsWith(prefix));
                    _objResponse.IsError = false;
                    _objResponse.data = results;
                }
                catch (Exception ex)
                {
                    // Handle exceptions and return an error response
                    _objResponse.IsError = true;
                    _objResponse.message = ex.Message;
                }
            }
            return _objResponse;
        }

        /// <summary>
        /// Adds a new anime record to the database.
        /// </summary>
        private Response AddAnime()
        {
            using (var db = DatabseHelper.GetConnectionFactory())
            {
                try
                {
                    // Insert the new anime record into the database
                    db.Insert(_objANI01);
                    _objResponse.IsError = false;
                    _objResponse.message = "Anime added successfully.";
                }
                catch (Exception ex)
                {
                    // Handle exceptions and return an error response
                    _objResponse.IsError = true;
                    _objResponse.message = ex.Message;
                }
            }
            return _objResponse;
        }

        /// <summary>
        /// Updates an existing anime record in the database.
        /// </summary>
        private Response UpdateAnime()
        {
            using (var db = DatabseHelper.GetConnectionFactory())
            {
                try
                {
                    // Fetch the current anime record by ID
                    var currentAnime = db.Single<ANI01>(a => a.I01F01 == _aniId);

                    // Update only the fields that are provided
                    if (!string.IsNullOrEmpty(_objANI01.I01F02)) currentAnime.I01F02 = _objANI01.I01F02;
                    if (_objANI01.I01F03 > 0) currentAnime.I01F03 = _objANI01.I01F03;
                    if (_objANI01.I01F04 > 0) currentAnime.I01F04 = _objANI01.I01F04;
                    if (_objANI01.I01F05 > 0) currentAnime.I01F05 = _objANI01.I01F05;

                    // Save the updated record to the database
                    db.Update(currentAnime);
                    _objResponse.IsError = false;
                    _objResponse.message = "Anime updated successfully.";
                }
                catch (Exception ex)
                {
                    // Handle exceptions and return an error response
                    _objResponse.IsError = true;
                    _objResponse.message = ex.Message;
                }
            }
            return _objResponse;
        }

        /// <summary>
        /// Deletes an anime record from the database by ID.
        /// </summary>
        private Response DeleteAnime()
        {
            using (var db = DatabseHelper.GetConnectionFactory())
            {
                try
                {
                    // Delete the anime record by ID
                    db.DeleteById<ANI01>(_aniId);
                    _objResponse.IsError = false;
                    _objResponse.message = "Anime removed successfully.";
                }
                catch (Exception ex)
                {
                    // Handle exceptions and return an error response
                    _objResponse.IsError = true;
                    _objResponse.message = ex.Message;
                }
            }
            return _objResponse;
        }

        /// <summary>
        /// Prepares for a delete operation by setting the anime ID.
        /// </summary>
        /// <param name="id">ID of the anime to delete.</param>
        public void preDelete(int id)
        {
            if (id > 0)
            {
                _aniId = id;
            }
        }

        /// <summary>
        /// Prepares for a save operation by converting DTO to POCO.
        /// </summary>
        /// <param name="objDto">DTO containing anime data.</param>
        public void preSave(DTOANI01 objDto)
        {
            // Convert DTO to POCO and reset response fields
            _objANI01 = objDto.ConvertTOPoco<ANI01>();
            _objResponse.data = null;
            _objResponse.message = null;
            _objResponse.IsError = false;

            if (Type == enmType.Edit && _objANI01.I01F01 > 0)
            {
                _aniId = _objANI01.I01F01;
            }
        }

        /// <summary>
        /// Checks if an anime record exists in the database by ID.
        /// </summary>
        private bool IsExistANI01()
        {
            using (var db = DatabseHelper.GetConnectionFactory())
            {
                // Check if the anime record exists
                return db.Exists<ANI01>(_aniId);
            }
        }

        /// <summary>
        /// Validates the operation based on the type and anime existence.
        /// </summary>
        /// <returns>Response indicating validation result.</returns>
        public Response Validate()
        {
            if (Type == enmType.Edit || Type == enmType.Delete)
            {
                if (IsExistANI01())
                {
                    _objResponse.IsError = false;
                }
                else
                {
                    _objResponse.IsError = true;
                    _objResponse.message = "Anime does not exist.";
                }
            }
            return _objResponse;
        }

        /// <summary>
        /// Executes the save operation based on the operation type.
        /// </summary>
        public Response Save()
        {
            if (Type == enmType.Add)
            {
                return AddAnime();
            }
            else if (Type == enmType.Delete)
            {
                return DeleteAnime();
            }
            else if (Type == enmType.Edit)
            {
                return UpdateAnime();
            }
            return _objResponse;
        }
    }
}
