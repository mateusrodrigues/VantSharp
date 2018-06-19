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
    }
}