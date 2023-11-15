namespace LzarcTool
{
    public class FileEntry
    {
        private string fileName;
        private uint dataPos;
        private uint compressedSize;
        private uint alignedPos;
        private uint ukn;
        private uint decompressedSize;
        private byte[] fileData;

        public FileEntry()
        {
            this.fileName = "";
            this.dataPos = 0;
            this.compressedSize = 0;
            this.alignedPos = 0;
            this.ukn = 0;
            this.decompressedSize = 0;
            this.fileData = new byte[] {};
        }

        public string FileName {
            get => this.fileName;
        }
        public uint DataPos {
            get => this.dataPos;
        }
        public uint CompressedSize {
            get => this.compressedSize;
        }
        public uint AlignedPos {
            get => this.alignedPos;
        }
        public uint Ukn {
            get => this.ukn;
        }
        public uint DecompressedSize {
            get => this.decompressedSize;
        }
        public byte[] FileData {
            get => this.fileData;
        }

        public void Read(BigEndianBinaryReader reader)
        {
            long pos = reader.BaseStream.Position;
            this.fileName = reader.ReadString();
            reader.BaseStream.Seek(pos + 128, SeekOrigin.Begin);
            this.dataPos = reader.ReadUInt32();
            this.compressedSize = reader.ReadUInt32();
            this.alignedPos = reader.ReadUInt32();
            this.ukn = reader.ReadUInt32();
            this.decompressedSize = reader.ReadUInt32();

            pos = reader.BaseStream.Position;
            reader.BaseStream.Seek(this.dataPos, SeekOrigin.Begin);
            reader.BaseStream.Seek(8, SeekOrigin.Current);
            this.fileData = reader.ReadBytes((int)this.compressedSize - 8);
            this.fileData = LZ77.Decompress(this.fileData, (int)this.decompressedSize);
            reader.BaseStream.Seek(pos, SeekOrigin.Begin);
        }
    }
}
