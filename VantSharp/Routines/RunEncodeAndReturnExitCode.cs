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

            Transmission transmission = new Transmission(opts.InputFile);
            Log.Information($"Your transmission contains {transmission.PacketCount} packets");

            // If transmit flag was passed in, initiate the trasmission by
            // calling the Python script responsible for it
            // if (opts.Transmit)
            // {
            //     Log.Information("Starting the transmission...");

            //     ProcessStartInfo start = new ProcessStartInfo()
            //     {
            //         FileName = "python3",
            //         UseShellExecute = false,
            //         RedirectStandardOutput = true
            //     };

            //     // Iterate over packets and trasmit each one
            //     for (int i = 1; i <= transmission.PacketCount; i++)
            //     {
            //         // Convert byte[] to a hex array and then to a string and
            //         // pass as arguments
            //         start.Arguments = string.Format("{0} {1}",
            //             "Scripts/transmit.py",
            //             transmission.Packets[i - 1].Encode()
            //         );

            //         // Log packet transmission status
            //         Log.Information($"Transmitting packet {i} of {transmission.PacketCount}...");
            //         Log.Information($"Packet content: {transmission.Packets[i - 1].Encode()}");

            //         try
            //         {
            //             using (Process process = Process.Start(start))
            //             {   
            //                 // TODO: Waiting is necessary since sending
            //                 // everything at once may result in loss of data.
            //                 // Find a way to wait for successful result.
            //                 System.Threading.Thread.Sleep(200);
            //             }
            //         }
            //         catch (Win32Exception ex)
            //         {
            //             Console.WriteLine("An error has occurred.");
            //             Log.Error("An error occurred while launching the Python process.");
            //             Log.Error($"Executable name: {start.FileName}");
            //             Log.Error($"{ex.NativeErrorCode}: {ex.Message}");
            //         }
            //     }
            // }

            return 0;
        }
    }
}