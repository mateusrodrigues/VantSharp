using Serilog;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using VantSharp.Configuration;
using VantSharp.Models;

namespace VantSharp.Routines
{
    public static class RunEncodeAndReturnExitCode
    {
        public static int Execute(EncodeOptions opts)
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

                Log.Information($"Your transmission contains {transmission.PacketCount} packets");
            }

            // If transmit flag was passed in, initiate the trasmission by
            // calling the Python script responsible for it
            if (opts.Transmit)
            {
                Log.Information("Starting the transmission...");

                ProcessStartInfo start = new ProcessStartInfo()
                {
                    FileName = "python3",
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                };

                // Iterate over packets and trasmit each one
                for (int i = 1; i <= transmission.PacketCount; i++)
                {
                    // Convert byte[] to a hex array and then to a string and
                    // pass as arguments
                    start.Arguments = string.Format("{0} {1}",
                        "Scripts/transmit.py",
                        transmission.Packets[i - 1].Encode()
                    );

                    // Log packet transmission status
                    Log.Information($"Transmitting packet {i} of {transmission.PacketCount}...");

                    try
                    {
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
                    catch (Win32Exception ex)
                    {
                        Console.WriteLine("An error has occurred.");
                        Log.Error("An error occurred while launching the Python process.");
                        Log.Error($"Executable name: {start.FileName}");
                        Log.Error($"{ex.NativeErrorCode}: {ex.Message}");
                    }
                }
            }

            return 0;
        }
    }
}