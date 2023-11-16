namespace LzarcTool.FileFormat
{
    public class LzarcFile
    {
        private const uint COMPRESSED_ALIGNEMENT = 64; // Compressed data is 64 bytes aligned
        private const uint DECOMPRESSED_ALIGNEMENT = 8192; // Decompressed data is 8KiB aligned
        private const uint ENTRY_SIZE = 148; // Every file entry is 148 bytes
        private const uint HEADER_SIZE = 12; // Header is 12 bytes

        private List<FileEntry> files;
        public LzarcFile()
        {
            this.files = new List<FileEntry>();
        }

        public uint FileSize
        {
            get => (uint)this.files.Sum(file => this.CalcAlignedSize(file.CompressedSize, COMPRESSED_ALIGNEMENT))
                + this.CalcAlignedSize(HEADER_SIZE + ENTRY_SIZE * this.FileCount, COMPRESSED_ALIGNEMENT);
        }
        public uint DecompressedSize
        {
            get => (uint)this.files.Sum(file => this.CalcAlignedSize(file.DecompressedSize + DECOMPRESSED_ALIGNEMENT, DECOMPRESSED_ALIGNEMENT))
                + DECOMPRESSED_ALIGNEMENT;
        }
        public uint FileCount
        {
            get => (uint)this.files.Count;
        }
        public List<FileEntry> Files
        {
            get => this.files;
        }

        private uint CalcAlignedSize(uint len, uint alignement)
        {
            return len + (alignement - 1) & ~(alignement - 1);
        }
    }
}
