using Roslyn.Compilers.CSharp;

namespace Dedox
{
    internal class TagCommentReaderStartState : TagCommentReaderState
    {
        public TagCommentReaderStartState(TagCommentReaderStateMachine stateMachine)
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
            return new TagCommentReaderInLineState(StateMachine);
        }
    }
}