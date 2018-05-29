using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using CommandLine;
using Serilog;
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
            // Configure Logger
            var loggerConfiguration = new LoggerConfiguration();
            loggerConfiguration.MinimumLevel.Debug();
            // If verbose flag was passed in, write output to console
            if (opts.Verbose)
                loggerConfiguration.WriteTo.Console();
            // If a log output file was passed in, write to that file
            if (!string.IsNullOrEmpty(opts.LogOutput)
                || !string.IsNullOrWhiteSpace(opts.LogOutput))
                loggerConfiguration.WriteTo.File($"{opts.LogOutput}.log");
            Log.Logger = loggerConfiguration.CreateLogger();

            Log.Information($"Reading from: {opts.InputFile}");

            Transmission transmission;
            using (Stream source = File.OpenRead(opts.InputFile))
            {
                // Gathering of packet information
                int bytesRead;
                int currentId = 1;
                byte[] buffer = new byte[Packet.PAYLOAD_SIZE];
                transmission = new Transmission();

                while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
                {
                    // Copy elements from buffer to new content array by
                    // 'bytesRead' amount to avoid junk content from previous
                    // iteration runs. This happens when previously allocated
                    // byte arrays of size 'x' are reused to store data of sizes
                    // smaller than 'x', so, the amount unused are kept with
                    // their original data which I refer to as "junk".
                    // Therefore, the solution used is to reallocate the byte
                    // array with the exact size it needs to have.
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

                Log.Information($"Your transmission has {transmission.PacketCount} packets");
            }

            // If transmit flag was passed in, initiate the trasmission by
            // calling the Python script responsible for it
            if (opts.Transmit)
            {
                Log.Information("Starting the transmission...");

                ProcessStartInfo start = new ProcessStartInfo();
                start.FileName = "python";
                start.Arguments = string.Format("{0}", "Scripts/transmit.py");
                start.UseShellExecute = false;
                start.RedirectStandardOutput = true;

                using (Process process = Process.Start(start))
                {
                    // TODO: Find a way to read output lines as they happen
                    using (StreamReader reader = process.StandardOutput)
                    {
                        while (!reader.EndOfStream)
                        {
                            Log.Information(reader.ReadLine());
                        }
                    }
                }
            }

            return 0;
        }
    }
}
