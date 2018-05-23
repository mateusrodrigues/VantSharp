using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;
using VantSharp.Configuration;
using VantSharp.Models;

namespace VantSharp
{
    class Program
    {
        static int Main(string[] args)
        {
            return CommandLine.Parser.Default.ParseArguments<EncodeOptions>(args)
                .MapResult(
                    (EncodeOptions opts) => RunEncodeAndReturnExitCode(opts),
                    errs => 1
                );
        }

        private static int RunEncodeAndReturnExitCode(EncodeOptions opts)
        {
            Console.WriteLine($"Reading from: {opts.InputFile}");

            using (Stream source = File.OpenRead(opts.InputFile))
            {
                // Gathering of packet information
                int bytesRead;
                int currentId = 1;
                byte[] buffer = new byte[Packet.PAYLOAD_SIZE];
                Transmission transmission = new Transmission();

                while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
                {
                    // Copy elements from buffer to new content array
                    // by 'bytesRead' amount to avoid junk content
                    // from previous iteration runs.
                    byte[] content = new byte[bytesRead];
                    Array.ConstrainedCopy(buffer, 0, content, 0, bytesRead);
                    // Create a new Packet object to store information gathered
                    // and add it to the transmission list.
                    Packet packet = new Packet
                    {
                        Id = currentId++,
                        Content = content
                    };
                    transmission.Packets.Add(packet);
                }

                Console.WriteLine($"Your transmission has {transmission.PacketCount} packets");
            }

            return 0;
        }
    }
}
