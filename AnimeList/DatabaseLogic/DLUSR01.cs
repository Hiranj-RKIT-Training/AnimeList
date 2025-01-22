using AnimeList.Helper;
using AnimeList.Models;
using AnimeList.Models.dto;
using AnimeList.Models.poco;
using CRUDwithDatabase.Helpers;
using MySql.Data.MySqlClient;
using ServiceStack;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using serviceStackDemo.Service;
using System;

namespace AnimeList.DatabaseLogic
{
    /// <summary>
    /// Database Logic Layer for handling operations related to users (USR01 table).
    /// </summary>
    public class DLUSR01 : IDataHandler<DTOUSR01>
    {
        private USR01 _objUSR01; // POCO object for user entity
        private Response _objResponse; // Response object to hold result and status
        private int _id; // ID of the user being processed

        public DLUSR01()
        {
            _objUSR01 = new USR01();
            _objResponse = new Response();
        }

        public enmType Type { get; set; } // Type of operation (Add, Edit, Delete)

        /// <summary>
        /// Adds a new user to the database.
        /// </summary>
        /// <returns>Response with the status of the operation.</returns>
        public Response addUser()
        {
            using (var db = DatabseHelper.GetConnectionFactory())
            {
                db.Insert<USR01>(_objUSR01); // Insert user into the database
                _objResponse.IsError = false;
                _objResponse.message = "User Added";

                // Generate JWT token for the new user
                var token = JWTHelper.GenerateToken(_objUSR01.R01F02, _objUSR01.R01F04.ToString());
                _objResponse.data = new
                {
                    token = token,
                };
            }
            return _objResponse;
        }

        /// <summary>
        /// Logs in a user by verifying their credentials.
        /// </summary>
        /// <param name="email">User's email.</param>
        /// <param name="password">User's password.</param>
        /// <returns>Response with a JWT token if login is successful.</returns>
        public Response login(string email, string password)
        {
            string encryptedPassword = AesProvider.Encryption(password); // Encrypt the input password
            string query = $"SELECT R01F02, R01F03, R01F04 FROM USR01 WHERE R01F02 = '{email}'";

            using (var connection = DatabseHelper.GetConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string databasePass = reader[1].ToString(); // Fetch the encrypted password from DB
                    if (databasePass == encryptedPassword)
                    {
                        // Generate JWT token if password matches
                        var jwtToken = JWTHelper.GenerateToken(reader[0].ToString(), reader[2].ToString());
                        _objResponse.data = new
                        {
                            token = jwtToken
                        };
                        _objResponse.IsError = false;
                    }
                    else
                    {
                        _objResponse.IsError = true;
                        _objResponse.message = "Invalid Email or Password";
                    }
                }
            }
            return _objResponse;
        }

        /// <summary>
        /// Updates an existing user in the database.
        /// </summary>
        /// <returns>Response indicating the status of the update.</returns>
        private Response UpdateUser()
        {
            using (var db = DatabseHelper.GetConnection())
            {
                USR01 existingUser = db.Single<USR01>(e => e.R01F01 == _id); // Fetch the user by ID

                // Update user fields if provided
                if (!string.IsNullOrEmpty(_objUSR01.R01F05))
                    existingUser.R01F05 = _objUSR01.R01F05;

                if (!string.IsNullOrEmpty(_objUSR01.R01F06))
                    existingUser.R01F06 = _objUSR01.R01F06;

                if (_objUSR01.R01F07 > 0) // Assuming age cannot be zero
                    existingUser.R01F07 = _objUSR01.R01F07;

                db.Update<USR01>(existingUser); // Save updates to the database
            }
            return _objResponse;
        }

        /// <summary>
        /// Deletes a user from the database.
        /// </summary>
        /// <returns>Response indicating the status of the deletion.</returns>
        private Response DeleteUser()
        {
            using (var db = DatabseHelper.GetConnectionFactory())
            {
                db.DeleteById<USR01>(_id); // Delete the user by ID
                _objResponse.message = "User Deleted Successfully";
                _objResponse.IsError = false;
            }
            return _objResponse;
        }

        /// <summary>
        /// Checks if a user exists in the database.
        /// </summary>
        /// <returns>True if the user exists, otherwise false.</returns>
        private bool IsExistUSR01()
        {
            using (var db = DatabseHelper.GetConnectionFactory())
            {
                return db.Exists<USR01>(_id); // Check if a user with the given ID exists
            }
        }

        /// <summary>
        /// Prepares for deletion by setting the user ID.
        /// </summary>
        /// <param name="id">ID of the user to be deleted.</param>
        public void preDelete(int id)
        {
            if (id > 0)
            {
                _id = id;
            }
        }

        /// <summary>
        /// Prepares the DTO for saving or updating.
        /// </summary>
        /// <param name="objDto">DTO object containing user data.</param>
        public void preSave(DTOUSR01 objDto)
        {
            _objUSR01 = objDto.ConvertTOPoco<USR01>();

            if (Type == enmType.Add)
            {
                _objUSR01.R01F03 = AesProvider.Encryption(_objUSR01.R01F03); // Encrypt password
            }

            if (Type == enmType.Edit && objDto.R01F01 > 0)
            {
                _id = objDto.R01F01; // Set ID for edit operations
            }
        }

        /// <summary>
        /// Saves the user data based on the operation type.
        /// </summary>
        /// <returns>Response indicating the status of the operation.</returns>
        public Response Save()
        {
            if (Type == enmType.Add)
            {
                _objResponse =  DatabseHelper.InsertQuery<USR01>(_objUSR01);

                if(_objResponse.IsError)
                {
                    return _objResponse;
                }
                var token = JWTHelper.GenerateToken(_objUSR01.R01F02, _objUSR01.R01F04.ToString());
                _objResponse.data = new
                {
                    token = token,
                };
                return _objResponse;
            }
            else if (Type == enmType.Edit)
            {
                return UpdateUser();
            }
            else if (Type == enmType.Delete)
            {
                return DeleteUser();
            }
            return _objResponse;
        }

        /// <summary>
        /// Validates the existence of a user for edit or delete operations.
        /// </summary>
        /// <returns>Response indicating validation result.</returns>
        public Response Validate()
        {
            if (Type == enmType.Edit || Type == enmType.Delete)
            {
                if (IsExistUSR01())
                {
                    _objResponse.IsError = false;
                }
                else
                {
                    _objResponse.IsError = true;
                    _objResponse.message = "User does not exist.";
                }
            }
            return _objResponse;
        }
    }
}
