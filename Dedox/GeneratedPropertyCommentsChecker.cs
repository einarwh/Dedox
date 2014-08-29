using System;
using System.Collections.Generic;
using System.Linq;

using Roslyn.Compilers.CSharp;

namespace Dedox
{
    class GeneratedPropertyCommentsChecker : GeneratedCommentsChecker<PropertyDeclarationSyntax>
    {
        public GeneratedPropertyCommentsChecker(PropertyDeclarationSyntax it)
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

        public override bool IsGenerated()
        {
            Console.WriteLine();
            Console.WriteLine("Property: " + Name);

            var predictedPropertySummaryText = GetPredictedPropertySummaryText();
            var predictedPropertyValueText = GetPredictedPropertyValueText();

            var expected = new Dictionary<string, string>();
            expected["summary"] = predictedPropertySummaryText;
            expected["value"] = predictedPropertyValueText;

            var docTrivia = GetDocumentationTrivia();
            var st = docTrivia.GetStructure();
            if (st == null)
            {
                // No XML comments.
                Console.WriteLine("Property {0} has no XML comments.", Name);
                return false;
            }

            var childNodes = st.ChildNodes();
            var maybeXmlElements = childNodes.Where(n => n.Kind == SyntaxKind.XmlElement);
            var xmlElements = maybeXmlElements.Cast<XmlElementSyntax>();

            foreach (XmlElementSyntax e in xmlElements)
            {
                string tag = e.StartTag.Name.LocalName.ValueText;
                if (!expected.ContainsKey(tag))
                {
                    // The XML comments contains an unexpected tag, 
                    // indicating it hasn't been (all) auto-generated.
                    Console.WriteLine("Property {0} has extra tag {1}.", Name, tag);
                    return false;
                }

                string expectedComment = expected[tag];
                string actualComment = ReadTagComment(e.Content);
                if (!string.Equals(expectedComment, actualComment, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Mismatch between expected comment '{0}' and actual comment '{1}'.", expectedComment, actualComment);
                    var dist = LevenshteinDistance.Compute(expectedComment, actualComment);
                    Console.WriteLine("Levenshtein distance: " + dist);
                    return false;
                }
            }

            Console.WriteLine("All the documentation for property {0} was written by a tool.", Name);

            return true;
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