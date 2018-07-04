using System;
using System.Diagnostics;
using System.IO;
using VantSharp.Configuration;

namespace VantSharp.Routines
{
    public static class RunInitAndReturnExitCode
    {
        public static int Execute(InitOptions opts)
        {
            var _defaultColor = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Starting up the receive listener...");
            Console.ForegroundColor = _defaultColor;

            // Start the receive script
            ProcessStartInfo start = new ProcessStartInfo()
            {
                FileName = "python3",
                UseShellExecute = false,
                RedirectStandardOutput = false,
                Arguments = "Scripts/receive.py"
            };

            using (Process process = Process.Start(start))
            {
                // Save process pid to external file for later kill
                File.WriteAllTextAsync(".pid", process.Id.ToString()).Wait();
            }

            return 0;
        }
    }
}