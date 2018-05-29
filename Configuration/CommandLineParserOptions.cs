using System;
using CommandLine;

namespace VantSharp.Configuration
{
    [Verb("encode", HelpText = "Encodes a file for hexadecimal transmission.")]
    public class EncodeOptions
    {
        [Option('f', "file", Required = true, HelpText = "File to be encoded for transmission.")]
        public string InputFile { get; set; }

        [Option('t', "transmit", Required = false, HelpText = "Transmit packets right after encoding them.")]
        public bool Transmit { get; set; }

        [Option('v', "verbose", Required = false, HelpText = "Turn on verbose output.")]
        public bool Verbose { get; set; }

        [Option('l', "log", Required = false, HelpText = "Write logging output to a file."
            + "This file has a .log extension by design.")]
        public string LogOutput { get; set; }
    }
}