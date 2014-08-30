﻿using System.IO;

using Roslyn.Compilers.CSharp;

namespace Dedox
{
    class GeneratedEnumCommentsChecker : GeneratedSummaryCommentsChecker<EnumDeclarationSyntax>
    {
        public GeneratedEnumCommentsChecker(EnumDeclarationSyntax it, IDedoxConfig config)
            : base(it, config)
        {
        }

        protected override string Name
        {
            get
            {
                return It.Identifier.ValueText;
            }
        }
    }
}