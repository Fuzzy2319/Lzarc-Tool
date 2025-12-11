using System.Collections.Generic;
using System.Linq;

namespace LzarcTool.FileFormat
{
    public class LzarcFile
    {
        public const uint COMPRESSED_ALIGNMENT = 64; // Compressed data is 64 bytes aligned
        public const uint DECOMPRESSED_ALIGNMENT = 8192; // Decompressed data is 8KiB aligned
        public const uint ENTRY_SIZE = 148; // Every file entry is 148 bytes
        public const uint HEADER_SIZE = 12; // Header is 12 bytes

        private readonly List<FileEntry> _files;

        public uint FileSize => (uint)this._files.Sum(file =>
                LzarcFile.CalcAlignedSize(file.CompressedSize, LzarcFile.COMPRESSED_ALIGNMENT)
            )
            + LzarcFile.CalcAlignedSize(LzarcFile.HEADER_SIZE + LzarcFile.ENTRY_SIZE * this.FileCount,
                LzarcFile.COMPRESSED_ALIGNMENT
            );

        public uint DecompressedSize => (uint)this._files.Sum(file =>
                LzarcFile.CalcAlignedSize(file.DecompressedSize + LzarcFile.DECOMPRESSED_ALIGNMENT,
                    LzarcFile.DECOMPRESSED_ALIGNMENT
                )
            )
            + LzarcFile.DECOMPRESSED_ALIGNMENT;

        public uint FileCount => (uint)this._files.Count;

        public List<FileEntry> Files => this._files;

        public LzarcFile()
        {
            this._files = [];
        }

        public static uint CalcAlignedSize(uint len, uint alignment)
        {
            return len + (alignment - 1) & ~(alignment - 1);
        }
    }
}
