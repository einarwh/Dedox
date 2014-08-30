using System.Collections.Generic;
using System.IO;

namespace Dedox
{
    public interface IDedoxConfig
    {
        bool Verbose { get; set; }

        string OutputDirectory { get; set; }

        TextWriter Writer { get; set; }

        List<FileInfo> GetInputFiles();
    }
}