using System;

namespace VantSharp.Models
{
    public class Packet
    {
        /* A Packet describes a payload for a transmission frame. It contains
         * headers as well as actual content. The structure for this payload
         * follows the structure as laid here:
         *
         *    3 characters for sequential ID
         *  253 characters for content
         * --------------------------------------
         *  256 characters (bytes) TOTAL
         *
         */

        /* Constants */
        public static int ID_SIZE = 3;
        public static int PAYLOAD_SIZE = 253;

        /* Properties */
        public int Id { get; set; }
        public byte[] Content { get; set; }

        /* Methods */
        public string Encode()
        {
            // Create a byte array to represent whole packet
            byte[] content = new byte[ID_SIZE + Content.Length];
            // Convert ID to a string of size 3 and leading zeros
            var id = Id.ToString().PadLeft(ID_SIZE, '0');
            // Iterate this string to copy over content to byte array
            for (int i = 0; i < id.Length; i++)
            {
                content[i] = (byte)(id.ToCharArray()[i]);
            }
            // Copy rest of the content from Content array starting at array
            // ID_SIZE to skip parts already containing ID
            Content.CopyTo(content, ID_SIZE);

            // Return result as a string of hex literals representing content
            // byte array
            return BitConverter.ToString(content).Replace("-", string.Empty);
        }
    }
}