using AuroraLib.Compression.Algorithms;
using System.Buffers.Binary;

namespace LzarcTool.Compression
{
    public class LZ77
    {
        public static byte[] Decompress(byte[] input, int decompSize)
        {
            MemoryStream output = new MemoryStream(decompSize);

            LZ11.DecompressHeaderless(new MemoryStream(input), output, output.Capacity);

            return output.GetBuffer();
        }

        public static byte[] Compress(byte[] input)
        {
            // TODO implement LZ77 type 11 compression
            List<byte> output = new List<byte>();
            byte[] header = new byte[sizeof(uint)];

            BinaryPrimitives.WriteUInt32LittleEndian(header, (uint)input.Length);
            for (int i = header.Length - 1; i > 0; i--)
            {
                header[i] = header[i - 1];
            }
            header[0] = 19; // file magic 0x13

            output.AddRange(header);

            header[0] = 17; // file magic 0x11

            output.AddRange(header);

            MemoryStream buf = new MemoryStream();
            LZ11.CompressHeaderless(input, buf);

            output.AddRange(buf.GetBuffer());

            return output.ToArray();
        }
    }
}
