using System;
using System.IO;

using Roslyn.Compilers.CSharp;

namespace Dedox
{
    class GeneratedFieldCommentsChecker : GeneratedSummaryCommentsChecker<FieldDeclarationSyntax>
    {
        public GeneratedFieldCommentsChecker(FieldDeclarationSyntax fieldDeclaration, TextWriter writer)
            : base(fieldDeclaration, writer)
        {
        }

        protected override string Name
        {
            get
            {
                return It.Declaration.Variables.First().Identifier.ValueText;
            }
        }
    }
}