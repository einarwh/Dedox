using System;

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

        protected override string GetExpectedCommentForTag(XmlElementStartTagSyntax startTag, Func<string, string> nameTransform)
        {
            var tag = startTag.Name.LocalName.ValueText;
            if ("summary".Equals(tag))
            {
                // Pattern: The $(name) interface.
                // Pattern: The $(decomposed-name) interface.
                // Pattern: The $(name).
                // Pattern: The $(decomposed-name).
                var n = Name;
                var name = n[0] == 'I' ? n.Substring(1) : n;
                return string.Format("The {0} interface.", nameTransform(name));
            }

            Info("Unexpected tag {0} in interface comment.", tag);
            return null;
        }
    }
}
