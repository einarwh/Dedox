using Roslyn.Compilers.CSharp;

namespace Dedox
{
    public interface ITagCommentReaderState
    {
        ITagCommentReaderState Accept(XmlNodeSyntax syntax);
    }
}