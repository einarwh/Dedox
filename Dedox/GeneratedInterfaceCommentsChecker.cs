using System;
using System.Collections.Generic;

using Roslyn.Compilers.CSharp;

namespace Dedox
{
    class GeneratedInterfaceCommentsChecker : GeneratedCommentsChecker<InterfaceDeclarationSyntax>
    {
        public GeneratedInterfaceCommentsChecker(InterfaceDeclarationSyntax it, IDedoxConfig config, IDedoxMetrics metrics)
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
                // Pattern: The $(name) interface.
                // Pattern: The $(decomposed-name) interface.
                // Pattern: The $(name).
                // Pattern: The $(decomposed-name).
                var stylecopChoppedName = Name.Substring(1);
                var list = new List<Func<string>>
                               {
                                   () => string.Format("The {0} interface.", stylecopChoppedName),
                                   () =>
                                   string.Format(
                                       "The {0} interface.",
                                       StyleCopDecompose(stylecopChoppedName)),
                                   () => string.Format("The {0}.", stylecopChoppedName),
                                   () => string.Format("The {0}.", StyleCopDecompose(stylecopChoppedName)),
                                   () => string.Format("The {0}.", Name),
                                   () => string.Format("The {0}.", StyleCopDecompose(Name))
                               };
                return list;
            }

            Info("Unexpected tag {0} in interface comment.", tag);
            return new List<Func<string>>();
        }

        protected override bool IsGeneratedCodeElement()
        {
            return IsGeneratedCodeElement(It);
        }
    }
}
