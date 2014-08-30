using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Roslyn.Compilers.CSharp;

namespace Dedox
{
    class GeneratedConstructorCommentsChecker : GeneratedCommentsChecker<ConstructorDeclarationSyntax>
    {
        public GeneratedConstructorCommentsChecker(ConstructorDeclarationSyntax it, IDedoxConfig config, IDedoxMetrics metrics)
            : base(it, config, metrics)
        {
        }

        protected override string Name
        {
            get
            {
                return It.Identifier.ValueText;
            }
        }

        protected string ClassTypeName
        {
            get
            {
                var classDeclaration = (ClassDeclarationSyntax)It.Parent;
                return classDeclaration.Identifier.ValueText;
            }
        }

        protected override void OnMismatch(string tag, string expectedComment, string actualComment)
        {
            if ("returns".Equals(tag))
            {
                const string genericReturnsPattern = @"The <see cref=""(\w+)""/>\.";
                var match = Regex.Match(actualComment, genericReturnsPattern);
                if (match.Success)
                {
                    var typeName = match.Groups[1].Value;
                    WriteLine("The XML comment for the returns tag is probably obsolete.");
                    WriteLine("Should be {0} but is actually {1}.", ClassTypeName, typeName);
                }

                WriteLine();
            }
        }

        protected override string GetExpectedCommentForTag(XmlElementStartTagSyntax startTag)
        {
            string tag = startTag.Name.LocalName.ValueText;

            if ("summary".Equals(tag))
            {
                var expectedMethodComment = string.Format("Initializes a new instance of the <see cref=\"{0}\"/> class.", ClassTypeName);
                WriteLine("Expected comment (based on class name): '{0}'", expectedMethodComment);
                return expectedMethodComment;
            }

            if ("param".Equals(tag))
            {
                string nameAttributeValue = "";
                foreach (var a in startTag.Attributes)
                {
                    var attrName = a.Name.LocalName.ValueText;
                    var attrValue = string.Join("", a.TextTokens.Select(t => t.ValueText));
                    if ("name".Equals(attrName))
                    {
                        nameAttributeValue = attrValue;
                    }
                    else
                    {
                        // Unexpected attribute.
                        return null;
                    }
                }

                var fixedParamName = NaiveNameFixer(nameAttributeValue);
                var expectedParamComment = string.Format("The {0}.", fixedParamName);
                WriteLine("Expected param comment: '{0}'", expectedParamComment);
                return expectedParamComment;
            }

            if ("returns".Equals(tag))
            {
                var returnTypeName = ClassTypeName;
                if (returnTypeName == null)
                {
                    WriteLine("Unknown content in return tag.");
                    return null;
                }

                var expectedReturnsComment = string.Format("The <see cref=\"{0}\"/>.", returnTypeName);
                WriteLine("Expected returns comment: {0}.", expectedReturnsComment);
                return expectedReturnsComment;
            }

            return null;
        }
    }
}