using System;
using System.Linq;
using System.Reflection;

namespace CRUDwithDatabase.Helpers
{
    /// <summary>
    /// Provides an extension method to convert a Data Transfer Object (DTO) to a Plain Old CLR Object (POCO).
    /// </summary>
    public static class DtoToPoco
    {
        /// <summary>
        /// Converts a DTO object to a POCO object of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the POCO object to create.</typeparam>
        /// <param name="dto">The DTO object to convert.</param>
        /// <returns>A new POCO object of type <typeparamref name="T"/> with properties mapped from the DTO.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="dto"/> is null.</exception>
        public static T ConvertTOPoco<T>(this object dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto), "DTO cannot be null.");
            }

            // Get the type of the POCO class and create an instance of it.
            Type pocoType = typeof(T);
            T pocoInstance = (T)Activator.CreateInstance(pocoType);

            // Retrieve property information for both the POCO and the DTO.
            PropertyInfo[] pocoProperties = pocoType.GetProperties();
            PropertyInfo[] dtoProperties = dto.GetType().GetProperties();

            // Map matching properties from the DTO to the POCO.
            foreach (PropertyInfo dtoProp in dtoProperties)
            {
                PropertyInfo pocoProp = Array.Find(pocoProperties, p => p.Name == dtoProp.Name);
                if (pocoProp != null && pocoProp.PropertyType == dtoProp.PropertyType)
                {
                    object value = dtoProp.GetValue(dto);
                    pocoProp.SetValue(pocoInstance, value);
                }
            }

            return pocoInstance;
        }
    }
}
