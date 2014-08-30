using System.Collections.Generic;
using System.IO;

namespace Dedox
{
    public interface IDedoxConfig
    {
        bool Verbose { get; set; }

        bool Metrics { get; set; }

        string OutputDirectory { get; set; }

        int LevenshteinLimit { get; set; }

        TextWriter Writer { get; set; }

        List<FileInfo> GetInputFiles();
    }
}