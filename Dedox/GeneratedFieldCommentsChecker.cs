using System;
using System.Linq;

using Roslyn.Compilers.CSharp;

namespace Dedox
{
    class GeneratedFieldCommentsChecker : GeneratedCommentsChecker<FieldDeclarationSyntax>
    {
        public GeneratedFieldCommentsChecker(FieldDeclarationSyntax fieldDeclaration)
            : base(fieldDeclaration)
        {
        }

        protected override string Name
        {
            get
            {
                return It.Declaration.Variables.First().Identifier.ValueText;
            }
        }

        public override bool IsGenerated()
        {
            Console.WriteLine("Field: " + Name);

            var docTrivia = GetDocumentationTrivia();
            var st = docTrivia.GetStructure();
            if (st == null)
            {
                // No XML comments.
                Console.WriteLine("Field {0} has no XML comments.", It.Declaration.Variables.First().Identifier.ValueText);
                return false;
            }
            
            var childNodes = st.ChildNodes();
            var maybeXmlElements = childNodes.Where(n => n.Kind == SyntaxKind.XmlElement);
            var xmlElements = maybeXmlElements.Cast<XmlElementSyntax>();

            foreach (XmlElementSyntax e in xmlElements)
            {
                XmlElementStartTagSyntax startTag = e.StartTag;
                string tag = startTag.Name.LocalName.ValueText;
                var expectedComment = GetExpectedFieldCommentForTag(Name, tag);
                if (expectedComment == null)
                {
                    Console.WriteLine("Failed to produce an expectation for tag " + tag);
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

                    return false;
                }
            }

            Console.WriteLine("All the documentation for field {0} was written by a tool.", Name);
            Console.WriteLine();

            return true;
        }

        private string GetExpectedFieldCommentForTag(string name, string tag)
        {
            if ("summary".Equals(tag))
            {
                var expectedComment = string.Format("The {0}.", NaiveNameFixer(name));
                Console.WriteLine("Expected field comment: '{0}'", expectedComment);
                return expectedComment;
            }

            Console.WriteLine("Unexpected tag {0} in field comment.", tag);
            return null;
        }
    }
}