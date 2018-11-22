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

            if (transmission.PacketCount == 0)
            {
                return -1;
            }

            if (opts.Transmit)
            {
                Log.Information("Starting the transmission...");

                transmission.Transmit();
            }

            return 0;
        }
    }
}