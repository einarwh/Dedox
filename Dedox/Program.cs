using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Dedox
{
    internal class Program
    {
        private static string GetSampleProgram()
        {
            var text = @"
using System;
class C
{
    private readonly string _title;

    /// <summary>
    /// Gets or sets the first name.
    /// This happens to be a multiline comment.
    /// </summary>
    /// <value>
    /// The first name.
    /// </value>
    public string FirstName { get; set; }

    public string MiddleInitial { get; set; }

    /// <summary>
    /// Gets or sets the last name.
    /// </summary>
    /// <value>
    /// The last name.
    /// </value>
    public string LastName { get; set; }

    /// <summary>
    /// Gets the title.
    /// </summary>
    public string Title { get { return _title; } }

    /// <summary>
    /// The find address by id and date.
    /// </summary>
    /// <param name=""id"">
    /// The id.
    /// </param>
    /// <param name=""dateTime"">
    /// The date time.
    /// </param>
    /// <returns>
    /// The <see cref=""Address""/>.
    /// </returns>
    public Address FindAddressByIdAndDate(int id, DateTime dateTime)
    {
        return new Address();
    }

    /// <summary>
    /// The read word.
    /// </summary>
    /// <remarks> 
    /// This is a remark.
    /// </remarks>
    /// <returns>
    /// The <see cref=""string""/>.
    /// </returns>
    public string ReadWord()
    {
        return ""foof"";
    }

    static void M()
    {
        if (true)
            Console.WriteLine(""Hello, World!"");
    }
}";
            return text;
        }

        public static void Main(string[] args)
        {
            if (args.Any())
            {
                var fileInfo = new FileInfo(args[0]);
                if (fileInfo.Exists)
                {
                    if (fileInfo.Name.EndsWith(".csproj"))
                    {
                        AnalyzeProject(fileInfo);
                    }
                    else if (fileInfo.Name.EndsWith(".cs"))
                    {
                        var output = Dedoxifier.Run(File.ReadAllText(fileInfo.FullName));
                        Console.WriteLine(output);
                    }
                }
                else
                {
                    Console.WriteLine("No such file: " + fileInfo.FullName);
                }
            }
            else
            {
                var sampleOutput = Dedoxifier.Run(GetSampleProgram());
                Console.WriteLine(sampleOutput);
            }

            Console.ReadKey();
        }

        private static List<string> GetCodeFilesFromProjectFile(FileInfo projectFile)
        {
            var doc = XDocument.Load(projectFile.FullName);

            var files =
                doc.Elements()
                    .First()
                    .Descendants()
                    .Where(d => d.Name.LocalName == "Compile" && d.Attributes().Any(a => a.Name == "Include"))
                    .Select(a => a.Attribute("Include").Value)
                    .ToList();

            return files;
        } 

        private static void AnalyzeProject(FileInfo projectFile)
        {
            Console.WriteLine("Proceed to analyze all files.");

            var dir = projectFile.Directory;
            if (dir == null)
            {
                return;
            }

            var filePaths = GetCodeFilesFromProjectFile(projectFile)
                .Select(file => Path.Combine(dir.FullName, file))
                .ToList();

            foreach (var filePath in filePaths)
            {
                var result = Dedoxifier.Run(File.ReadAllText(filePath));

                if (result == null)
                {
                    Console.WriteLine("File OK.");
                }
                else
                {
                    Console.WriteLine(result);
                }
            }
        }
    }
}
