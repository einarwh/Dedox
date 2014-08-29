using System;

using Roslyn.Compilers.CSharp;

namespace Dedox
{
    internal class TagCommentReaderNewLineState : TagCommentReaderState
    {
        public TagCommentReaderNewLineState(TagCommentReaderStateMachine stateMachine)
            : base(stateMachine)
        {
        }

        protected override ITagCommentReaderState AcceptXmlText(XmlTextSyntax syntax)
        {
            StateMachine.AddText(Environment.NewLine);
            StateMachine.AddText(ReadText(syntax));
            return this;
        }

        protected override ITagCommentReaderState AcceptXmlEmptyElement(XmlEmptyElementSyntax syntax)
        {
            StateMachine.AddText(" ");
            StateMachine.AddText(syntax.ToString());
            return new TagCommentReaderInLineState(StateMachine);
        }
    }
}