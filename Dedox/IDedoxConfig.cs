using System.Collections.Generic;
using System.IO;

namespace Dedox
{
    public interface IDedoxConfig
    {
        bool Verbose { get; set; }

        bool Metrics { get; set; }

        bool IncludeGeneratedCode { get; set; }

        string OutputDirectory { get; set; }

        int LevenshteinLimit { get; set; }

        IConsoleWriter Writer { get; set; }

        bool VeryVerbose { get; set; }

        List<FileInfo> GetInputFiles();
    }
}