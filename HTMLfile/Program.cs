using System;
using System.Data.SqlTypes;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using HTMLfile;
using HTMLfile.CustomFeatures;

class Program
{
    static void Main()
    {
        Console.Write("FilePath is: \n \"C:\\Users\\Milena\\Desktop\\Uni\\HTMLfile - 471223028\\HTML_Crawler.txt\"\n");
        string filePath = @"C:\Users\Milena\Desktop\Uni\HTMLfile - 471223028\HTML_Crawler.txt";
        string htmlContent = File.ReadAllText(filePath);
        StrFeatures StrF = new StrFeatures(htmlContent);


        if (!HTMLvalidator.ValidateHTML(htmlContent))
        {
            Console.WriteLine("Invalid HTML content.");
            return;
        }

        if (File.Exists(filePath))
        {
            HTMLparser parser = new HTMLparser(StrF);
            HTMLelement root = parser.Parse();
            HTMLsearchpath searcher = new HTMLsearchpath();

            Console.WriteLine("\nTree Structure:");
            HTMLsearchpath.PrintTree(root, 0);



            while (true)
            {
                Console.WriteLine("\nWrite a command(PRINT or SET or COPY//path):");
                string? commandInput = Console.ReadLine();
                StrFeatures command = new StrFeatures(commandInput);
                if (!string.IsNullOrEmpty(commandInput))
                    if (command.FindFirstOccurrence("PRINT") == 0)
                    {
                        if (command.Length() < 7)
                            Console.WriteLine("Invalid PRINT command.");
                        else
                            if (command.FindLastOccurrence("//") == command.Length() - 2)
                        {
                            HTMLsearchpath.PrintPath(root, "html");
                        }
                        else
                        {
                            string path = command.Substring(7);
                            //HTMLelement foundElement = HTMLsearchpath.ParallelSearch(root, path);
                            //HTMLsearchpath.PrintTree(foundElement, 0);
                            if(HTMLvalidator.ValidatePath(path))
                                HTMLsearchpath.PrintPath(root, path);
                            else Console.WriteLine("Invalid path format.");
                        }
                    }
                    else if (command.FindFirstOccurrence("SET") == 0)
                    {
                        if (command.Length() < 9)
                            Console.WriteLine("Invalid SET command.");
                        else
                        {
                            string[] parts = command.Substring(5, command.Length() - 1).Split(" \"");
                            if (parts.Length == 2)
                            {
                                string path = parts[0];
                                string newContent = parts[1];
                                if (!HTMLvalidator.ValidatePath(path))
                                {
                                    Console.WriteLine("Invalid path format.");
                                    return;
                                }
                                else HTMLmodifier.SetPath(root, path, newContent);
                            }
                        }
                    }
                    else if (command.FindFirstOccurrence("COPY") == 0)
                    {
                        string[] parts = command.Split(' ');
                        if (parts.Length == 2)
                        {
                            StrFeatures sf = new StrFeatures(parts[0]);
                            string[] CopiedPath = sf.Split("//");
                            sf = new StrFeatures(parts[1]);
                            CopiedPath[0] = sf.Substring(6, 6);
                            string path = sf.Substring(2, sf.Length());
                            HTMLsearchpath.CopyPath(root, CopiedPath[1], path);
                        }
                        else
                        {
                            Console.WriteLine("Invalid COPY command.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid command.");
                    }
            }
        }
        else
        {
            Console.WriteLine($"Error: The file at '{filePath}' was not found.");
        }
    }
}
