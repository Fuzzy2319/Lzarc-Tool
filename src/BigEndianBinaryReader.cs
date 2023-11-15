using System.Buffers.Binary;
using System.Text;

namespace LzarcTool
{
    public class BigEndianBinaryReader : BinaryReader
    {
        public BigEndianBinaryReader(Stream input) : base(input)
        {
        }

        public BigEndianBinaryReader(Stream input, Encoding encoding) : base(input, encoding)
        {
        }

        public BigEndianBinaryReader(Stream input, Encoding encoding, bool leaveOpen) : base(input, encoding, leaveOpen)
        {
        }

        public override uint ReadUInt32()
        {
            return BinaryPrimitives.ReadUInt32BigEndian(this.ReadBytes(sizeof(uint)));
        }

        public override string ReadString()
        {
            StringBuilder sb = new StringBuilder();
            char val = this.ReadChar();
            while (val != '\0')
            {
                sb.Append(val);
                val = this.ReadChar();
            }

            return sb.ToString();
        }
    }
}
