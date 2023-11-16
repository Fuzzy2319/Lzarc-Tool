using LzarcTool.Compressor;
using LzarcTool.FileFormat;
using LzarcTool.IO;

namespace LzarcTool
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Program.DisplayHelp();

                return;
            }
            switch (args[0].Trim().ToLower()) {
                case "-l":
                case "--list":
                    if (args.Length < 2) {
                        Console.WriteLine("Error: Not enough arguments");
                        Program.DisplayHelp();

                        return;
                    }
                    Program.ListFiles(args[1]);
                    break;
                case "-x":
                case "--extract":
                    if (args.Length < 3)
                    {
                        Console.WriteLine("Error: Not enough arguments");
                        Program.DisplayHelp();

                        return;
                    }
                    Program.ExtractFiles(args[1], args[2]);
                    break;
                default:
                    Program.DisplayHelp();
                    break;
            }
        }

        static void DisplayHelp()
        {
            throw new NotImplementedException();
        }

        static void ListFiles(string filePath)
        {
            LzarcFile? lzarcFile = Program.ReadFromFile(filePath);

            if (lzarcFile == null) {
                return;
            }

            Console.WriteLine($"Found: {lzarcFile.FileCount} file(s) in this archive.");
            foreach (FileEntry file in lzarcFile.Files)
            {
                Console.WriteLine();
                Console.WriteLine($"File name: {file.FileName}");
                Console.WriteLine($"File compressed size: {file.CompressedSize}");
                Console.WriteLine($"File decompressed size: {file.DecompressedSize}");
            }
        }

        static void ExtractFiles(string filePath, string outDirectory) {
            LzarcFile? lzarcFile = Program.ReadFromFile(filePath);

            if (lzarcFile == null)
            {
                return;
            }

            if (!Directory.Exists(outDirectory))
            {
                Directory.CreateDirectory(outDirectory);
            }

            foreach (FileEntry file in lzarcFile.Files)
            {
                Console.WriteLine($"Extracting {file.FileName}...");
                string path = Path.Combine(outDirectory, file.FileName);
                string? dir = Path.GetDirectoryName(path);
                if (dir != null && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                File.WriteAllBytes(path, file.DecompressedFileData);
            }

            Console.WriteLine("Extraction done.");
        }

        static LzarcFile? ReadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Error: Specified file doesn't exist");

                return null;
            }

            Stream stream = File.OpenRead(filePath);
            BigEndianBinaryReader reader = new BigEndianBinaryReader(stream);
            LzarcFile lzarcFile = new LzarcFile();

            // Skip FileSize && DecompressedSize
            reader.BaseStream.Seek(sizeof(uint) * 2, SeekOrigin.Current);
            uint fileCount = reader.ReadUInt32();

            for (int i = 0; i < fileCount; i++)
            {
                FileEntry entry = new FileEntry();

                long pos = reader.BaseStream.Position;
                entry.FileName = reader.ReadString();
                reader.BaseStream.Seek(pos + 128, SeekOrigin.Begin);
                uint dataPos = reader.ReadUInt32();
                uint compressedDataSize = reader.ReadUInt32();
                //uint decompressedPos = reader.ReadUInt32();
                //uint ukn = reader.ReadUInt32();
                // Skip DecompressedPos && Ukn
                reader.BaseStream.Seek(sizeof(uint) * 2, SeekOrigin.Current);
                uint decompressedSize = reader.ReadUInt32();

                pos = reader.BaseStream.Position;
                reader.BaseStream.Seek(dataPos, SeekOrigin.Begin);
                reader.BaseStream.Seek(8, SeekOrigin.Current);
                entry.CompressedFileData = reader.ReadBytes((int)compressedDataSize - 8);
                entry.DecompressedFileData = LZ77.Decompress(entry.CompressedFileData, (int)decompressedSize);
                reader.BaseStream.Seek(pos, SeekOrigin.Begin);

                lzarcFile.Files.Add(entry);
            }

            reader.Dispose();

            return lzarcFile;
        }
    }
}
