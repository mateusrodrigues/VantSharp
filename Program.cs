using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using CommandLine;
using Serilog;
using VantSharp.Configuration;
using VantSharp.Models;
using VantSharp.Routines;

namespace VantSharp
{
    class Program
    {
        static int Main(string[] args)
        {
            return CommandLine.Parser.Default.ParseArguments<EncodeOptions>(args)
                .MapResult(
                    (EncodeOptions opts) => RunEncodeAndReturnExitCode.Execute(opts),
                    errs => 1
                );
        }
    }
}
