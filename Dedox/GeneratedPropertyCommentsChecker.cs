using System;
using System.Linq;

using Roslyn.Compilers.CSharp;

namespace Dedox
{
    class GeneratedPropertyCommentsChecker : GeneratedCommentsChecker<PropertyDeclarationSyntax>
    {
        public GeneratedPropertyCommentsChecker(PropertyDeclarationSyntax it, IDedoxConfig config, IDedoxMetrics metrics)
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

        protected override string GetExpectedCommentForTag(XmlElementStartTagSyntax startTag, Func<string, string> nameTransform)
        {
            var tag = startTag.Name.LocalName.ValueText;
            if ("summary".Equals(tag))
            {
                return GetPredictedPropertySummaryText(nameTransform);
            }

            if ("value".Equals(tag))
            {
                return GetPredictedPropertyValueText(nameTransform);
            }

            Info("Unexpected tag {0} in property comment.", tag);

            return null;
        }

        private string GetPredictedPropertyValueText(Func<string, string> nameTransform)
        {
            var name = nameTransform(Name);
            return string.Format("The {0}.", name);
        }

        private string GetPredictedPropertySummaryText(Func<string, string> nameTransform)
        {
            Func<string, bool> hasAccessor =
                accessorName => It.AccessorList.Accessors.Any(ads => accessorName.Equals(ads.Keyword.ValueText));
            bool hasGetter = hasAccessor("get");
            bool hasSetter = hasAccessor("set");

            var fixedName = nameTransform(Name);

            string summaryText = string.Format("the {0}.", fixedName);

            if (It.Type.Kind == SyntaxKind.PredefinedType)
            {
                var predefType = (PredefinedTypeSyntax)It.Type;
                var isBoolProperty = "bool" == predefType.Keyword.ValueText;
                if (isBoolProperty)
                {
                    summaryText = string.Format("a value indicating whether " + summaryText);
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