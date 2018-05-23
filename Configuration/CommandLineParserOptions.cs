using System;
using CommandLine;

namespace VantSharp.Configuration
{
    [Verb("encode", HelpText = "Encodes a file for hexadecimal transmission")]
    public class EncodeOptions
    {
        [Option('f', "file", Required = true, HelpText = "File to be encoded for transmission")]
        public string InputFile { get; set; }
    }
}