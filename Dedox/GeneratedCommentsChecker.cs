using System;
using System.Linq;
using System.Text.RegularExpressions;

using Roslyn.Compilers.CSharp;

namespace Dedox
{
    abstract class GeneratedCommentsChecker<T> : IGeneratedCommentsChecker where T : SyntaxNode
    {
        protected readonly T It;

        protected GeneratedCommentsChecker(T it)
        {
            It = it;
        }

        protected abstract string Name
        {
            get;
        }

        protected string GetCodeElementType()
        {
            var s = It.Kind.ToString();
            return s.Substring(0, s.IndexOf("Declaration", StringComparison.Ordinal));
        }

        protected SyntaxTrivia GetDocumentationTrivia()
        {
            return It.GetLeadingTrivia().FirstOrDefault(t => t.Kind == SyntaxKind.DocumentationCommentTrivia);
        }

        protected string ReadTagComment(SyntaxList<XmlNodeSyntax> content)
        {
            var reader = new TagCommentReader(content);
            return reader.Read();
        }

        public bool IsGenerated()
        {
            Console.WriteLine();
            var elemType = GetCodeElementType();
            Console.WriteLine("{0}: {1}", elemType, Name);

            var docTrivia = GetDocumentationTrivia();
            var st = docTrivia.GetStructure();
            if (st == null)
            {
                // No XML comments.
                Console.WriteLine("{0} {1} has no XML comments.", elemType, Name);
                return false;
            }

            var childNodes = st.ChildNodes();
            var maybeXmlElements = childNodes.Where(n => n.Kind == SyntaxKind.XmlElement);
            var xmlElements = maybeXmlElements.Cast<XmlElementSyntax>();

            foreach (XmlElementSyntax e in xmlElements)
            {
                XmlElementStartTagSyntax startTag = e.StartTag;
                string tag = startTag.Name.LocalName.ValueText;

                // Particular
                var expectedComment = GetExpectedCommentForTag(startTag);

                if (expectedComment == null)
                {
                    Console.WriteLine("{0} {1}: Failed to produce an expectation for tag {2}.", elemType, Name, tag);
                    return false;
                }

                string actualComment = ReadTagComment(e.Content);

                if (!string.Equals(expectedComment, actualComment, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Comment mismatch.");
                    Console.WriteLine("Expected comment: '{0}'", expectedComment);
                    Console.WriteLine("Actual comment: '{0}'", actualComment);

                    var dist = LevenshteinDistance.Compute(expectedComment, actualComment);
                    Console.WriteLine("Levenshtein distance: " + dist);

                    OnMismatch(tag, expectedComment, actualComment);

                    return false;
                }
            }

            Console.WriteLine("All the documentation for {0} {1} was written by a tool.", elemType, Name);

            return true;
        }

        protected virtual void OnMismatch(string tag, string expectedComment, string actualComment)
        {
        }

        protected abstract string GetExpectedCommentForTag(XmlElementStartTagSyntax startTag);

        protected string SplitCamelCase(string input)
        {
            // Better expression: "(?<=[a-z])([A-Z])"
            return Regex.Replace(input, "([A-Z])", " $1", RegexOptions.Compiled).Trim();
        }

        protected string NaiveNameFixer(string input)
        {
            var lowercased = SplitCamelCase(input).Split(' ').Select(s => s.ToLower());
            return string.Join(" ", lowercased);
        }
    }
}