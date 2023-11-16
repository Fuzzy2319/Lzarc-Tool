using System.Buffers.Binary;
using System.Text;

namespace LzarcTool.IO
{
    public class BigEndianBinaryWriter : BinaryWriter
    {
        public BigEndianBinaryWriter(Stream input) : base(input)
        {
        }

        public BigEndianBinaryWriter(Stream input, Encoding encoding) : base(input, encoding)
        {
        }

        public BigEndianBinaryWriter(Stream input, Encoding encoding, bool leaveOpen) : base(input, encoding, leaveOpen)
        {
        }

        public override void Write(uint value)
        {
            Span<byte> buffer = new Span<byte>();
            BinaryPrimitives.WriteUInt32BigEndian(buffer, value);

            base.Write(buffer);
        }
    }
}
