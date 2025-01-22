using AnimeList.Helper;
using AnimeList.Models;
using AnimeList.Models.dto;
using AnimeList.Models.poco;
using Google.Protobuf.Collections;
using MySql.Data.MySqlClient;
using ServiceStack;
using ServiceStack.OrmLite;
using serviceStackDemo.Service;
using System;
using System.Collections.Generic;

namespace AnimeList.DatabaseLogic
{
    public class DLANL01 : IDataHandler<DTOANL01>
    {
        private Response _objResponse; // Response object for operation results
        private ANL01 _objANL01; // POCO object for anime list data
        private int _listId; // List ID for identifying specific lists
        private int _aniId; // Anime ID for specific anime in a list

        public DLANL01()
        {
            _objResponse = new Response();
            _objANL01 = new ANL01();
        }

        // Type of operation (Add, Edit, Delete)
        public enmType Type { get; set; }

        /// <summary>
        /// Retrieves all anime entries in a specific list.
        /// </summary>
        /// <param name="id">ID of the list.</param>
        /// <returns>Response containing the list of anime.</returns>
        public Response GetListAnimes(int id)
        {
            //string query = "SELECT L01F01, I01F02, I01F03, I01F04, I01F05, L01F03, I01F01 " +
            //               "FROM ANL01 as L01 " +
            //               "JOIN ANI01 as I01 ON ANI01.I01F01 = ANL01.L01F02 " +
            //               "WHERE L01F01 = " + id;

            string query = @"SELECT 
                                L01F01, 
                                I01F02, 
                                I01F03, 
                                I01F04, 
                                I01F05, 
                                L01F03, 
                                I01F01
                             FROM
                               ANL01 as L01 
                             JOIN 
                               ANI01 as I01 ON ANI01.I01F01 = ANL01.L01F02
                             WHERE
                                L01F01 = @id";
            using (var connection = DatabseHelper.GetConnection())
            {
                try
                {
                    var results = new List<dynamic>();
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.AddParam("@id", id);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    // Read and map the data to a dynamic object list
                    while (reader.Read())
                    {
                        results.Add(new
                        {
                            ListId = reader[0],
                            AnimeID = reader[6],
                            AnimeTitle = reader[1].ToString(),
                            NoOfSeasons = reader[2],
                            NoOfEpisodes = reader[3],
                            ReleaseYear = reader[4],
                            Status = reader[5],
                        });
                    }
                    _objResponse.IsError = false;
                    _objResponse.data = results;
                }
                catch (Exception ex)
                {
                    // Handle exceptions and set error response
                    _objResponse.IsError = true;
                    _objResponse.message = ex.Message;
                }
            }
            return _objResponse;
        }

        /// <summary>
        /// Updates the status of an anime in the list.
        /// </summary>
        public Response UpdateStatus()
        {
            using (var db = DatabseHelper.GetConnectionFactory())
            {
                try
                {
                    // Update the anime's status in the list
                    db.Update(_objANL01);
                    _objResponse.IsError = false;
                    _objResponse.message = "Status updated successfully.";
                }
                catch (Exception ex)
                {
                    // Handle exceptions and set error response
                    _objResponse.IsError = true;
                    _objResponse.message = ex.Message;
                }
            }
            return _objResponse;
        }

        /// <summary>
        /// Prepares for delete operation by setting list and anime IDs.
        /// </summary>
        public void preDelete(int listId, int animeId)
        {
            if (listId > 0 && animeId > 0)
            {
                _listId = listId;
                _aniId = animeId;
            }
        }

        /// <summary>
        /// Prepares for save operation by converting DTO to POCO.
        /// </summary>
        public void preSave(DTOANL01 objDto)
        {
            _objANL01 = objDto.ConvertTo<ANL01>();
            _objResponse.data = null;
            _objResponse.message = null;
            _objResponse.IsError = false;

            if (objDto.L01F01 > 0 && objDto.L01F02 > 0)
            {
                _listId = objDto.L01F01;
                _aniId = objDto.L01F02;
            }
        }

        /// <summary>
        /// Adds an anime to a list.
        /// </summary>
        private Response AddAnime()
        {
            using (var db = DatabseHelper.GetConnectionFactory())
            {
                try
                {
                    // Insert a new anime into the list
                    db.Insert(_objANL01);
                    _objResponse.IsError = false;
                    _objResponse.message = "Anime added to the list.";
                }
                catch (Exception ex)
                {
                    // Handle exceptions and set error response
                    _objResponse.IsError = true;
                    _objResponse.message = ex.Message;
                }
            }
            return _objResponse;
        }

        /// <summary>
        /// Deletes an anime from a list.
        /// </summary>
        private Response DeleteAnime()
        {
            using (var db = DatabseHelper.GetConnectionFactory())
            {
                try
                {
                    // Delete anime by list ID
                    db.DeleteById<ANL01>(_listId);
                    _objResponse.IsError = false;
                    _objResponse.message = "Anime removed successfully.";
                }
                catch (Exception ex)
                {
                    // Handle exceptions and set error response
                    _objResponse.IsError = true;
                    _objResponse.message = ex.Message;
                }
            }
            return _objResponse;
        }

        /// <summary>
        /// Checks if a specific anime exists in a list.
        /// </summary>
        private bool IsExistANL01()
        {
            using (var db = DatabseHelper.GetConnectionFactory())
            {
                // Check if the record exists based on list and anime IDs
                return db.Exists<ANL01>(x => x.L01F01 == _listId && x.L01F02 == _aniId);
            }
        }

        /// <summary>
        /// Saves the anime list based on the operation type.
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
                return UpdateStatus();
            }

            return _objResponse;
        }

        /// <summary>
        /// Validates if the anime list operation is valid.
        /// </summary>
        public Response Validate()
        {
            if (Type == enmType.Delete || Type == enmType.Edit)
            {
                if (IsExistANL01())
                {
                    _objResponse.IsError = false;
                    return _objResponse;
                }
                else
                {
                    _objResponse.IsError = true;
                    _objResponse.message = "List does not exist.";
                }
            }
            return _objResponse;
        }
    }
}
