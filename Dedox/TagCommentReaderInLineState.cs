using Roslyn.Compilers.CSharp;

namespace Dedox
{
    internal class TagCommentReaderInLineState : TagCommentReaderState
    {
        public TagCommentReaderInLineState(TagCommentReaderStateMachine stateMachine)
            : base(stateMachine)
        {
        }

        protected override ITagCommentReaderState AcceptXmlText(XmlTextSyntax syntax)
        {
            StateMachine.AddText(ReadText(syntax));
            return new TagCommentReaderNewLineState(StateMachine);
        }

        protected override ITagCommentReaderState AcceptXmlEmptyElement(XmlEmptyElementSyntax syntax)
        {
            StateMachine.AddText(syntax.ToString());
            return this;
        }
    }
}