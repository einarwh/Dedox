namespace Dedox
{
    internal class LevenshteinLimitArgumentReaderState : IArgumentReaderState
    {
        private readonly ArgumentReaderStateMachine _machine;

        public LevenshteinLimitArgumentReaderState(ArgumentReaderStateMachine machine)
        {
            _machine = machine;
        }

        public IArgumentReaderState Accept(string arg)
        {
            var limit = int.Parse(arg);
            _machine.LevenshteinLimit = limit;

            return new ArgumentReaderState(_machine);
        }
    }
}