using System;
using System.Collections.Generic;
using System.Linq;

using Roslyn.Compilers.CSharp;

namespace Dedox
{
    public static class Dedoxifier
    {
        public static string Run(string text)
        {
            Console.WriteLine("Running Dedox...");

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
                    Console.WriteLine("Remove XML comments from these classes:");
                    foreach (var c in purgeClasses)
                    {
                        Console.WriteLine(" - {0}", c.Identifier.ValueText);
                    }

                    Console.WriteLine();
                }

                if (purgeInterfaces.Any())
                {
                    Console.WriteLine("Remove XML comments from these interfaces:");
                    foreach (var it in purgeInterfaces)
                    {
                        Console.WriteLine(" - {0}", it.Identifier.ValueText);
                    }

                    Console.WriteLine();
                }

                if (purgeEnums.Any())
                {
                    Console.WriteLine("Remove XML comments from these enums:");
                    foreach (var it in purgeEnums)
                    {
                        Console.WriteLine(" - {0}", it.Identifier.ValueText);
                    }

                    Console.WriteLine();
                }

                if (purgeEnumMembers.Any())
                {
                    Console.WriteLine("Remove XML comments from these enum members:");
                    foreach (var it in purgeEnumMembers)
                    {
                        Console.WriteLine(" - {0}", it.Identifier.ValueText);
                    }

                    Console.WriteLine();
                }


                if (purgeFields.Any())
                {
                    Console.WriteLine("Remove XML comments from these fields:");
                    foreach (var f in purgeFields)
                    {
                        Console.WriteLine(" - {0}", f.Declaration.Variables.First().Identifier.ValueText);
                    }

                    Console.WriteLine();
                }

                if (purgeProperties.Any())
                {
                    Console.WriteLine("Remove XML comments from these properties:");
                    foreach (var p in purgeProperties)
                    {
                        Console.WriteLine(" - {0}", p.Identifier.ValueText);
                    }

                    Console.WriteLine();
                }

                if (purgeMethods.Any())
                {
                    Console.WriteLine("Remove XML comments from these methods:");
                    foreach (var m in purgeMethods)
                    {
                        Console.WriteLine(" - {0}", m.Identifier.ValueText);
                    }

                    Console.WriteLine();
                }

                if (purgeCtors.Any())
                {
                    Console.WriteLine("Remove XML comments from these constructors:");
                    foreach (var c in purgeCtors)
                    {
                        Console.WriteLine(" - {0}", c.Identifier.ValueText);
                    }

                    Console.WriteLine();
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
            
            Console.WriteLine("Nothing to dedox.");

            return null;
        }

        private static bool IsConstructorDocumentationGenerated(ConstructorDeclarationSyntax arg)
        {
            return new GeneratedConstructorCommentsChecker(arg).IsGenerated();
        }

        private static bool IsEnumMemberDocumentationGenerated(EnumMemberDeclarationSyntax enumMemberDeclaration)
        {
            return new GeneratedEnumMemberCommentsChecker(enumMemberDeclaration).IsGenerated();
        }

        private static bool IsEnumDocumentationGenerated(EnumDeclarationSyntax enumDeclaration)
        {
            return new GeneratedEnumCommentsChecker(enumDeclaration).IsGenerated();
        }

        private static bool IsClassDocumentationGenerated(ClassDeclarationSyntax classDeclaration)
        {
            return new GeneratedClassCommentsChecker(classDeclaration).IsGenerated();
        }

        private static bool IsInterfaceDocumentationGenerated(InterfaceDeclarationSyntax interfaceDeclaration)
        {
            return new GeneratedInterfaceCommentsChecker(interfaceDeclaration).IsGenerated();
        }

        private static bool IsFieldDocumentationGenerated(FieldDeclarationSyntax fieldDeclaration)
        {
            return new GeneratedFieldCommentsChecker(fieldDeclaration).IsGenerated();
        }

        private static bool IsMethodDocumentationGenerated(MethodDeclarationSyntax methodDeclaration)
        {
            return new GeneratedMethodCommentsChecker(methodDeclaration).IsGenerated();
        }

        private static bool IsPropertyDocumentationGenerated(PropertyDeclarationSyntax p)
        {
            return new GeneratedPropertyCommentsChecker(p).IsGenerated();
        }
    }
}
