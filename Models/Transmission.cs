using System;
using System.Collections.Generic;

namespace VantSharp.Models
{
    public class Transmission
    {
        private List<Packet> _packets;
        public ICollection<Packet> Packets
        {
            get { return _packets; }
        }

        public int PacketCount
        {
            get => _packets.Count;
        }
        
        public Transmission()
        {
            _packets = new List<Packet>();
        }
    }
}