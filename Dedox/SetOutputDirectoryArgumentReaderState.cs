using System;
using System.IO;

namespace Dedox
{
    internal class SetOutputDirectoryArgumentReaderState : IArgumentReaderState
    {
        private readonly ArgumentReaderStateMachine _machine;

        public SetOutputDirectoryArgumentReaderState(ArgumentReaderStateMachine machine)
        {
            _machine = machine;
        }

        public IArgumentReaderState Accept(string arg)
        {
            Console.WriteLine("Output dir: " + arg);

            var dir = new DirectoryInfo(arg);
            if (!dir.Exists)
            {
                throw new FileNotFoundException("No such directory.");
            }

            _machine.OutputDirectory = dir.FullName;

            return new ArgumentReaderState(_machine);
        }
    }
}