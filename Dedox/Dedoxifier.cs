using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Roslyn.Compilers.CSharp;

namespace Dedox
{
    public static class Dedoxifier
    {
        public static void Run(string text)
        {
            Console.WriteLine("Running Dedox...");

            var tree = SyntaxTree.ParseText(text);
            var nodes = tree.GetRoot().DescendantNodes().ToList();

            var classNodes = nodes
                .Where(n => n.Kind == SyntaxKind.ClassDeclaration)
                .Cast<ClassDeclarationSyntax>();

            var fieldNodes = nodes
                .Where(n => n.Kind == SyntaxKind.FieldDeclaration)
                .Cast<FieldDeclarationSyntax>();

            var propertyNodes = nodes
                .Where(n => n.Kind == SyntaxKind.PropertyDeclaration)
                .Cast<PropertyDeclarationSyntax>();
            var methodNodes = nodes
                .Where(n => n.Kind == SyntaxKind.MethodDeclaration)
                .Cast<MethodDeclarationSyntax>();

            var purgeClasses = classNodes.Where(IsClassDocumentationGenerated).ToList();

            var purgeFields = fieldNodes.Where(IsFieldDocumentationGenerated).ToList();

            var purgeProperties = propertyNodes.Where(IsPropertyDocumentationGenerated).ToList();

            var purgeMethods = methodNodes.Where(IsMethodDocumentationGenerated).ToList();

            var purgeMembers = purgeFields.Cast<MemberDeclarationSyntax>()
                .Concat(purgeClasses)
                .Concat(purgeProperties)
                .Concat(purgeMethods)
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
                Console.WriteLine(newRoot);
            }
            else
            {
                Console.WriteLine("Nothing to dedox.");
            }

            Console.WriteLine();

            Console.ReadKey();
        }

        private static bool IsClassDocumentationGenerated(ClassDeclarationSyntax classDeclaration)
        {
            return new GeneratedClassCommentsChecker(classDeclaration).IsGenerated();
        }

        private static bool IsFieldDocumentationGenerated(FieldDeclarationSyntax fieldDeclaration)
        {
            return new GeneratedFieldCommentsChecker(fieldDeclaration).IsGenerated();
        }

        private static bool IsMethodDocumentationGenerated(MethodDeclarationSyntax methodDeclaration)
        {
            return new GeneratedMethodCommentsChecker(methodDeclaration).IsGenerated();
        }

        private static SyntaxTrivia GetDocumentationTrivia(MemberDeclarationSyntax p)
        {
            return p.GetLeadingTrivia().FirstOrDefault(t => t.Kind == SyntaxKind.DocumentationCommentTrivia);
        }

        private static bool IsPropertyDocumentationGenerated(PropertyDeclarationSyntax p)
        {
            return new GeneratedPropertyCommentsChecker(p).IsGenerated();
        }
    }
}
