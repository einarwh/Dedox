using System.Linq;

using Roslyn.Compilers.CSharp;

namespace Dedox
{
    using System;

    abstract class TagCommentReaderState : ITagCommentReaderState
    {
        protected readonly TagCommentReaderStateMachine StateMachine;

        protected TagCommentReaderState(TagCommentReaderStateMachine stateMachine)
        {
            StateMachine = stateMachine;
        }

        public ITagCommentReaderState Accept(XmlNodeSyntax syntax)
        {
            // Console.WriteLine(GetType() + ": " + syntax.Kind);
            if (syntax.Kind == SyntaxKind.XmlText)
            {
                return AcceptXmlText((XmlTextSyntax)syntax);
            }

            if (syntax.Kind == SyntaxKind.XmlEmptyElement)
            {
                return AcceptXmlEmptyElement((XmlEmptyElementSyntax)syntax);
            }

            StateMachine.AddText(syntax.ToString());
            return new TagCommentReaderInLineState(StateMachine);
        }

        protected string ReadText(XmlTextSyntax textItem)
        {
            var texts = textItem.TextTokens.Select(t => t.ValueText.Trim()).Where(txt => !string.IsNullOrEmpty(txt));
            return string.Join(Environment.NewLine, texts.ToList());
        }

        protected abstract ITagCommentReaderState AcceptXmlText(XmlTextSyntax syntax);

        protected abstract ITagCommentReaderState AcceptXmlEmptyElement(XmlEmptyElementSyntax syntax);
    }
}