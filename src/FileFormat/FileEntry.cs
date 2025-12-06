namespace LzarcTool.FileFormat
{
    public class FileEntry
    {
        public string FileName { get; set; }

        public uint CompressedSize => (uint)this.CompressedFileData.Length;

        public uint DecompressedSize => (uint)this.DecompressedFileData.Length;

        public byte[] CompressedFileData { get; set; }

        public byte[] DecompressedFileData { get; set; }

        public FileEntry()
        {
            this.FileName = "";
            this.CompressedFileData = [];
            this.DecompressedFileData = [];
        }
    }
}
