using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;

using Roslyn.Compilers.CSharp;

namespace Dedox
{
    abstract class GeneratedCommentsChecker<T> : IGeneratedCommentsChecker, IConsoleWriter where T : SyntaxNode
    {
        protected readonly T It;

        private readonly IDedoxConfig _config;
        private readonly IDedoxMetrics _metrics;
        private readonly IConsoleWriter _writer;

        protected GeneratedCommentsChecker(T it, IDedoxConfig config, IDedoxMetrics metrics)
        {
            It = it;

            _config = config;
            _writer = config.Writer;
            _metrics = metrics;
        }

        protected abstract string Name
        {
            get;
        }

        protected string GetCodeElementType()
        {
            var s = It.Kind.ToString();
            return s.Substring(0, s.IndexOf("Declaration", StringComparison.Ordinal));
        }

        protected SyntaxTrivia GetDocumentationTrivia()
        {
            return It.GetLeadingTrivia().FirstOrDefault(t => t.Kind == SyntaxKind.DocumentationCommentTrivia);
        }

        protected string ReadTagComment(SyntaxList<XmlNodeSyntax> content)
        {
            var reader = new TagCommentReader(content);
            return reader.Read();
        }

        public void Info(string format, params object[] args)
        {
            _writer.Info(format, args);
        }

        public void Info()
        {
            _writer.Info();
        }

        public void Debug(string format, params object[] args)
        {
            _writer.Debug(format, args);
        }

        public void Debug()
        {
            _writer.Debug();
        }

        public bool IsGenerated()
        {
            _metrics.IncrementAllCodeElements();

            if (SkipCodeElement())
            {
                return false;
            }

            _metrics.IncrementCodeElements();
            
            var elemType = GetCodeElementType();

            var docTrivia = GetDocumentationTrivia();
            var st = docTrivia.GetStructure();
            if (st == null)
            {
                // No XML comments.
                Debug();
                Debug("{0} {1}", elemType, Name);
                Debug("No XML comments.");

                return false;
            }

            _metrics.IncrementCodeElementsWithDocumentation();

            var childNodes = st.ChildNodes();
            var maybeXmlElements = childNodes.Where(n => n.Kind == SyntaxKind.XmlElement);
            var xmlElements = maybeXmlElements.Cast<XmlElementSyntax>();

            bool hasSummaryTag = false;

            foreach (XmlElementSyntax e in xmlElements)
            {
                string actualComment = ReadTagComment(e.Content);

                XmlElementStartTagSyntax startTag = e.StartTag;
                string tag = startTag.Name.LocalName.ValueText;
                if ("summary".Equals(tag))
                {
                    hasSummaryTag = true;
                }

                var commentGenerators = GetExpectedCommentForTag(startTag);

                var levenshteins = new List<Tuple<string, int>>();
                bool found = false;
                foreach (var generator in commentGenerators)
                {
                    if (!found)
                    {
                        var expectedComment = generator();

                        if (expectedComment != null)
                        {
                            string lowerExpected = expectedComment.ToLowerInvariant();
                            string lowerActual = actualComment.ToLowerInvariant();
                            if (string.Equals(lowerExpected, lowerActual))
                            {
                                found = true;
                            }
                            else
                            {
                                int diffLength = actualComment.Length - expectedComment.Length;
                                if (diffLength <= _config.LevenshteinLimit)
                                {
                                    var distance = LevenshteinDistance.Compute(lowerExpected, lowerActual);
                                    levenshteins.Add(Tuple.Create(expectedComment, distance));                                    
                                }
                                OnMismatch(tag, expectedComment, actualComment);
                            }
                        }
                    }
                }

                if (!found)
                {
                    Info();
                    Info("{0} {1}", elemType, Name);
                    Info("Unable to reproduce the comment for tag '{0}'.", tag);
                    Info("The comment was '{0}'", actualComment);
                    
                    var best = levenshteins.OrderBy(t => t.Item2).FirstOrDefault();
                    if (best == null)
                    {
                        Info("The generated guesses were not even close.");
                    }
                    else
                    {
                        var bestComment = best.Item1;
                        var bestDistance = best.Item2;

                        Info("Best guess has Levenshtein distance {1}: '{0}'", bestComment, bestDistance);

                        if (bestDistance <= _config.LevenshteinLimit)
                        {
                            Info("The Levenshtein distance is within the specified limit.");
                            found = true;
                        }
                    }
                }

                if (!found)
                {
                    Info("No acceptable guess found.");
                    return false;
                }

                Debug();
                Debug("{0} {1}", elemType, Name);
                Debug("Reproduced comment for tag {0}: '{1}'", tag, actualComment);
            }

            if (!hasSummaryTag)
            {
                Debug();
                Debug("{0} {1}", elemType, Name);
                Debug("No summary tag found.");
                return false;
            }

            _metrics.IncrementCodeElementsWithGeneratedDocumentation();

            Info();
            Info("{0} {1}", elemType, Name);
            Info("All the documentation was written by a tool.");
            
            return true;
        }

        private bool SkipCodeElement()
        {
            return !_config.IncludeGeneratedCode && IsGeneratedCodeElement();
        }

        protected bool IsGeneratedCodeElement(Func<SyntaxList<AttributeListSyntax>> attributeGetter)
        {
            const string genCodeName = "GeneratedCode";

            var attrNames = attributeGetter().SelectMany(attrList => attrList.Attributes).Select(a => a.Name).ToList();

            var qaNames = attrNames
                .Where(n => n.Kind == SyntaxKind.QualifiedName)
                .Cast<QualifiedNameSyntax>()
                .Select(n => n.Right.Identifier);
            if (qaNames.Any(n => n.ValueText == genCodeName))
            {
                Debug("Code element {0} {1} is annotated with the GeneratedCode attribute.", GetCodeElementType(), Name);
                return true;
            }

            var idNames = attrNames
                .Where(n => n.Kind == SyntaxKind.IdentifierName)
                .Cast<IdentifierNameSyntax>()
                .Select(n => n.Identifier);

            if (idNames.Any(n => n.ValueText == genCodeName))
            {
                Debug("Code element {0} {1} is annotated with the GeneratedCode attribute.", GetCodeElementType(), Name);
                return true;
            }

            return false;
        }   

        protected bool IsGeneratedCodeElement(TypeDeclarationSyntax declaration)
        {
            return IsGeneratedCodeElement(() => declaration.AttributeLists);
        }

        protected virtual bool IsGeneratedCodeElement()
        {
            return false;
        }

        protected virtual void OnMismatch(string tag, string expectedComment, string actualComment)
        {
        }

        // TODO: Make this entirely pattern-driven. 
        // Select patterns based on code element and tag. 
        // Patterns are sorted in order of priority.
        // Patterns are Func<SOMETHING (everything needed to build pattern), string>.
        // SOMETHING must contain (funcs to get) name, decomposed name, declaring type...
        protected abstract List<Func<string>> GetExpectedCommentForTag(XmlElementStartTagSyntax startTag);
       
        protected string SplitCamelCase(string input)
        {
            // Better expression: "(?<=[a-z])([A-Z])"
            return Regex.Replace(input, "([A-Z])", " $1", RegexOptions.Compiled).Trim();
        }

        protected string StyleCopDecompose(string input)
        {
            var lowercased = SplitCamelCase(input).Split(' ').Select(s => s.ToLower());
            return string.Join(" ", lowercased);
        }
    }
}