using LzarcTool.Compression;
using LzarcTool.FileFormat;
using LzarcTool.IO;

namespace LzarcTool
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Program.DisplayHelp();

                return;
            }

            switch (args[0].Trim().ToLower())
            {
                case "-l":
                case "--list":
                    if (args.Length < 2)
                    {
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
                case "-p":
                case "--pack":
                    if (args.Length < 3)
                    {
                        Console.WriteLine("Error: Not enough arguments");
                        Program.DisplayHelp();

                        return;
                    }

                    Program.PackFiles(args[1], args[2]);
                    break;
                default:
                    Program.DisplayHelp();
                    break;
            }
        }

        private static void DisplayHelp()
        {
            Console.WriteLine("Usage: Lzarc_Tool [-l/--list] [-x/--extract] [-p/--pack] ...");
            Console.WriteLine();
            Console.WriteLine("-l/--list: list the files in given archive");
            Console.WriteLine("Ex: Lzarc_Tool -l ./Fld_AD_Town_map.lzarc");
            Console.WriteLine("Ex: Lzarc_Tool --list ./Fld_AD_Town_map.lzarc");
            Console.WriteLine();
            Console.WriteLine("-x/--extract: extract the files from an arc hive to the given directory");
            Console.WriteLine("Ex: Lzarc_Tool -x ./Fld_AD_Town_map.lzarc ./Fld_AD_Town_map/");
            Console.WriteLine("Ex: Lzarc_Tool --extract ./Fld_AD_Town_map.lzarc ./Fld_AD_Town_map/");
            Console.WriteLine();
            Console.WriteLine("-p/--pack: pack a directory into a new archive");
            Console.WriteLine("Ex: Lzarc_Tool -p /Fld_TN_PostOffice_map/ ./Fld_TN_PostOffice_map_repack.lzarc");
            Console.WriteLine("Ex: Lzarc_Tool --pack /Fld_TN_PostOffice_map/ ./Fld_TN_PostOffice_map_repack.lzarc");
        }

        private static void ListFiles(string filePath)
        {
            LzarcFile? lzarcFile = Program.ReadFromFile(filePath);

            if (lzarcFile == null)
            {
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

        private static void ExtractFiles(string filePath, string outDirectory)
        {
            LzarcFile? lzarcFile = Program.ReadFromFile(filePath);

            if (lzarcFile == null)
            {
                return;
            }

            Directory.CreateDirectory(outDirectory);

            foreach (FileEntry file in lzarcFile.Files)
            {
                Console.WriteLine($"Extracting: {file.FileName}...");
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

        private static void PackFiles(string directoryPath, string outFile)
        {
            LzarcFile? lzarcFile = Program.ReadFromDirectory(directoryPath);

            if (lzarcFile == null)
            {
                return;
            }

            lzarcFile.Files.Sort((file1, file2) => file1.CompressedSize.CompareTo(file2.CompressedSize));

            Stream stream = File.Create(outFile);
            BigEndianBinaryWriter writer = new BigEndianBinaryWriter(stream);

            writer.Write(lzarcFile.FileSize);
            writer.Write(lzarcFile.DecompressedSize);
            writer.Write(lzarcFile.FileCount);
            uint dataStartPos = LzarcFile.CalcAlignedSize(
                LzarcFile.HEADER_SIZE + (LzarcFile.ENTRY_SIZE * lzarcFile.FileCount),
                LzarcFile.COMPRESSED_ALIGNMENT
            );
            uint compressedDataPos = dataStartPos;
            uint decompressedDataPos = LzarcFile.DECOMPRESSED_ALIGNMENT;
            List<byte> data = new List<byte>();

            foreach (FileEntry entry in lzarcFile.Files)
            {
                Console.WriteLine($"Processing file: {entry.FileName}");
                writer.Write(entry.FileName);
                for (int i = 0; i < 128 - entry.FileName.Length; i++)
                {
                    writer.Write((byte)0);
                }

                writer.Write(compressedDataPos);
                writer.Write(entry.CompressedSize);
                writer.Write(decompressedDataPos);
                writer.Write(entry.DecompressedSize);
                writer.Write(entry.DecompressedSize);

                data.AddRange(entry.CompressedFileData);

                uint posPadding = LzarcFile.CalcAlignedSize(
                                      entry.CompressedSize,
                                      LzarcFile.COMPRESSED_ALIGNMENT
                                  ) -
                                  entry.CompressedSize;

                for (int i = 0; i < posPadding; i++)
                {
                    data.Add(0);
                }

                compressedDataPos += entry.CompressedSize + posPadding;
                decompressedDataPos += LzarcFile.CalcAlignedSize(
                    entry.DecompressedSize + LzarcFile.DECOMPRESSED_ALIGNMENT,
                    LzarcFile.DECOMPRESSED_ALIGNMENT
                );
            }

            uint sectionPadding = dataStartPos - (uint)writer.BaseStream.Position;
            for (int i = 0; i < sectionPadding; i++)
            {
                writer.Write((byte)0);
            }

            foreach (byte value in data)
            {
                writer.Write(value);
            }

            writer.Dispose();
        }

        private static LzarcFile? ReadFromFile(string filePath)
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
                entry.DecompressedFileData = Lz77.Decompress(entry.CompressedFileData, (int)decompressedSize);
                reader.BaseStream.Seek(pos, SeekOrigin.Begin);

                lzarcFile.Files.Add(entry);
            }

            reader.Dispose();

            return lzarcFile;
        }

        private static LzarcFile? ReadFromDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Console.WriteLine("Error: Specified directory doesn't exist");

                return null;
            }

            LzarcFile lzarcFile = new LzarcFile();
            string[] files = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories);
            List<Task> tasks = [];
            tasks.AddRange(
                files.Select(
                    file => Task.Factory.StartNew(f =>
                        {
                            FileEntry entry = new FileEntry
                            {
                                DecompressedFileData = File.ReadAllBytes((string)f!)
                            };

                            f = Path.GetRelativePath(directoryPath, (string)f!)
                                .Replace("\\", "/");
                            entry.FileName = (string)f;

                            Console.WriteLine($"Compressing file: {entry.FileName}...");
                            entry.CompressedFileData = Lz77.Compress(entry.DecompressedFileData);
                            Console.WriteLine($"Compression for: {entry.FileName} done");

                            lzarcFile.Files.Add(entry);
                        },
                        file
                    )
                )
            );

            Task.WaitAll(tasks.ToArray());
            Console.WriteLine("Compression: done");

            return lzarcFile;
        }
    }
}
