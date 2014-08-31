using System.IO;

namespace Dedox
{
    public class DebugConsoleWriter : IConsoleWriter
    {
        private readonly TextWriter _writer;

        public DebugConsoleWriter(TextWriter writer)
        {
            _writer = writer;
        }

        public void Debug()
        {
            _writer.WriteLine();
        }

        public void Debug(string format, params object[] args)
        {
            _writer.WriteLine(format, args);
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