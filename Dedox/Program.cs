using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

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
            var config = new ArgumentReaderStateMachine();

            foreach (var a in args)
            {
                config.Accept(a);
            }

            Run(config);

            

            Console.ReadKey();
        }

        private static void Run(ArgumentReaderStateMachine config)
        {
            Console.WriteLine("Verbose? " + config.Verbose);
            Console.WriteLine("Output dir? " + config.OutputDirectory);

            var tw = config.Verbose ? Console.Out : new StringWriter(new StringBuilder());

            var inputFiles = config.GetInputFiles();
            if (inputFiles.Any())
            {
                foreach (var inputFile in config.GetInputFiles())
                {
                    ProcessInputFile(inputFile, tw);
                }
            }
            else
            {
                var sampleOutput = new Dedoxifier(tw).Run(GetSampleProgram());
                tw.WriteLine(sampleOutput);
            }
        }

        private static void ProcessInputFile(FileInfo inputFile, TextWriter tw)
        {
            if (inputFile.Exists)
            {
                tw.WriteLine("Processing file: " + inputFile.Name);

                if (inputFile.Name.EndsWith(".csproj"))
                {
                    AnalyzeProject(inputFile, tw);
                }
                else if (inputFile.Name.EndsWith(".cs"))
                {
                    var output = new Dedoxifier(tw).Run(File.ReadAllText(inputFile.FullName));
                    tw.WriteLine(output);
                }
            }
            else
            {
                Console.WriteLine("No such file: " + inputFile.FullName);
            }
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

        private static void AnalyzeProject(FileInfo projectFile, TextWriter tw)
        {
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
                var result = new Dedoxifier(tw).Run(File.ReadAllText(filePath));

                if (result != null)
                {
                    // Output dedox file. 
                    Console.WriteLine(result);
                }
            }
        }
    }
}
