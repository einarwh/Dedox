using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Roslyn.Compilers.CSharp;

namespace Dedox
{
    public static class Dedoxifier
    {
        public static string SplitCamelCase(string input)
        {
            // Better expression: "(?<=[a-z])([A-Z])"
            return Regex.Replace(input, "([A-Z])", " $1", RegexOptions.Compiled).Trim();
        }

        public static string NaiveNameFixer(string input)
        {
            var lowercased = SplitCamelCase(input).Split(' ').Select(s => s.ToLower());
            return string.Join(" ", lowercased);
        }

        public static void Run(string text)
        {
            Console.WriteLine("Running Dedox...");
            Console.WriteLine();

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
            var name = classDeclaration.Identifier.ValueText;
            Console.WriteLine("Class: " + name);

            var docTrivia = GetDocumentationTrivia(classDeclaration);
            var st = docTrivia.GetStructure();
            if (st == null)
            {
                // No XML comments.
                Console.WriteLine("Class {0} has no XML comments.", name);
                return false;
            }

            var childNodes = st.ChildNodes();
            var maybeXmlElements = childNodes.Where(n => n.Kind == SyntaxKind.XmlElement);
            var xmlElements = maybeXmlElements.Cast<XmlElementSyntax>();

            foreach (XmlElementSyntax e in xmlElements)
            {
                XmlElementStartTagSyntax startTag = e.StartTag;
                string tag = startTag.Name.LocalName.ValueText;
                var expectedComment = GetExpectedClassCommentForTag(name, tag);
                if (expectedComment == null)
                {
                    Console.WriteLine("Failed to produce an expectation for tag " + tag);
                    return false;
                }

                string actualComment = ReadTagComment(e.Content);

                if (!string.Equals(expectedComment, actualComment, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Comment mismatch.");
                    Console.WriteLine("Expected comment: '{0}'", expectedComment);
                    Console.WriteLine("Actual comment: '{0}'", actualComment);

                    var dist = LevenshteinDistance.Compute(expectedComment, actualComment);
                    Console.WriteLine("Levenshtein distance: " + dist);

                    return false;
                }
            }

            Console.WriteLine("All the documentation for class {0} was written by a tool.", name);
            Console.WriteLine();

            return true;
        }

        private static string GetExpectedClassCommentForTag(string name, string tag)
        {
            if ("summary".Equals(tag))
            {
                var expectedComment = string.Format("The {0}.", NaiveNameFixer(name));
                Console.WriteLine("Expected class comment: '{0}'", expectedComment);
                return expectedComment;
            }

            Console.WriteLine("Unexpected tag {0} in class comment.", tag);
            return null;
        }

        private static bool IsFieldDocumentationGenerated(FieldDeclarationSyntax fieldDeclaration)
        {
            var name = fieldDeclaration.Declaration.Variables.First().Identifier.ValueText;
            Console.WriteLine("Field: " + name);

            var docTrivia = GetDocumentationTrivia(fieldDeclaration);
            var st = docTrivia.GetStructure();
            if (st == null)
            {
                // No XML comments.
                Console.WriteLine("Field {0} has no XML comments.", fieldDeclaration.Declaration.Variables.First().Identifier.ValueText);
                return false;
            }
            
            var childNodes = st.ChildNodes();
            var maybeXmlElements = childNodes.Where(n => n.Kind == SyntaxKind.XmlElement);
            var xmlElements = maybeXmlElements.Cast<XmlElementSyntax>();

            foreach (XmlElementSyntax e in xmlElements)
            {
                XmlElementStartTagSyntax startTag = e.StartTag;
                string tag = startTag.Name.LocalName.ValueText;
                var expectedComment = GetExpectedFieldCommentForTag(name, tag);
                if (expectedComment == null)
                {
                    Console.WriteLine("Failed to produce an expectation for tag " + tag);
                    return false;
                }

                string actualComment = ReadTagComment(e.Content);

                if (!string.Equals(expectedComment, actualComment, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Comment mismatch.");
                    Console.WriteLine("Expected comment: '{0}'", expectedComment);
                    Console.WriteLine("Actual comment: '{0}'", actualComment);

                    var dist = LevenshteinDistance.Compute(expectedComment, actualComment);
                    Console.WriteLine("Levenshtein distance: " + dist);

                    return false;
                }
            }

            Console.WriteLine("All the documentation for field {0} was written by a tool.", name);
            Console.WriteLine();

            return true;
        }

        private static string GetExpectedFieldCommentForTag(string name, string tag)
        {
            if ("summary".Equals(tag))
            {
                var expectedComment = string.Format("The {0}.", NaiveNameFixer(name));
                Console.WriteLine("Expected field comment: '{0}'", expectedComment);
                return expectedComment;                
            }

            Console.WriteLine("Unexpected tag {0} in field comment.", tag);
            return null;
        }

        private static bool IsMethodDocumentationGenerated(MethodDeclarationSyntax m)
        {
            var docTrivia = GetDocumentationTrivia(m);
            var st = docTrivia.GetStructure();
            if (st == null)
            {
                // No XML comments.
                // Console.WriteLine("Method {0} has no XML comments.", methodName);
                return false;
            }

            var childNodes = st.ChildNodes();
            var maybeXmlElements = childNodes.Where(n => n.Kind == SyntaxKind.XmlElement);
            var xmlElements = maybeXmlElements.Cast<XmlElementSyntax>();

            foreach (XmlElementSyntax e in xmlElements)
            {
                XmlElementStartTagSyntax startTag = e.StartTag;
                string tag = startTag.Name.LocalName.ValueText;
                var expectedComment = GetExpectedMethodCommentForTag(m, startTag);
                if (expectedComment == null)
                {
                    Console.WriteLine("Failed to produce an expectation for tag " + tag);
                    return false;
                }

                string actualComment = ReadTagComment(e.Content);

                if (!expectedComment.Equals(actualComment))
                {
                    Console.WriteLine("Comment mismatch.");
                    Console.WriteLine("Expected comment: '{0}'", expectedComment);
                    Console.WriteLine("Actual comment: '{0}'", actualComment);

                    var dist = LevenshteinDistance.Compute(expectedComment, actualComment);
                    Console.WriteLine("Levenshtein distance: " + dist);

                    if ("returns".Equals(tag))
                    {
                        const string genericReturnsPattern = @"The <see cref=""(\w+)""/>\.";
                        var match = Regex.Match(actualComment, genericReturnsPattern);
                        if (match.Success)
                        {
                            var typeName = match.Groups[1].Value;
                            Console.WriteLine("The XML comment for the returns tag is probably obsolete.");
                            Console.WriteLine("Should be {0} but is actually {1}.", GetTypeNameForReturnsComment(m), typeName);
                        }

                        Console.WriteLine();
                    }

                    return false;
                }
            }

            Console.WriteLine("All the documentation for method {0} was written by a tool.", m.Identifier.ValueText);
            Console.WriteLine();
            
            return true;
        }

        private static string GetTypeNameForReturnsComment(MethodDeclarationSyntax methodDeclaration)
        {
            var ret = methodDeclaration.ReturnType;

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

            Console.WriteLine("Unknown return type kind: " + ret.Kind);
            return null;
        }

        private static string GetExpectedMethodCommentForTag(MethodDeclarationSyntax methodDeclaration, XmlElementStartTagSyntax startTag)
        {
            var methodName = methodDeclaration.Identifier.ValueText;
            string tag = startTag.Name.LocalName.ValueText;

            if ("summary".Equals(tag))
            {
                var fixedMethodName = NaiveNameFixer(methodName);
                var expectedMethodComment = string.Format("The {0}.", fixedMethodName);
                Console.WriteLine("Expected method comment (based on method name): '{0}'", expectedMethodComment);
                return expectedMethodComment;
            }

            if ("param".Equals(tag))
            {
                string nameAttributeValue = "";
                foreach (var a in startTag.Attributes)
                {
                    var attrName = a.Name.LocalName.ValueText;
                    var attrValue = string.Join("", a.TextTokens.Select(t => t.ValueText));
                    if ("name".Equals(attrName))
                    {
                        nameAttributeValue = attrValue;
                    }
                    else
                    {
                        // Unexpected attribute.
                        return null;
                    }
                }

                var fixedParamName = NaiveNameFixer(nameAttributeValue);
                var expectedParamComment = string.Format("The {0}.", fixedParamName);
                Console.WriteLine("Expected param comment: '{0}'", expectedParamComment);
                return expectedParamComment;
            }

            if ("returns".Equals(tag))
            {
                var returnTypeName = GetTypeNameForReturnsComment(methodDeclaration);
                if (returnTypeName == null)
                {
                    Console.WriteLine("Unknown content in return tag.");
                    return null;
                }

                var expectedReturnsComment = string.Format("The <see cref=\"{0}\"/>.", returnTypeName);
                Console.WriteLine("Expected returns comment: {0}.", expectedReturnsComment);
                return expectedReturnsComment;
            }

            // Console.WriteLine("Unexpected tag.");

            return null;
        }

        private static SyntaxTrivia GetDocumentationTrivia(MemberDeclarationSyntax p)
        {
            return p.GetLeadingTrivia().FirstOrDefault(t => t.Kind == SyntaxKind.DocumentationCommentTrivia);
        }

        private static bool IsPropertyDocumentationGenerated(PropertyDeclarationSyntax p)
        {
            var predictedPropertySummaryText = GetPredictedPropertySummaryText(p);
            var predictedPropertyValueText = GetPredictedPropertyValueText(p);

            var expected = new Dictionary<string, string>();
            expected["summary"] = predictedPropertySummaryText;
            expected["value"] = predictedPropertyValueText;

            var docTrivia = GetDocumentationTrivia(p);
            var st = docTrivia.GetStructure();
            if (st == null)
            {
                // No XML comments.
                Console.WriteLine("Property {0} has no XML comments.", p.Identifier.ValueText);
                return false;
            }

            var childNodes = st.ChildNodes();
            var maybeXmlElements = childNodes.Where(n => n.Kind == SyntaxKind.XmlElement);
            var xmlElements = maybeXmlElements.Cast<XmlElementSyntax>();

            foreach (XmlElementSyntax e in xmlElements)
            {
                string tag = e.StartTag.Name.LocalName.ValueText;
                if (!expected.ContainsKey(tag))
                {
                    // The XML comments contains an unexpected tag, 
                    // indicating it hasn't been (all) auto-generated.
                    Console.WriteLine("Property {0} has extra tag {1}.", p.Identifier.ValueText, tag);
                    return false;
                }

                string expectedComment = expected[tag];
                string actualComment = ReadTagComment(e.Content);
                if (!string.Equals(expectedComment, actualComment, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Mismatch between expected comment '{0}' and actual comment '{1}'.", expectedComment, actualComment);
                    var dist = LevenshteinDistance.Compute(expectedComment, actualComment);
                    Console.WriteLine("Levenshtein distance: " + dist);
                    return false;
                }
            }

            Console.WriteLine("All the documentation for property {0} was written by a tool.", p.Identifier.ValueText);
            Console.WriteLine();
            
            return true;
        }

        private static string ReadTagComment(SyntaxList<XmlNodeSyntax> content)
        {
            var reader = new TagCommentReader(content);
            return reader.Read();
        }

        private static string GetPredictedPropertyValueText(PropertyDeclarationSyntax p)
        {
            var name = NaiveNameFixer(p.Identifier.ValueText);
            return string.Format("The {0}.", name);
        }

        private static string GetPredictedPropertySummaryText(PropertyDeclarationSyntax p)
        {
            Func<string, bool> hasAccessor =
                accessorName => p.AccessorList.Accessors.Any(ads => accessorName.Equals(ads.Keyword.ValueText));
            bool hasGetter = hasAccessor("get");
            bool hasSetter = hasAccessor("set");

            var fixedName = NaiveNameFixer(p.Identifier.ValueText);

            string summaryText = string.Format("the {0}.", fixedName);
            if (hasGetter && hasSetter)
            {
                return "Gets or sets " + summaryText;
            }

            if (hasGetter)
            {
                return "Gets " + summaryText;
            }

            if (hasSetter)
            {
                return "Sets " + summaryText;
            }

            throw new Exception("This makes no sense.");
        }
    }
}
