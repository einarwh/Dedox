using System;
using System.Linq;
using System.Text.RegularExpressions;

using Roslyn.Compilers.CSharp;

namespace Dedox
{
    class GeneratedMethodCommentsChecker : GeneratedCommentsChecker<MethodDeclarationSyntax>
    {
        public GeneratedMethodCommentsChecker(MethodDeclarationSyntax it)
            : base(it)
        {
        }

        protected override string Name
        {
            get
            {
                return It.Identifier.ValueText;
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
                    Console.WriteLine("The XML comment for the returns tag is probably obsolete.");
                    Console.WriteLine("Should be {0} but is actually {1}.", GetTypeNameForReturnsComment(), typeName);
                }

                Console.WriteLine();
            }
        }

        private string GetTypeNameForReturnsComment()
        {
            var ret = It.ReturnType;

            if (ret.Kind == SyntaxKind.PredefinedType)
            {
                // This includes 'void'. It makes little sense to document 'void'. Should detect this case.
                // At the same time, there will be a mismatch downstream.
                var predefType = (PredefinedTypeSyntax)ret;
                return predefType.Keyword.ValueText;
            }

            if (ret.Kind == SyntaxKind.IdentifierName)
            {
                // This includes custom types.
                var identType = (IdentifierNameSyntax)ret;
                return identType.Identifier.ValueText;
            }

            if (ret.Kind == SyntaxKind.GenericName)
            {
                var genericName = (GenericNameSyntax)ret;
                return genericName.Identifier.ValueText;
            }

            Console.WriteLine("Unknown return type kind: " + ret.Kind);
            return null;
        }

        protected override string GetExpectedCommentForTag(XmlElementStartTagSyntax startTag)
        {
            string tag = startTag.Name.LocalName.ValueText;

            if ("summary".Equals(tag))
            {
                var fixedMethodName = NaiveNameFixer(Name);
                var expectedMethodComment = string.Format("The {0}.", fixedMethodName);
                Console.WriteLine("Expected method comment (based on method name): '{0}'", expectedMethodComment);
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
                Console.WriteLine("Expected param comment: '{0}'", expectedParamComment);
                return expectedParamComment;
            }

            if ("returns".Equals(tag))
            {
                var returnTypeName = GetTypeNameForReturnsComment();
                if (returnTypeName == null)
                {
                    Console.WriteLine("Unknown content in return tag.");
                    return null;
                }

                var expectedReturnsComment = string.Format("The <see cref=\"{0}\"/>.", returnTypeName);
                Console.WriteLine("Expected returns comment: {0}.", expectedReturnsComment);
                return expectedReturnsComment;
            }

            // Console.WriteLine("Unexpected tag.");

            return null;
        }
    }
}