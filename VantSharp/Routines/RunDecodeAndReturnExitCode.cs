using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using VantSharp.Configuration;
using VantSharp.Models;

namespace VantSharp.Routines
{
    public static class RunDecodeAndReturnExitCode
    {
        public static int Execute(DecodeOptions opts)
        {
            string filePath = opts.InputFile;
            if (File.Exists(filePath))
            {
                string transmissionFile = File.ReadAllText(filePath);
                byte[] transmissionBytes = StringToByteArray(transmissionFile);

                Transmission transmission = new Transmission();
                transmission.Decode(transmissionBytes);

                List<byte> data = new List<byte>();
                foreach (var packet in transmission.Packets)
                {
                    if (packet.IsFirstPacket)
                        continue;

                    data.AddRange(packet.Payload);
                }

                File.WriteAllBytes(opts.OutputFile ?? "output.png", data.ToArray());
            }

            return -1;
        }

        private static byte[] StringToByteArray(string hex) {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
    }
}