namespace LzarcTool.FileFormat
{
    public class FileEntry
    {
        private byte[] _compressedFileData;
        private byte[] _decompressedFileData;
        private string _fileName;

        public string FileName
        {
            get => this._fileName;
            set => this._fileName = value;
        }

        public uint CompressedSize => (uint)this._compressedFileData.Length;

        public uint DecompressedSize => (uint)this._decompressedFileData.Length;

        public byte[] CompressedFileData
        {
            get => this._compressedFileData;
            set => this._compressedFileData = value;
        }

        public byte[] DecompressedFileData
        {
            get => this._decompressedFileData;
            set => this._decompressedFileData = value;
        }

        public FileEntry()
        {
            this._fileName = "";
            this._compressedFileData = [];
            this._decompressedFileData = [];
        }
    }
}
