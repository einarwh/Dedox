using System;
using System.Collections.Generic;
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
                    Info("The XML comment for the returns tag is probably obsolete.");
                    Info("Should be {0} but is actually {1}.", ClassTypeName, typeName);
                }

                Info();
            }
        }

        protected override List<Func<string>> GetExpectedCommentForTag(XmlElementStartTagSyntax startTag)
        {
            string tag = startTag.Name.LocalName.ValueText;

            if ("summary".Equals(tag))
            {
                return ExpectedCommentForSummaryTag();
            }

            if ("param".Equals(tag))
            {
                return ExpectedCommentForParamTag(startTag);
            }

            if ("returns".Equals(tag))
            {
                return ExpectedCommentForReturnsTag();
            }

            return new List<Func<string>>();
        }

        private List<Func<string>> ExpectedCommentForSummaryTag()
        {
            var list = new List<Func<string>>
                           {
                               () =>
                               string.Format(
                                   "Initializes a new instance of the <see cref=\"{0}\"/> class.",
                                   ClassTypeName)
                           };
            return list;
        }

        private List<Func<string>> ExpectedCommentForParamTag(XmlElementStartTagSyntax startTag)
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
                    return new List<Func<string>>();
                }
            }

            return new List<Func<string>> { () => string.Format("The {0}.", StyleCopDecompose(nameAttributeValue)) };
        }

        private List<Func<string>> ExpectedCommentForReturnsTag()
        {
            var returnTypeName = ClassTypeName;
            if (returnTypeName == null)
            {
                Info("Unknown content in return tag.");
                return new List<Func<string>>();
            }

            return new List<Func<string>> { () => string.Format("The <see cref=\"{0}\"/>.", returnTypeName) };
        }
    }
}