using System;
using System.Collections.Generic;
using System.IO;
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

                var commentGenerators = new Func<string>[]
                                            {
                                                () => GetExpectedCommentForTag(startTag, StyleCopDecompose),
                                                () => GetExpectedCommentForTag(startTag)
                                            };

                var levenshteins = new List<Tuple<string, int>>();
                bool found = false;
                foreach (var generator in commentGenerators)
                {
                    if (!found)
                    {
                        // Particular
                        var expectedComment = generator();

                        if (expectedComment != null)
                        {
                            if (string.Equals(expectedComment, actualComment, StringComparison.OrdinalIgnoreCase))
                            {
                                found = true;
                            }
                            else
                            {
                                var distance = LevenshteinDistance.Compute(expectedComment, actualComment);
                                levenshteins.Add(Tuple.Create(expectedComment, distance));
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
                    if (best != null)
                    {
                        var bestComment = best.Item1;
                        var bestDistance = best.Item2;

                        Info("Best comment has Levenshtein distance {1}: '{0}'", bestComment, bestDistance);

                        if (bestDistance <= _config.LevenshteinLimit)
                        {
                            Info("The Levenshtein distance is within the specified limit.");
                            found = true;
                        }
                    }
                }

                if (!found)
                {
                    Info("No acceptable comment found.");
                    return false;
                }
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
            Info("The documentation was written by a tool.");

            return true;
        }

        protected virtual void OnMismatch(string tag, string expectedComment, string actualComment)
        {
        }

        // TODO: Make this entirely pattern-driven. 
        // Select patterns based on code element and tag. 
        // Patterns are sorted in order of priority.
        // Patterns are Func<SOMETHING (everything needed to build pattern), string>.
        // SOMETHING must contain (funcs to get) name, decomposed name, declaring type...
        protected virtual string GetExpectedCommentForTag(XmlElementStartTagSyntax startTag)
        {
            return GetExpectedCommentForTag(startTag, n => n);
        }

        protected abstract string GetExpectedCommentForTag(XmlElementStartTagSyntax startTag, Func<string, string> nameTransform);

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