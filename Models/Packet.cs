using System;

namespace VantSharp.Models
{
    /* A Packet describes a payload for a transmission frame. It contains
     * headers as well as actual content. The structure for this payload
     * follows the structure as laid here:
     *
     * First packet (handshake):
     *
     *    4 characters for sequential ID 0000 (ID)
     *   32 characters for content hash (HASH)
     *    1 characters for packet type (TYPE)
     *    4 characters for tag identifier (TAG)
     *    2 characters for source node identifier (SNO)
     *    2 characters for destination node identifier (DNO)
     *   18 characters for GPS position of drone (POS)
     *   20 characters for timestamp of event (TIME)
     *    4 characters for sequential identifier of last packet (LID)
     * --------------------------------------
     *   87 characters (bytes) TOTAL
     *
     *
     *
     * Data packet:
     *
     *    4 characters for sequential ID (ID)
     *    4 characters for tag identifier (TAG)
     *  248 characters for data (PAYLOAD)
     * --------------------------------------
     *  256 characters (bytes) TOTAL
     *
     */

    public enum PacketType
    {
        HANDSHAKE = 0,
        IMAGE = 1
    }
    
    public class Packet
    {
        /* Constants */
        public static int ID_SIZE = 4;
        public static int HASH_SIZE = 32;
        public static int TYPE_SIZE = 1;
        public static int TAG_SIZE = 4;
        public static int SNO_SIZE = 2;
        public static int DNO_SIZE = 2;
        public static int POS_SIZE = 18;
        public static int TIME_SIZE = 20;
        public static int LID_SIZE = 4;
        public static int PAYLOAD_SIZE = 256 - (ID_SIZE + TAG_SIZE);

        /* Properties */
        public int Id { get; set; }
        public string Hash { get; set; }
        public PacketType Type { get; set; }
        public int Tag { get; set; }
        public int SourceNode { get; set; }
        public int DestinationNode { get; set; }
        public string Position { get; set; }
        public string Time { get; set; }
        public int LastIdentification { get; set; }
        public byte[] Payload { get; set; }

        /* Methods */
        public string Encode()
        {
            // Create a byte array to represent whole packet
            byte[] content = new byte[ID_SIZE + Payload.Length];
            // Convert ID to a string of size 3 and leading zeros
            var id = Id.ToString().PadLeft(ID_SIZE, '0');
            // Iterate this string to copy over content to byte array
            for (int i = 0; i < id.Length; i++)
            {
                content[i] = (byte)(id.ToCharArray()[i]);
            }
            // Copy rest of the content from Content array starting at array
            // ID_SIZE to skip parts already containing ID
            Payload.CopyTo(content, ID_SIZE);

            // Return result as a string of hex literals representing content
            // byte array
            return BitConverter.ToString(content).Replace("-", string.Empty);
        }
    }
}