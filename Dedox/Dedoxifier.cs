using System.Collections.Generic;
using System.IO;
using System.Linq;

using Roslyn.Compilers.CSharp;

namespace Dedox
{
    public class Dedoxifier
    {
        private readonly IDedoxConfig _config;
        private readonly IDedoxMetrics _metrics;
        private readonly IConsoleWriter _writer;

        public Dedoxifier(IDedoxConfig config, IDedoxMetrics metrics)
        {
            _config = config;
            _writer = config.Writer;
            _metrics = metrics;
        }

        public string Run(string text)
        {
            var tree = SyntaxTree.ParseText(text);
            var nodes = tree.GetRoot().DescendantNodes().ToList();

            var classNodes = nodes
                .Where(n => n.Kind == SyntaxKind.ClassDeclaration)
                .Cast<ClassDeclarationSyntax>();

            var interfaceNodes = nodes
                .Where(n => n.Kind == SyntaxKind.InterfaceDeclaration)
                .Cast<InterfaceDeclarationSyntax>();

            var enumNodes = nodes
                .Where(n => n.Kind == SyntaxKind.EnumDeclaration)
                .Cast<EnumDeclarationSyntax>();

            var enumMemberNodes = nodes
                .Where(n => n.Kind == SyntaxKind.EnumMemberDeclaration)
                .Cast<EnumMemberDeclarationSyntax>();

            var fieldNodes = nodes
                .Where(n => n.Kind == SyntaxKind.FieldDeclaration)
                .Cast<FieldDeclarationSyntax>();

            var propertyNodes = nodes
                .Where(n => n.Kind == SyntaxKind.PropertyDeclaration)
                .Cast<PropertyDeclarationSyntax>();
            
            var methodNodes = nodes
                .Where(n => n.Kind == SyntaxKind.MethodDeclaration)
                .Cast<MethodDeclarationSyntax>();

            var ctorNodes = nodes
                .Where(n => n.Kind == SyntaxKind.ConstructorDeclaration)
                .Cast<ConstructorDeclarationSyntax>();

            var purgeClasses = classNodes.Where(IsClassDocumentationGenerated).ToList();

            var purgeInterfaces = interfaceNodes.Where(IsInterfaceDocumentationGenerated).ToList();

            var purgeEnums = enumNodes.Where(IsEnumDocumentationGenerated).ToList();

            var purgeEnumMembers = enumMemberNodes.Where(IsEnumMemberDocumentationGenerated).ToList();

            var purgeFields = fieldNodes.Where(IsFieldDocumentationGenerated).ToList();

            var purgeProperties = propertyNodes.Where(IsPropertyDocumentationGenerated).ToList();

            var purgeMethods = methodNodes.Where(IsMethodDocumentationGenerated).ToList();

            var purgeCtors = ctorNodes.Where(IsConstructorDocumentationGenerated).ToList();

            var purgeMembers = purgeFields.Cast<MemberDeclarationSyntax>()
                .Concat(purgeClasses)
                .Concat(purgeInterfaces)
                .Concat(purgeEnums)
                .Concat(purgeEnumMembers)
                .Concat(purgeProperties)
                .Concat(purgeMethods)
                .Concat(purgeCtors)
                .ToList();

            if (purgeMembers.Any())
            {
                Info();

                if (purgeClasses.Any())
                {
                    Info("Remove XML comments from these classes:");
                    foreach (var c in purgeClasses)
                    {
                        Info(" - {0}", c.Identifier.ValueText);
                    }

                    Info();
                }

                if (purgeInterfaces.Any())
                {
                    Info("Remove XML comments from these interfaces:");
                    foreach (var it in purgeInterfaces)
                    {
                        Info(" - {0}", it.Identifier.ValueText);
                    }

                    Info();
                }

                if (purgeEnums.Any())
                {
                    Info("Remove XML comments from these enums:");
                    foreach (var it in purgeEnums)
                    {
                        Info(" - {0}", it.Identifier.ValueText);
                    }

                    Info();
                }

                if (purgeEnumMembers.Any())
                {
                    Info("Remove XML comments from these enum members:");
                    foreach (var it in purgeEnumMembers)
                    {
                        Info(" - {0}", it.Identifier.ValueText);
                    }

                    Info();
                }

                if (purgeFields.Any())
                {
                    Info("Remove XML comments from these fields:");
                    foreach (var f in purgeFields)
                    {
                        Info(" - {0}", f.Declaration.Variables.First().Identifier.ValueText);
                    }

                    Info();
                }

                if (purgeProperties.Any())
                {
                    Info("Remove XML comments from these properties:");
                    foreach (var p in purgeProperties)
                    {
                        Info(" - {0}", p.Identifier.ValueText);
                    }

                    Info();
                }

                if (purgeMethods.Any())
                {
                    Info("Remove XML comments from these methods:");
                    foreach (var m in purgeMethods)
                    {
                        Info(" - {0}", m.Identifier.ValueText);
                    }

                    Info();
                }

                if (purgeCtors.Any())
                {
                    Info("Remove XML comments from these constructors:");
                    foreach (var c in purgeCtors)
                    {
                        Info(" - {0}", c.Identifier.ValueText);
                    }

                    Info();
                }

                var root = tree.GetRoot();
                var removeTrivia = new List<SyntaxTrivia>();

                foreach (var p in purgeMembers)
                {
                    var leadingTriviaReverse = p.GetLeadingTrivia().Reverse();
                    foreach (var trivia in leadingTriviaReverse)
                    {
                        removeTrivia.Add(trivia);
                        if (trivia.Kind == SyntaxKind.DocumentationCommentTrivia)
                        {
                            break;
                        }
                    }
                }

                var newRoot = root.ReplaceTrivia(removeTrivia, (t1, t2) => SyntaxTriviaList.Empty);

                return newRoot.ToFullString();
            }
            
            return null;
        }

        private bool IsConstructorDocumentationGenerated(ConstructorDeclarationSyntax declaration)
        {
            return new GeneratedConstructorCommentsChecker(declaration, _config, _metrics).IsGenerated();
        }

        private bool IsEnumMemberDocumentationGenerated(EnumMemberDeclarationSyntax declaration)
        {
            return new GeneratedEnumMemberCommentsChecker(declaration, _config, _metrics).IsGenerated();
        }

        private bool IsEnumDocumentationGenerated(EnumDeclarationSyntax declaration)
        {
            return new GeneratedEnumCommentsChecker(declaration, _config, _metrics).IsGenerated();
        }

        private bool IsClassDocumentationGenerated(ClassDeclarationSyntax declaration)
        {
            return new GeneratedClassCommentsChecker(declaration, _config, _metrics).IsGenerated();
        }

        private bool IsInterfaceDocumentationGenerated(InterfaceDeclarationSyntax declaration)
        {
            return new GeneratedInterfaceCommentsChecker(declaration, _config, _metrics).IsGenerated();
        }

        private bool IsFieldDocumentationGenerated(FieldDeclarationSyntax declaration)
        {
            return new GeneratedFieldCommentsChecker(declaration, _config, _metrics).IsGenerated();
        }

        private bool IsMethodDocumentationGenerated(MethodDeclarationSyntax declaration)
        {
            return new GeneratedMethodCommentsChecker(declaration, _config, _metrics).IsGenerated();
        }

        private bool IsPropertyDocumentationGenerated(PropertyDeclarationSyntax declaration)
        {
            return new GeneratedPropertyCommentsChecker(declaration, _config, _metrics).IsGenerated();
        }

        private void Info(string format, params object[] args)
        {
            _writer.Info(format, args);
        }

        private void Info()
        {
            _writer.Info();
        }
    }
}
