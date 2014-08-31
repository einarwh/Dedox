namespace Dedox
{
    public class NullConsoleWriter : IConsoleWriter
    {
        public virtual void Debug()
        {
        }

        public virtual void Debug(string format, params object[] args)
        {
        }

        public virtual void Info()
        {
        }

        public virtual void Info(string format, params object[] args)
        {
        }
    }
}