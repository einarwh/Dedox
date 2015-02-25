using System;
using System.Collections.Generic;

using Roslyn.Compilers.CSharp;

namespace Dedox
{
    class GeneratedIndexerCommentsChecker : GeneratedCommentsChecker<IndexerDeclarationSyntax>
    {
        public GeneratedIndexerCommentsChecker(IndexerDeclarationSyntax declaration, IDedoxConfig config, IDedoxMetrics metrics) : base(declaration, config, metrics)
        {
        }

        protected override string Name
        {
            get
            {
                return "this";
            }
        }

        protected override List<Func<string>> GetExpectedCommentForTag(XmlElementStartTagSyntax startTag)
        {
            var tag = startTag.Name.LocalName.ValueText;
            if ("summary".Equals(tag))
            {
                return GetPredictedIndexerSummaryText();
            }

            if ("value".Equals(tag))
            {
                return GetPredictedIndexerValueText();
            }

            Info("Unexpected tag {0} in property comment.", tag);

            return new List<Func<string>>();
        }

        private List<Func<string>> GetPredictedIndexerValueText()
        {
            return new List<Func<string>>();
        }

        private List<Func<string>> GetPredictedIndexerSummaryText()
        {
            return new List<Func<string>>();
        }
    }
}