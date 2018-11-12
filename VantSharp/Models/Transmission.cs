using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Serilog;

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

        // This constructor is used for transmissions where files
        // are present, such as photos, text files, and such.
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
                    Position = "000000000000000000",
                    Time = DateTimeOffset.Now.ToUnixTimeSeconds().ToString(),
                    LastIdentification = packetCount,
                    IsFirstPacket = true
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

                    Packets.Add(packet);
                    startIndex += Packet.PAYLOAD_SIZE;
                }
            }
        }

        public void Transmit()
        {
            ProcessStartInfo start = new ProcessStartInfo()
            {
                FileName = "python3",
                UseShellExecute = false,
                RedirectStandardOutput = true
            };

            // Iterate over packets and transmit each one
            int counter = 0;
            foreach (var packet in Packets)
            {
                var encodedPacket = packet.Encode();

                // Log packet transmission status
                Log.Information($"Transmitting packet {++counter} of {PacketCount}...");
                Log.Information($"Packet content: {encodedPacket}");

                // Convert byte[] to a hex array and then to a string and
                // pass as arguments
                start.Arguments = string.Format("{0} {1}",
                    "Scripts/transmit.py",
                    encodedPacket
                );

                try
                {
                    using (Process process = Process.Start(start))
                    {
                        // TODO: Waiting is necessary since sending
                        // everything at once may result in loss of data.
                        // Find a way to wait for successful result.
                        System.Threading.Thread.Sleep(200);
                    }
                }
                catch (Win32Exception ex)
                {
                    Console.WriteLine("An error has occurred.");
                    Log.Error("An error occurred while launching the Python process.");
                    Log.Error($"Executable name: {start.FileName}");
                    Log.Error($"{ex.NativeErrorCode}: {ex.Message}");
                }
            }
        }
    }
}