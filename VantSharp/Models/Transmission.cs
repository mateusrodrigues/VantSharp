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
            TimeSpan totalEncodingElapsedTime = new TimeSpan(0);
            TimeSpan totalPythonElapsedTime = new TimeSpan(0);
            foreach (var packet in Packets)
            {
                // Start timer count
                var startTime = DateTime.Now;
                var encodedPacket = packet.Encode();
                var endTime = DateTime.Now;
                Log.Information($"Packet {counter} took {endTime - startTime} to encode.");
                totalEncodingElapsedTime += (endTime - startTime);

                // Log packet transmission status
                Log.Information($"Transmitting packet {++counter} of {PacketCount}...");
                // Log.Information($"Packet content: {encodedPacket}");

                // Convert byte[] to a hex array and then to a string and
                // pass as arguments
                start.Arguments = string.Format("{0} {1}",
                    "Scripts/transmit.py",
                    encodedPacket
                );

                try
                {
                    startTime = DateTime.Now;
                    using (Process process = Process.Start(start))
                    {
                        process.WaitForExit();
                        // Finish timer count
                        endTime = DateTime.Now;
                        Log.Information($"Packet {counter} took {endTime - startTime} to transmit.");
                        totalPythonElapsedTime += (endTime - startTime);
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

            Log.Information($"The total encoding time was {totalEncodingElapsedTime}.");
            Log.Information($"The total transmission time was {totalPythonElapsedTime}.");
        }

        public void Decode(byte[] transmission)
        {
            // Decode first packet
            int currentIndex = 0;
            List<byte> bytes = new List<byte>(transmission);

            Packet firstPacket = new Packet();

            firstPacket.Id = int.Parse(
                Encoding.ASCII.GetString(bytes.GetRange(currentIndex, Packet.ID_SIZE).ToArray())
            );
            currentIndex += Packet.ID_SIZE;
            firstPacket.Hash = Encoding.ASCII.GetString(
                bytes.GetRange(currentIndex, Packet.HASH_SIZE).ToArray()
            );
            currentIndex += Packet.HASH_SIZE;
            firstPacket.Type = (PacketType) int.Parse(
                Encoding.ASCII.GetString(bytes.GetRange(currentIndex, Packet.TYPE_SIZE).ToArray())
            );
            currentIndex += Packet.TYPE_SIZE;
            firstPacket.Tag = int.Parse(
                Encoding.ASCII.GetString(bytes.GetRange(currentIndex, Packet.TAG_SIZE).ToArray())
            );
            currentIndex += Packet.TAG_SIZE;
            firstPacket.SourceNode = int.Parse(
                Encoding.ASCII.GetString(bytes.GetRange(currentIndex, Packet.SNO_SIZE).ToArray())
            );
            currentIndex += Packet.SNO_SIZE;
            firstPacket.DestinationNode = int.Parse(
                Encoding.ASCII.GetString(bytes.GetRange(currentIndex, Packet.DNO_SIZE).ToArray())
            );
            currentIndex += Packet.DNO_SIZE;
            firstPacket.Position = Encoding.ASCII.GetString(
                bytes.GetRange(currentIndex, Packet.POS_SIZE).ToArray()
            );
            currentIndex += Packet.POS_SIZE;
            firstPacket.Time = Encoding.ASCII.GetString(
                bytes.GetRange(currentIndex, Packet.TIME_SIZE).ToArray()
            );
            currentIndex += Packet.TIME_SIZE;
            firstPacket.LastIdentification = int.Parse(
                Encoding.ASCII.GetString(bytes.GetRange(currentIndex, Packet.LID_SIZE).ToArray())
            );
            currentIndex += Packet.LID_SIZE;

            firstPacket.IsFirstPacket = true;
            Packets.Add(firstPacket);

            for (int i = 0; i < firstPacket.LastIdentification; i++)
            {
                Packet packet = new Packet();
                
                packet.Id = int.Parse(
                    Encoding.ASCII.GetString(bytes.GetRange(currentIndex, Packet.ID_SIZE).ToArray())
                );
                currentIndex += Packet.ID_SIZE;
                packet.Tag = int.Parse(
                    Encoding.ASCII.GetString(bytes.GetRange(currentIndex, Packet.TAG_SIZE).ToArray())
                );
                currentIndex += Packet.TAG_SIZE;

                if (packet.Id == firstPacket.LastIdentification)
                {
                    int size = 1;
                    while (true)
                    {
                        try
                        {
                            packet.Payload = bytes.GetRange(currentIndex, size++).ToArray();
                        }
                        catch (Exception ex)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    packet.Payload = bytes.GetRange(currentIndex, Packet.PAYLOAD_SIZE).ToArray();
                    currentIndex += Packet.PAYLOAD_SIZE;
                }

                Packets.Add(packet);
            }
        }
    }
}