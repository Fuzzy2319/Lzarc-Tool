namespace LzarcTool.FileFormat
{
    public class LzarcFile
    {
        public const uint COMPRESSED_ALIGNMENT = 64; // Compressed data is 64 bytes aligned
        public const uint DECOMPRESSED_ALIGNMENT = 8192; // Decompressed data is 8KiB aligned
        public const uint ENTRY_SIZE = 148; // Every file entry is 148 bytes
        public const uint HEADER_SIZE = 12; // Header is 12 bytes

        public LzarcFile()
        {
            this.Files = new List<FileEntry>();
        }

        public uint FileSize
        {
            get => (uint)this.Files.Sum(file => CalcAlignedSize(file.CompressedSize, LzarcFile.COMPRESSED_ALIGNMENT))
                + CalcAlignedSize(HEADER_SIZE + ENTRY_SIZE * this.FileCount, LzarcFile.COMPRESSED_ALIGNMENT);
        }

        public uint DecompressedSize
        {
            get => (uint)this.Files.Sum(file => CalcAlignedSize(file.DecompressedSize + LzarcFile.DECOMPRESSED_ALIGNMENT, LzarcFile.DECOMPRESSED_ALIGNMENT))
                + LzarcFile.DECOMPRESSED_ALIGNMENT;
        }

        public uint FileCount
        {
            get => (uint)this.Files.Count;
        }

        public List<FileEntry> Files { get; }

        public static uint CalcAlignedSize(uint len, uint alignment)
        {
            return len + (alignment - 1) & ~(alignment - 1);
        }
    }
}
