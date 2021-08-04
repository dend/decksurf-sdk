// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace DeckSurf.SDK.Util
{
    /// <summary>
    /// Helper class used for data conversion and management.
    /// </summary>
    public class DataHelpers
    {
        /// <summary>
        /// Converts a byte array to string.
        /// </summary>
        /// <param name="data">Source byte array.</param>
        /// <returns>String, generated from byte array.</returns>
        public static string ByteArrayToString(byte[] data)
        {
            return BitConverter.ToString(data);
        }
    }
}
