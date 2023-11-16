namespace LzarcTool.FileFormat
{
    public class FileEntry
    {
        private string fileName;
        private byte[] compressedFileData;
        private byte[] decompressedFileData;

        public FileEntry()
        {
            this.fileName = "";
            this.compressedFileData = Array.Empty<byte>();
            this.decompressedFileData = Array.Empty<byte>();
        }

        public string FileName
        {
            get => this.fileName;
            set => this.fileName = value;
        }

        public uint CompressedSize
        {
            get => (uint)this.compressedFileData.Length;
        }

        public uint DecompressedSize
        {
            get => (uint)this.decompressedFileData.Length;
        }

        public byte[] CompressedFileData
        {
            get => this.compressedFileData;
            set => this.compressedFileData = value;
        }

        public byte[] DecompressedFileData
        {
            get => this.decompressedFileData;
            set => this.decompressedFileData = value;
        }
    }
}
