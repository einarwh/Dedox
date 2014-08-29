using System;
using System.IO;
using System.Linq;

using Roslyn.Compilers.CSharp;

namespace Dedox
{
    class GeneratedPropertyCommentsChecker : GeneratedCommentsChecker<PropertyDeclarationSyntax>
    {
        public GeneratedPropertyCommentsChecker(PropertyDeclarationSyntax it, TextWriter writer)
            : base(it, writer)
        { 
        }

        protected override string Name
        {
            get
            {
                return It.Identifier.ValueText;
            }
        }

        protected override string GetExpectedCommentForTag(XmlElementStartTagSyntax startTag)
        {
            var tag = startTag.Name.LocalName.ValueText;
            if ("summary".Equals(tag))
            {
                var expectedComment = GetPredictedPropertySummaryText();
                WriteLine("Expected comment: '{0}'", expectedComment);
                return expectedComment;
            }

            if ("value".Equals(tag))
            {
                var expectedComment = GetPredictedPropertyValueText();
                WriteLine("Expected comment: '{0}'", expectedComment);
                return expectedComment;
            }

            WriteLine("Unexpected tag {0} in property comment.", tag);
            return null;
        }

        private string GetPredictedPropertyValueText()
        {
            var name = NaiveNameFixer(Name);
            return string.Format("The {0}.", name);
        }

        private string GetPredictedPropertySummaryText()
        {
            Func<string, bool> hasAccessor =
                accessorName => It.AccessorList.Accessors.Any(ads => accessorName.Equals(ads.Keyword.ValueText));
            bool hasGetter = hasAccessor("get");
            bool hasSetter = hasAccessor("set");

            var fixedName = NaiveNameFixer(Name);

            string summaryText = string.Format("the {0}.", fixedName);

            if (It.Type.Kind == SyntaxKind.PredefinedType)
            {
                var predefType = (PredefinedTypeSyntax)It.Type;
                var isBoolProperty = "bool" == predefType.Keyword.ValueText;
                if (isBoolProperty)
                {
                    summaryText = "a value indicating whether " + summaryText;
                }
            }

            if (hasGetter && hasSetter)
            {
                return "Gets or sets " + summaryText;
            }

            if (hasGetter)
            {
                return "Gets " + summaryText;
            }

            if (hasSetter)
            {
                return "Sets " + summaryText;
            }

            throw new Exception("This makes no sense.");
        }
    }
}