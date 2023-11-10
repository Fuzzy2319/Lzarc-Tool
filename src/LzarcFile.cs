namespace Lzarc_Tool
{
    public class LzarcFile
    {
        private const uint COMPRESSED_ALIGNEMENT = 64; // Compressed data is 64 bits aligned
        private const uint DECOMPRESSED_ALIGNEMENT = 8192; // Decompressed data is 8KiB aligned
        private const uint ENTRY_SIZE = 148; // Every file entry is 148 bits
        private const uint HEADER_SIZE = 12; // Header is 12 bits

        private List<FileEntry> files;
        public LzarcFile() {
            this.files = new List<FileEntry>();
        }

        public uint FileSize {
            get => (uint)this.files.Sum(file => this.CalcAlignedSize(file.CompressedSize, LzarcFile.COMPRESSED_ALIGNEMENT))
                + this.CalcAlignedSize(LzarcFile.HEADER_SIZE + (LzarcFile.ENTRY_SIZE * this.FileCount), LzarcFile.COMPRESSED_ALIGNEMENT);
        }
        public uint DecompressedSize {
            get => (uint)this.files.Sum(file => this.CalcAlignedSize(file.DecompressedSize + LzarcFile.DECOMPRESSED_ALIGNEMENT, LzarcFile.DECOMPRESSED_ALIGNEMENT))
                + LzarcFile.DECOMPRESSED_ALIGNEMENT;
        }
        public uint FileCount {
            get => (uint)this.files.Count;
        }
        public List<FileEntry> Files {
            get => this.files;
        }

        private uint CalcAlignedSize(uint len, uint alignement)
        {
            return (len + (alignement - 1) & ~(alignement - 1));
        }

        public void Read(BigEndianBinaryReader reader)
        {
            // Skip FileSize && DecompressedSize
            reader.BaseStream.Seek(sizeof(uint) * 2, SeekOrigin.Current);
            uint fileCount = reader.ReadUInt32();

            for (int i = 0; i < fileCount; i++)
            {
                FileEntry entry = new FileEntry();
                entry.Read(reader);
                this.files.Add(entry);
            }
        }
    }
}
