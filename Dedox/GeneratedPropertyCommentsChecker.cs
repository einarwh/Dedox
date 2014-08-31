using System;
using System.Collections.Generic;
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

        protected override List<Func<string>> GetExpectedCommentForTag(XmlElementStartTagSyntax startTag)
        {
            var tag = startTag.Name.LocalName.ValueText;
            if ("summary".Equals(tag))
            {
                return GetPredictedPropertySummaryText();
            }

            if ("value".Equals(tag))
            {
                return GetPredictedPropertyValueText();
            }

            Info("Unexpected tag {0} in property comment.", tag);

            return new List<Func<string>>();
        }

        private List<Func<string>> GetPredictedPropertyValueText()
        {
            return new List<Func<string>>
                       {
                           () => string.Format("The {0}.", StyleCopDecompose(Name)),
                           () => string.Format("The {0}.", Name)
                       };
        }

        private List<Func<string>> GetPredictedPropertySummaryText()
        {
            Func<string, bool> hasAccessor =
                accessorName => It.AccessorList.Accessors.Any(ads => accessorName.Equals(ads.Keyword.ValueText));
            bool hasGetter = hasAccessor("get");
            bool hasSetter = hasAccessor("set");

            var decomposedName = StyleCopDecompose(Name);

            bool isBoolProperty = false;
            if (It.Type.Kind == SyntaxKind.PredefinedType)
            {
                var predefType = (PredefinedTypeSyntax)It.Type;
                isBoolProperty = "bool" == predefType.Keyword.ValueText;
            }

            var boolText = isBoolProperty ? " a value indicating whether" : "";
            var basicList = new List<Func<string>>
                                {
                                    () => string.Format("{1} the {0}.", decomposedName, boolText),
                                    () => string.Format("{1} {0}.", decomposedName, boolText)
                                };

            if (hasGetter && hasSetter)
            {
                return basicList.Select(g => (Func<string>)(() => "Gets or sets" + g())).ToList();
            }

            if (hasGetter)
            {
                return basicList.Select(g => (Func<string>)(() => "Gets" + g())).ToList();
            }

            if (hasSetter)
            {
                return basicList.Select(g => (Func<string>)(() => "Sets" + g())).ToList();
            }

            throw new Exception("This makes no sense.");
        }
    }
}