namespace Dedox
{
    internal class InputFilesArgumentReaderState : IArgumentReaderState
    {
        private readonly ArgumentReaderStateMachine _machine;

        public InputFilesArgumentReaderState(ArgumentReaderStateMachine machine)
        {
            _machine = machine;
        }

        public IArgumentReaderState Accept(string arg)
        {
            _machine.AddInputFile(arg);

            return this;
        }
    }
}