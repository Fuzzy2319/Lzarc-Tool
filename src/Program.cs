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
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Error: Specified file doesn't exist");

                return;
            }

            Stream stream = File.OpenRead(filePath);
            BigEndianBinaryReader reader = new BigEndianBinaryReader(stream);
            LzarcFile lzarcFile = new LzarcFile();
            lzarcFile.Read(reader);
            reader.Dispose();

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
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Error: Specified file doesn't exist");

                return;
            }

            Stream stream = File.OpenRead(filePath);
            BigEndianBinaryReader reader = new BigEndianBinaryReader(stream);
            LzarcFile lzarcFile = new LzarcFile();
            lzarcFile.Read(reader);
            reader.Dispose();

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
                File.WriteAllBytes(path, file.FileData);
            }

            Console.WriteLine("Extraction done.");
        }
    }
}
