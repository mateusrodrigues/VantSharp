using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;
using VantSharp.Configuration;

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
                // Read chunks of 256 bytes from the file
                // at a time and encode it to save somewhere else.
                byte[] buffer = new byte[256];
                List<byte> file = new List<byte>();
                int bytesRead;
                while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
                {
                    file.AddRange(buffer);
                }

                Console.WriteLine($"The file list is {file.Count} entries long");
            }

            return 0;
        }
    }
}
