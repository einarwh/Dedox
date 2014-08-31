namespace Dedox
{
    public interface IConsoleWriter
    {
        void Debug();

        void Debug(string format, params object[] args);

        void Info();
             
        void Info(string format, params object[] args);
    }
}