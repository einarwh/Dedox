using System.Text;

using Roslyn.Compilers.CSharp;

namespace Dedox
{
    public class TagCommentReaderStateMachine
    {
        private readonly StringBuilder _builder = new StringBuilder();

        private ITagCommentReaderState _state;

        public TagCommentReaderStateMachine()
        {
            _state = new TagCommentReaderStartState(this);
        }

        public void Accept(XmlNodeSyntax syntaxNode)
        {
            _state = _state.Accept(syntaxNode);
        }

        public void AddText(string text)
        {
            _builder.Append(text);
        }

        public override string ToString()
        {
            return _builder.ToString();
        }
    }
}