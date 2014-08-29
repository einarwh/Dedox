namespace Dedox
{
    public interface IArgumentReaderState
    {
        IArgumentReaderState Accept(string arg);
    }
}