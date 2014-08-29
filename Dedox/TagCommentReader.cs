using System;

using Roslyn.Compilers.CSharp;

namespace Dedox
{
    public class TagCommentReader
    {
        private readonly SyntaxList<XmlNodeSyntax> _content;

        public TagCommentReader(SyntaxList<XmlNodeSyntax> content)
        {
            _content = content;
        }

        public string Read()
        {
            var stateMachine = new TagCommentReaderStateMachine();
            foreach (var item in _content)
            {
                stateMachine.Accept(item);
            }

            var result = stateMachine.ToString();
            //Console.WriteLine("State machine built this (from XML comments): '{0}'", result);
            return result;
        }   
    }
}