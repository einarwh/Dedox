using System.IO;

namespace Dedox
{
    public class InfoConsoleWriter : IConsoleWriter
    {
        private readonly TextWriter _writer;

        public InfoConsoleWriter(TextWriter writer)
        {
            _writer = writer;
        }

        public void Debug()
        {
        }

        public void Debug(string format, params object[] args)
        {
        }

        public void Info()
        {
            _writer.WriteLine();
        }

        public void Info(string format, params object[] args)
        {
            _writer.WriteLine(format, args);
        }
    }
}