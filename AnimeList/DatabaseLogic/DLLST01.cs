using AnimeList.Helper;
using AnimeList.Models;
using AnimeList.Models.dto;
using AnimeList.Models.poco;
using ServiceStack;
using ServiceStack.OrmLite;
using serviceStackDemo.Service;
using System;
using System.Collections.Generic;

namespace AnimeList.DatabaseLogic
{
    /// <summary>
    /// Database Logic Layer for handling operations related to anime lists (LST01 table).
    /// </summary>
    public class DLLST01 : IDataHandler<DTOLST01>
    {
        private Response _objResponse; // Response object to store results or errors
        private LST01 _objLST01; // POCO object representing the anime list
        private int _listId; // ID of the list to be manipulated

        /// <summary>
        /// Constructor to initialize the response and list objects.
        /// </summary>
        public DLLST01()
        {
            _objResponse = new Response();
            _objLST01 = new LST01();
        }

        // Type of operation to perform (Add or Delete).
        public enmType Type { get; set; }

        /// <summary>
        /// Retrieves all anime lists for a specific user.
        /// </summary>
        /// <param name="userId">ID of the user.</param>
        /// <returns>Response containing the list of anime lists.</returns>
        public Response GetUserList(int userId)
        {
            using (var db = DatabseHelper.GetConnectionFactory())
            {
                try
                {
                    // Fetch all lists where T01F02 matches the user ID
                    List<LST01> results = db.Select<LST01>(e => e.T01F02 == userId);
                    _objResponse.IsError = false;
                    _objResponse.data = results;
                }
                catch (Exception ex)
                {
                    // Handle exceptions and return error response
                    _objResponse.IsError = true;
                    _objResponse.message = ex.Message;
                }
            }
            return _objResponse;
        }

        /// <summary>
        /// Creates a new anime list in the database.
        /// </summary>
        /// <returns>Response indicating success or failure.</returns>
        private Response CreateList()
        {
            using (var db = DatabseHelper.GetConnectionFactory())
            {
                try
                {
                    // Insert a new list into the database
                    db.Insert(_objLST01);
                    _objResponse.IsError = false;
                    _objResponse.message = "List created successfully.";
                }
                catch (Exception ex)
                {
                    // Handle exceptions and return error response
                    _objResponse.IsError = true;
                    _objResponse.message = ex.Message;
                }
            }
            return _objResponse;
        }

        /// <summary>
        /// Deletes an existing anime list from the database.
        /// </summary>
        /// <returns>Response indicating success or failure.</returns>
        private Response DeleteList()
        {
            using (var db = DatabseHelper.GetConnectionFactory())
            {
                try
                {
                    // Delete the list based on the list ID
                    db.DeleteById<LST01>(_listId);
                    _objResponse.IsError = false;
                    _objResponse.message = "List removed successfully.";
                }
                catch (Exception ex)
                {
                    // Handle exceptions and return error response
                    _objResponse.IsError = true;
                    _objResponse.message = ex.Message;
                }
            }
            return _objResponse;
        }

        /// <summary>
        /// Prepares for the delete operation by setting the list ID.
        /// </summary>
        /// <param name="id">ID of the list to be deleted.</param>
        public void preDelete(int id)
        {
            if (id > 0)
            {
                _listId = id;
            }
        }

        /// <summary>
        /// Prepares the DTO object for saving to the database.
        /// </summary>
        /// <param name="objDto">DTO object containing the list data.</param>
        public void preSave(DTOLST01 objDto)
        {
            _objLST01 = objDto.ConvertTo<LST01>();
            _objResponse.data = null;
            _objResponse.message = null;
            _objResponse.IsError = false;
        }

        /// <summary>
        /// Checks if a list exists in the database.
        /// </summary>
        /// <returns>True if the list exists; otherwise, false.</returns>
        private bool IsExistLST01()
        {
            using (var db = DatabseHelper.GetConnectionFactory())
            {
                // Check if a list exists with the specified ID
                return db.Exists<LST01>(_listId);
            }
        }

        /// <summary>
        /// Saves the list data based on the specified operation type.
        /// </summary>
        /// <returns>Response indicating success or failure.</returns>
        public Response Save()
        {
            if (Type == enmType.Add)
            {
                return CreateList();
            }
            else if (Type == enmType.Delete)
            {
                return DeleteList();
            }

            return _objResponse;
        }

        /// <summary>
        /// Validates if the specified list exists for the operation.
        /// </summary>
        /// <returns>Response indicating validation result.</returns>
        public Response Validate()
        {
            if (Type == enmType.Delete)
            {
                if (IsExistLST01())
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
