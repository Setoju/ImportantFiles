using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ImportantFileManager
{
    static class ImportantFileManager
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: [command] [filePath] [options]");
                return;
            }

            string command = args[0];
            string filePath = args[1];

            switch (command.ToLower())
            {
                case "mark":
                    MarkAsImportant(filePath);
                    break;
                case "unmark":
                    UnmarkAsImportant(filePath);
                    break;
                case "find":
                    string directory = args.Contains("--dir") ? args[Array.IndexOf(args, "--dir") + 1] : Directory.GetCurrentDirectory();
                    string extension = args.Contains("--ext") ? args[Array.IndexOf(args, "--ext") + 1] : null;
                    string nameContains = args.Contains("--name-contains") ? args[Array.IndexOf(args, "--name-contains") + 1] : null;
                    SearchImportantFiles(directory, extension, nameContains);
                    break;
                default:
                    Console.WriteLine("Usage: [command] [filePath] [options]");
                    break;
            }
        }

        static void MarkAsImportant(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Error: File {filePath} does not exist.");
                return;
            }

            FileAttributes attributes = File.GetAttributes(filePath);

            if ((attributes & FileAttributes.Archive) == 0)
            {
                File.SetAttributes(filePath, attributes | FileAttributes.Archive);
                Console.WriteLine($"File {filePath} has been marked as important.");
            }
            else
            {
                Console.WriteLine($"File {filePath} is already marked as important.");
            }
        }

        static void UnmarkAsImportant(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Error: File {filePath} does not exist.");
                return;
            }

            FileAttributes attributes = File.GetAttributes(filePath);

            if ((attributes & FileAttributes.Archive) != 0)
            {
                File.SetAttributes(filePath, attributes & ~FileAttributes.Archive); //~ інверсія бітів, що вимикає атрибут, якщо біт 0 то стане 1
                Console.WriteLine($"File {filePath} has been unmarked as important.");
            }
            else
            {
                Console.WriteLine($"File {filePath} is not marked as important.");
            }
        }

        static void SearchImportantFiles(string directory, string extension, string nameContains)
        {
            var importantFiles = Directory.EnumerateFiles(directory, "*", SearchOption.AllDirectories)
                .Where(file => (File.GetAttributes(file) & FileAttributes.Archive) != 0 &&
                               (extension == null || file.EndsWith($".{extension}", StringComparison.OrdinalIgnoreCase)) &&
                               (nameContains == null || Path.GetFileName(file).Contains(nameContains, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            if (importantFiles.Count == 0)
            {
                Console.WriteLine("No important files found matching the criteria.");
            }
            else
            {
                foreach (var file in importantFiles)
                {
                    Console.WriteLine(Path.GetFullPath(file));
                }
            }
        }
    }
}