namespace LzarcTool {
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
                case "list":
                    if (args.Length < 2) {
                        Console.WriteLine("Error: Not enough arguments");
                        Program.DisplayHelp();

                        return;
                    }
                    Program.ListFiles(args[1]);
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
            throw new NotImplementedException();
        }
    }
}
