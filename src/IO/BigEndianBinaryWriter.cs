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
            byte[] buffer = new byte[sizeof(uint)];
            BinaryPrimitives.WriteUInt32BigEndian(buffer, value);

            base.Write(buffer);
        }

        public override void Write(string value)
        {
            foreach (char val in value)
            {
                base.Write(val);
            }
        }
    }
}
