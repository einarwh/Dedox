using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Roslyn.Compilers.CSharp;

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
            var text = args.Any() ? File.ReadAllText(args[0]) : GetSampleProgram();
            Dedoxifier.Run(text);
        }
    }
}
