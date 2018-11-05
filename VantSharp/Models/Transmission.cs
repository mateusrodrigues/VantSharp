using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

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
        
        /* Constructors */
        public Transmission()
        {
            _packets = new List<Packet>();
        }

        public Transmission(string filePath)
        {
            _packets = new List<Packet>();
            if (File.Exists(filePath))
            {
                // Read the file and get necessary metadata information
                byte[] file = File.ReadAllBytes(filePath);
                // Calculate the amount of necessary packets
                int packetCount = int.Parse(Math.Ceiling(
                    (double) file.Length / Packet.PAYLOAD_SIZE).ToString());
                // Id counter
                int counter = 0;

                // Build the first packet and add it to transmission
                Packet firstPacket = new Packet
                {
                    Id = counter++,
                    Hash = file.GetHashCode().ToString("X"),
                    //TODO: Get transmission type dynamically
                    Type = PacketType.IMAGE,
                    Tag = 1,
                    //TODO: Gather source node from other tool
                    SourceNode = 0,
                    //TODO: Gather destination node from other tool
                    DestinationNode = 0,
                    //TODO: Gather position from other tool
                    Position = string.Empty,
                    Time = DateTimeOffset.Now.ToUnixTimeSeconds().ToString(),
                    LastIdentification = packetCount
                };
                Packets.Add(firstPacket);

                // Add payload data packets to transmission
                int startIndex = 0;
                while (counter <= packetCount)
                {
                    int length = 0;
                    if (counter < packetCount)
                    {
                        length = Packet.PAYLOAD_SIZE;
                    }
                    else
                    {
                        length = file.Length - ((counter - 1) * Packet.PAYLOAD_SIZE);
                    }

                    Packet packet = new Packet
                    {
                        Id = counter++,
                        Tag = 1,
                        Payload = new byte[length]
                    };

                    Array.Copy(file, startIndex, packet.Payload,
                        0, length);
                    startIndex += Packet.PAYLOAD_SIZE;
                }
            }
        }
    }
}