using System;
using System.Linq;

using Roslyn.Compilers.CSharp;

namespace Dedox
{
    class GeneratedClassCommentsChecker : GeneratedCommentsChecker<ClassDeclarationSyntax>
    {
        public GeneratedClassCommentsChecker(ClassDeclarationSyntax classDeclaration)
            : base(classDeclaration)
        {
        }

        protected override string Name
        {
            get
            {
                return It.Identifier.ValueText;
            }
        }

        public override bool IsGenerated()
        {
            Console.WriteLine("Class: " + Name);

            var docTrivia = GetDocumentationTrivia();
            var st = docTrivia.GetStructure();
            if (st == null)
            {
                // No XML comments.
                Console.WriteLine("Class {0} has no XML comments.", Name);
                return false;
            }

            var childNodes = st.ChildNodes();
            var maybeXmlElements = childNodes.Where(n => n.Kind == SyntaxKind.XmlElement);
            var xmlElements = maybeXmlElements.Cast<XmlElementSyntax>();

            foreach (XmlElementSyntax e in xmlElements)
            {
                XmlElementStartTagSyntax startTag = e.StartTag;
                string tag = startTag.Name.LocalName.ValueText;
                var expectedComment = GetExpectedClassCommentForTag(Name, tag);
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

            Console.WriteLine("All the documentation for class {0} was written by a tool.", Name);
            Console.WriteLine();

            return true;
        }

        private string GetExpectedClassCommentForTag(string name, string tag)
        {
            if ("summary".Equals(tag))
            {
                var expectedComment = string.Format("The {0}.", NaiveNameFixer(name));
                Console.WriteLine("Expected class comment: '{0}'", expectedComment);
                return expectedComment;
            }

            Console.WriteLine("Unexpected tag {0} in class comment.", tag);
            return null;
        }
    }
}