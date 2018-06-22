using CommandLine;
using VantSharp.Configuration;
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
