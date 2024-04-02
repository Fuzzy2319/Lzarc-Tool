namespace LzarcTool.FileFormat
{
    public class FileEntry
    {
        public FileEntry()
        {
            this.FileName = "";
            this.CompressedFileData = Array.Empty<byte>();
            this.DecompressedFileData = Array.Empty<byte>();
        }

        public string FileName { get; set; }

        public uint CompressedSize
        {
            get => (uint)this.CompressedFileData.Length;
        }

        public uint DecompressedSize
        {
            get => (uint)this.DecompressedFileData.Length;
        }

        public byte[] CompressedFileData { get; set; }

        public byte[] DecompressedFileData { get; set; }
    }
}
