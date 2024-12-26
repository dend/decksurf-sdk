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

        /// <summary>
        /// Converts an unsigned integer into a byte array representation that can be used
        /// in packet payloads. Contains the Little Endian representation.
        /// </summary>
        /// <param name="value">Value to be converted.</param>
        /// <returns>An array of <see cref="byte"/> that represents the Little Endian representation.</returns>
        public static byte[] GetLittleEndianBytesFromInt(int value)
        {
            byte[] littleEndianBytes =
            [
                (byte)(value & 0xFF),
                (byte)((value >> 8) & 0xFF),
            ];
            return littleEndianBytes;
        }

        /// <summary>
        /// Converts a byte array representation in Little Endian back into an integer.
        /// </summary>
        /// <param name="littleEndianBytes">Array of bytes in Little Endian format.</param>
        /// <returns>An unsigned integer obtained from the byte array.</returns>
        public static int GetIntFromLittleEndianBytes(byte[] littleEndianBytes)
        {
            if (littleEndianBytes.Length != 2)
            {
                throw new ArgumentException("The byte array must be exactly 2 bytes long.");
            }

            int value = littleEndianBytes[0] | (littleEndianBytes[1] << 8);

            return value;
        }
    }
}
