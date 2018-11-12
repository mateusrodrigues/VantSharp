using System;
using System.Text;
using Serilog;

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
        DATA = 0,
        STATUS = 1,
        IMAGE = 2
    }
    
    public class Packet
    {
        /* Constants */
        public static int ID_SIZE = 4;
        public static int HASH_SIZE = 7;
        public static int TYPE_SIZE = 1;
        public static int TAG_SIZE = 4;
        public static int SNO_SIZE = 2;
        public static int DNO_SIZE = 2;
        public static int POS_SIZE = 18;
        public static int TIME_SIZE = 10;
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

        public bool IsFirstPacket { get; set; }

        /* Methods */
        public string Encode()
        {
            // Create a byte array to represent whole packet
            byte[] content;
            if (IsFirstPacket)
            {
                content = new byte[ID_SIZE
                    + HASH_SIZE
                    + TYPE_SIZE
                    + TAG_SIZE
                    + SNO_SIZE
                    + DNO_SIZE
                    + POS_SIZE 
                    + TIME_SIZE
                    + LID_SIZE
                ];
            }
            else
            {
                content = new byte[ID_SIZE + TAG_SIZE + Payload.Length];
            }

            // Convert ID to a string of size 3 and leading zeros
            var id = Id.ToString().PadLeft(ID_SIZE, '0');
            // Iterate this string to copy over content to byte array
            for (int i = 0; i < id.Length; i++)
            {
                content[i] = (byte)(id.ToCharArray()[i]);
            }

            // First packet requires special treatment since its contents are
            // different than ordinary ones.
            if (IsFirstPacket)
            {
                // Copy the hash over to content
                Array.Copy(Encoding.ASCII.GetBytes(Hash.ToCharArray()),
                    0, content, ID_SIZE, HASH_SIZE);
                // Copy the type number over to content
                var type = ((int) Type).ToString().PadLeft(TYPE_SIZE, '0');
                Array.Copy(Encoding.ASCII.GetBytes(type.ToCharArray()),
                    0, content, ID_SIZE + HASH_SIZE, TYPE_SIZE);
                // Copy the tag over to content
                var tag = Tag.ToString().PadLeft(TAG_SIZE, '0');
                Array.Copy(Encoding.ASCII.GetBytes(tag.ToCharArray()),
                    0, content, ID_SIZE + HASH_SIZE + TYPE_SIZE, TAG_SIZE);
                // Copy the source node address over to content
                var sno = SourceNode.ToString().PadLeft(SNO_SIZE, '0');
                Array.Copy(Encoding.ASCII.GetBytes(sno.ToCharArray()),
                    0, content, ID_SIZE + HASH_SIZE + TYPE_SIZE + TAG_SIZE, 
                    SNO_SIZE);
                // Copy destination node over to content
                var dno = DestinationNode.ToString().PadLeft(DNO_SIZE, '0');
                Array.Copy(Encoding.ASCII.GetBytes(dno.ToCharArray()),
                    0, content, ID_SIZE + HASH_SIZE + TYPE_SIZE + TAG_SIZE 
                        + SNO_SIZE, 
                    DNO_SIZE);
                // Copy the GPS position over to content
                Array.Copy(Encoding.ASCII.GetBytes(Position.ToCharArray()),
                    0, content, ID_SIZE + HASH_SIZE + TYPE_SIZE + TAG_SIZE 
                        + SNO_SIZE + DNO_SIZE,
                    POS_SIZE);
                // Copy the timestamp over to content
                Array.Copy(Encoding.ASCII.GetBytes(Time.ToCharArray()),
                    0, content, ID_SIZE + HASH_SIZE + TYPE_SIZE + TAG_SIZE 
                        + SNO_SIZE + DNO_SIZE + POS_SIZE,
                    TIME_SIZE);
                // Copy the last identification over to content
                var lid = LastIdentification.ToString().PadLeft(LID_SIZE, '0');
                Array.Copy(Encoding.ASCII.GetBytes(lid.ToCharArray()),
                    0, content, ID_SIZE + HASH_SIZE + TYPE_SIZE + TAG_SIZE 
                        + SNO_SIZE + DNO_SIZE + POS_SIZE + TIME_SIZE, 
                    LID_SIZE);
            }
            else
            {
                // Copy the tag over to content
                var tag = Tag.ToString().PadLeft(TAG_SIZE, '0');
                Array.Copy(Encoding.ASCII.GetBytes(tag.ToCharArray()),
                    0, content, ID_SIZE, TAG_SIZE);
                // Copy the payload over to content
                // Use Payload.Length instead of PAYLOAD_SIZE because the last
                // payload packet may not be of size PAYLOAD_SIZE, triggering an
                // CLR exception.
                Array.Copy(Payload, 0, content, ID_SIZE + TAG_SIZE, Payload.Length);
            }

            // Copy rest of the content from Content array starting at array
            // ID_SIZE to skip parts already containing ID
            //Payload.CopyTo(content, ID_SIZE);

            // Return result as a string of hex literals representing content
            // byte array
            return BitConverter.ToString(content).Replace("-", string.Empty);
        }
    }
}