using AuroraLib.Compression.Algorithms;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace LzarcTool.Compression
{
    public static class Lz77
    {
        public static byte[] Decompress(byte[] input, int decompSize)
        {
            using MemoryStream output = new MemoryStream(decompSize);
            using MemoryStream buf = new MemoryStream(input);

            LZ11.DecompressHeaderless(buf, output, (uint)output.Capacity);

            return output.GetBuffer()[..(int)output.Length];
        }

        public static byte[] Compress(byte[] input)
        {
            List<byte> output = [];
            Span<byte> header = stackalloc byte[sizeof(uint)];

            BinaryPrimitives.WriteUInt32LittleEndian(header, (uint)input.Length);
            for (int i = header.Length - 1; i > 0; i--)
            {
                header[i] = header[i - 1];
            }
            header[0] = 0x13; // file magic 0x13

            output.AddRange(header);

            header[0] = 0x11; // file magic 0x11

            output.AddRange(header);

            using MemoryStream buf = new MemoryStream();
            LZ11.CompressHeaderless(input, buf, true, CompressionLevel.SmallestSize);

            output.AddRange(buf.GetBuffer()[..(int)buf.Length]);

            return output.ToArray();
        }
    }
}
