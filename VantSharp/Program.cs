using CommandLine;
using VantSharp.Configuration;
using VantSharp.Routines;

namespace VantSharp
{
    class Program
    {
        static int Main(string[] args)
        {
            return CommandLine.Parser.Default.ParseArguments<EncodeOptions, InitOptions, KillOptions>(args)
                .MapResult(
                    (EncodeOptions opts) => RunEncodeAndReturnExitCode.Execute(opts),
                    (InitOptions opts)   => RunInitAndReturnExitCode.Execute(opts),
                    (KillOptions opts)   => RunKillAndReturnExitCode.Execute(opts),
                    errs => 1
                );
        }
    }
}
