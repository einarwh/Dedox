using System;

namespace Dedox
{
    class ArgumentReaderState : IArgumentReaderState
    {
        private readonly ArgumentReaderStateMachine _machine;

        public ArgumentReaderState(ArgumentReaderStateMachine machine)
        {
            _machine = machine;
        }

        public IArgumentReaderState Accept(string arg)
        {
            if ("-v".Equals(arg))
            {
                Console.WriteLine("Flag verbose.");
                _machine.Verbose = true;
                return this;
            }

            if ("-o".Equals(arg))
            {
                Console.WriteLine("Ready to accept output dir.");
                return new SetOutputDirectoryArgumentReaderState(_machine);
            }

            _machine.AddInputFile(arg);

            return new InputFilesArgumentReaderState(_machine);
        }
    }
}