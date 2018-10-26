using System.Collections.Generic;

namespace VantSharp.Models
{
    public class Transmission
    {
        /* Properties */
        private List<Packet> _packets;
        public List<Packet> Packets
        {
            get { return _packets; }
        }

        public int PacketCount
        {
            get => _packets.Count;
        }
        
        /* Methods */
        public Transmission()
        {
            _packets = new List<Packet>();
        }
    }
}