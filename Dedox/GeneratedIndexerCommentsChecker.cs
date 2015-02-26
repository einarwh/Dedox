using System;
using System.Collections.Generic;
using System.Linq;

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

            if ("param".Equals(tag))
            {
                return ExpectedCommentForParamTag(startTag);
            }


            if ("returns".Equals(tag))
            {
                return GetPredictedIndexerReturnsText();
            }

            Info("Unexpected tag {0} in property comment.", tag);

            return new List<Func<string>>();
        }

        private List<Func<string>> ExpectedCommentForParamTag(XmlElementStartTagSyntax startTag)
        {
            string paramNameFromAttribute = "";
            foreach (var a in startTag.Attributes)
            {
                var attrName = a.Name.LocalName.ValueText;
                var attrValue = string.Join("", a.TextTokens.Select(t => t.ValueText));
                if ("name".Equals(attrName))
                {
                    paramNameFromAttribute = attrValue;
                }
                else
                {
                    // Unexpected attribute.
                    return new List<Func<string>>();
                }
            }

            // Sanity check for attribute value.
            var paramExists =
                It.ParameterList.Parameters.Any(
                    ps =>
                    string.Equals(
                        ps.Identifier.ValueText,
                        paramNameFromAttribute,
                        StringComparison.InvariantCultureIgnoreCase));

            if (!paramExists)
            {
                Info("! The indexer does not have a parameter named {0}.", paramNameFromAttribute);
            }

            var decomposedParamName = StyleCopDecompose(paramNameFromAttribute);

            return new List<Func<string>>
                       {
                           () => string.Format("The {0}.", decomposedParamName),
                           () => string.Format("The {0}.", paramNameFromAttribute)
                       };
        }

        private List<Func<string>> GetPredictedIndexerReturnsText()
        {
            var returnTypeName = GetTypeNameForReturnsComment();
            if (returnTypeName == null)
            {
                Info("Unknown content in return tag.");
                return new List<Func<string>>();
            }

            return new List<Func<string>> { () => string.Format("The <see cref=\"{0}\"/>.", returnTypeName) };
        }

        private string GetTypeNameForReturnsComment()
        {   
            var ret = It.Type;

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

            Info("Unknown return type kind: " + ret.Kind);
            return null;
        }

        private List<Func<string>> GetPredictedIndexerValueText()
        {
            return new List<Func<string>>();
        }

        private List<Func<string>> GetPredictedIndexerSummaryText()
        {
            return new List<Func<string>>
                       {
                           () => string.Format("The this.")
                       };
        }
    }
}