namespace Lzarc_Tool
{
    public class LzarcFile
    {
        private uint fileSize;
        private uint alignedSize;
        private uint fileCount;
        private List<FileEntry> files;
        public LzarcFile() {
            this.files = new List<FileEntry>();
            this.fileSize = 0;
            this.alignedSize = 0;
            this.fileCount = 0;
        }

        public uint FileSize {
            get => this.fileSize;
        }
        public uint AlignedSize {
            get => this.alignedSize;
        }
        public uint FileCount {
            get => this.fileCount;
        }
        public List<FileEntry> Files {
            get => this.files;
        }

        public void Read(BigEndianBinaryReader reader)
        {
            this.fileSize = reader.ReadUInt32();
            this.alignedSize = reader.ReadUInt32();
            this.fileCount = reader.ReadUInt32();

            for (int i = 0; i < fileCount; i++)
            {
                FileEntry entry = new FileEntry();
                entry.Read(reader);
                this.files.Add(entry);
            }
        }
    }
}
