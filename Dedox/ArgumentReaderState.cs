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
                _machine.Verbose = true;
                return this;
            }

            if ("-m".Equals(arg))
            {
                _machine.Metrics = true;
                return this;
            }

            if ("-l".Equals(arg))
            {
                return new LevenshteinLimitArgumentReaderState(_machine);
            }

            if ("-o".Equals(arg))
            {
                return new SetOutputDirectoryArgumentReaderState(_machine);
            }

            _machine.AddInputFile(arg);

            return new InputFilesArgumentReaderState(_machine);
        }
    }
}