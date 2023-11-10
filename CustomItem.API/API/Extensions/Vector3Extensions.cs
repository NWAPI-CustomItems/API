using UnityEngine;

namespace NWAPI.CustomItems.API.Extensions
{
    /// <summary>
    /// A collection of extension methods for <see cref="Vector3"/>
    /// </summary>
    public static class Vector3Extensions
    {
        /// <summary>
        /// Parses a string representation of a Vector3 into a Vector3 object.
        /// The string can be in the format "(x, y, z)", "x, y, z", or "x y z".
        /// </summary>
        /// <param name="stringVector">The string representation of the Vector3.</param>
        /// <returns>A Vector3 object if parsing is successful, or null if the input is invalid.</returns>
        public static Vector3? Parse(string stringVector)
        {
            // Remove parentheses and spaces, and split the string into components
            stringVector = stringVector.Replace("(", "").Replace(")", "").Replace(" ", "");
            string[] sArray = stringVector.Split(',');

            if (sArray.Length == 3 &&
                float.TryParse(sArray[0], out float x) &&
                float.TryParse(sArray[1], out float y) &&
                float.TryParse(sArray[2], out float z))
            {
                return new Vector3(x, y, z);
            }

            return null;
        }

        /// <summary>
        /// Tries to parse a string representation of a Vector3 into a Vector3 object.
        /// The string can be in the format "(x, y, z)", "x, y, z", or "x y z".
        /// </summary>
        /// <param name="stringVector">The string representation of the Vector3.</param>
        /// <param name="vector">The parsed Vector3 if parsing is successful, or Vector3.zero if the input is invalid.</param>
        /// <returns>True if parsing is successful, false if the input is invalid.</returns>
        public static bool TryParse(string stringVector, out Vector3 vector)
        {
            vector = Vector3.zero; // Initialize the output vector with a default value

            // Remove parentheses and spaces, and split the string into components
            stringVector = stringVector.Replace("(", "").Replace(")", "").Replace(" ", "");
            string[] sArray = stringVector.Split(',');

            if (sArray.Length == 3 &&
                float.TryParse(sArray[0], out float x) &&
                float.TryParse(sArray[1], out float y) &&
                float.TryParse(sArray[2], out float z))
            {
                vector = new Vector3(x, y, z);
                return true;
            }

            return false;
        }
    }
}
