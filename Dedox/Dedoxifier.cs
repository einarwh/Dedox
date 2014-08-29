using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Roslyn.Compilers.CSharp;

namespace Dedox
{
    public class Dedoxifier
    {
        private readonly TextWriter _writer;

        public Dedoxifier(TextWriter writer)
        {
            _writer = writer;
        }

        public string Run(string text)
        {
            _writer.WriteLine();

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
                if (purgeClasses.Any())
                {
                    _writer.WriteLine("Remove XML comments from these classes:");
                    foreach (var c in purgeClasses)
                    {
                        _writer.WriteLine(" - {0}", c.Identifier.ValueText);
                    }

                    _writer.WriteLine();
                }

                if (purgeInterfaces.Any())
                {
                    _writer.WriteLine("Remove XML comments from these interfaces:");
                    foreach (var it in purgeInterfaces)
                    {
                        _writer.WriteLine(" - {0}", it.Identifier.ValueText);
                    }

                    _writer.WriteLine();
                }

                if (purgeEnums.Any())
                {
                    _writer.WriteLine("Remove XML comments from these enums:");
                    foreach (var it in purgeEnums)
                    {
                        _writer.WriteLine(" - {0}", it.Identifier.ValueText);
                    }

                    _writer.WriteLine();
                }

                if (purgeEnumMembers.Any())
                {
                    _writer.WriteLine("Remove XML comments from these enum members:");
                    foreach (var it in purgeEnumMembers)
                    {
                        _writer.WriteLine(" - {0}", it.Identifier.ValueText);
                    }

                    _writer.WriteLine();
                }


                if (purgeFields.Any())
                {
                    _writer.WriteLine("Remove XML comments from these fields:");
                    foreach (var f in purgeFields)
                    {
                        _writer.WriteLine(" - {0}", f.Declaration.Variables.First().Identifier.ValueText);
                    }

                    _writer.WriteLine();
                }

                if (purgeProperties.Any())
                {
                    _writer.WriteLine("Remove XML comments from these properties:");
                    foreach (var p in purgeProperties)
                    {
                        _writer.WriteLine(" - {0}", p.Identifier.ValueText);
                    }

                    _writer.WriteLine();
                }

                if (purgeMethods.Any())
                {
                    _writer.WriteLine("Remove XML comments from these methods:");
                    foreach (var m in purgeMethods)
                    {
                        _writer.WriteLine(" - {0}", m.Identifier.ValueText);
                    }

                    _writer.WriteLine();
                }

                if (purgeCtors.Any())
                {
                    _writer.WriteLine("Remove XML comments from these constructors:");
                    foreach (var c in purgeCtors)
                    {
                        _writer.WriteLine(" - {0}", c.Identifier.ValueText);
                    }

                    _writer.WriteLine();
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

        private bool IsConstructorDocumentationGenerated(ConstructorDeclarationSyntax arg)
        {
            return new GeneratedConstructorCommentsChecker(arg, _writer).IsGenerated();
        }

        private bool IsEnumMemberDocumentationGenerated(EnumMemberDeclarationSyntax enumMemberDeclaration)
        {
            return new GeneratedEnumMemberCommentsChecker(enumMemberDeclaration, _writer).IsGenerated();
        }

        private bool IsEnumDocumentationGenerated(EnumDeclarationSyntax enumDeclaration)
        {
            return new GeneratedEnumCommentsChecker(enumDeclaration, _writer).IsGenerated();
        }

        private bool IsClassDocumentationGenerated(ClassDeclarationSyntax classDeclaration)
        {
            return new GeneratedClassCommentsChecker(classDeclaration, _writer).IsGenerated();
        }

        private bool IsInterfaceDocumentationGenerated(InterfaceDeclarationSyntax interfaceDeclaration)
        {
            return new GeneratedInterfaceCommentsChecker(interfaceDeclaration, _writer).IsGenerated();
        }

        private bool IsFieldDocumentationGenerated(FieldDeclarationSyntax fieldDeclaration)
        {
            return new GeneratedFieldCommentsChecker(fieldDeclaration, _writer).IsGenerated();
        }

        private bool IsMethodDocumentationGenerated(MethodDeclarationSyntax methodDeclaration)
        {
            return new GeneratedMethodCommentsChecker(methodDeclaration, _writer).IsGenerated();
        }

        private bool IsPropertyDocumentationGenerated(PropertyDeclarationSyntax p)
        {
            return new GeneratedPropertyCommentsChecker(p, _writer).IsGenerated();
        }
    }
}
