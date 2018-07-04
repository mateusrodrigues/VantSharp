using System;
using System.Diagnostics;
using System.IO;
using VantSharp.Configuration;

namespace VantSharp.Routines
{
    public static class RunKillAndReturnExitCode
    {
        public static int Execute(KillOptions opts)
        {
            var _defaultColor = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Stopping the receive listener...");
            Console.ForegroundColor = _defaultColor;

            // Read pid from external file
            var pid = File.ReadAllText(".pid");

            // Kill process
            var process = Process.GetProcessById(int.Parse(pid));
            process.Kill();

            // Remove remaining external file
            File.Delete(".pid");

            return 0;
        }
    }
}