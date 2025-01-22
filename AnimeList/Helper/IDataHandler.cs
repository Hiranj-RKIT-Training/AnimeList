using AnimeList.Models;

using System;

namespace serviceStackDemo.Service
{
    /// <summary>
    /// Defines a contract for handling data operations for a specific data type.
    /// </summary>
    /// <typeparam name="T">The type of the data object (e.g., DTO) to be handled.</typeparam>
    internal interface IDataHandler<T>
    {
        /// <summary>
        /// Gets or sets the type of operation (Add, Edit, Delete).
        /// </summary>
        enmType Type { get; set; }

        /// <summary>
        /// Prepares the system for saving the specified data object.
        /// </summary>
        /// <param name="objDto">The data object to prepare for saving.</param>
        void preSave(T objDto);

        /// <summary>
        /// Validates the current operation and its associated data.
        /// </summary>
        /// <returns>A <see cref="Response"/> object containing the validation results.</returns>
        Response Validate();

        /// <summary>
        /// Saves the data object to the data store.
        /// </summary>
        /// <returns>A <see cref="Response"/> object indicating the result of the save operation.</returns>
        Response Save();
    }
}
