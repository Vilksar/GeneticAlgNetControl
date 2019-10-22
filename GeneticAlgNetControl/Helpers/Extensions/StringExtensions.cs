using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace GeneticAlgNetControl.Helpers.Extensions
{
    /// <summary>
    /// Provides extensions for strings.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Checks if a specified string is a valid JSON object of the specified type.
        /// </summary>
        /// <typeparam name="T">Represents the type of the object to check the JSON string against.</typeparam>
        /// <param name="jsonString">Represents a JSON string.</param>
        /// <returns>Returns "true" if the specified JSON string is valid, and "false" otherwise.</returns>
        public static bool IsNullOrInvalidJsonObject<T>(this string jsonString)
        {
            // Check if the string is null or empty.
            if (string.IsNullOrEmpty(jsonString))
            {
                // Return true.
                return true;
            }
            // Try to deserialize the given string as the chosen object.
            try
            {
                // Deserialize the given JSON string as the given type.
                JsonSerializer.Deserialize<T>(jsonString);
                // Everything went alright, so return false.
                return false;
            }
            catch (Exception)
            {
                // If anything went wrong, return true.
                return true;
            }
        }
    }
}
