using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Roslyn.Compilers.CSharp;

namespace Dedox
{
    class GeneratedMethodCommentsChecker : GeneratedCommentsChecker<MethodDeclarationSyntax>
    {
        public GeneratedMethodCommentsChecker(MethodDeclarationSyntax it, IDedoxConfig config, IDedoxMetrics metrics)
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

        protected override void OnMismatch(string tag, string expectedComment, string actualComment)
        {
            if ("returns".Equals(tag))
            {
                const string genericReturnsPattern = @"The <see cref=""(\w+)""/>\.";
                var match = Regex.Match(actualComment, genericReturnsPattern);
                if (match.Success)
                {
                    var typeName = match.Groups[1].Value;
                    
                    Info();
                    Info("{0} {1}", GetCodeElementType(), Name);
                    Info("! The XML comment for the returns tag is probably obsolete.");
                    Info("! Should be {0} but is actually {1}.", GetTypeNameForReturnsComment(), typeName);
                }
            }
        }

        private string GetTypeNameForReturnsComment()
        {
            var ret = It.ReturnType;

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

            Info("Unknown return type kind: " + ret.Kind);
            return null;
        }

        protected override List<Func<string>> GetExpectedCommentForTag(XmlElementStartTagSyntax startTag)
        {
            string tag = startTag.Name.LocalName.ValueText;

            if ("summary".Equals(tag))
            {
                return ExpectedCommentForSummaryTag();
            }

            if ("param".Equals(tag))
            {
                return ExpectedCommentForParamTag(startTag);
            }

            if ("returns".Equals(tag))
            {
                return ExpectedCommentForReturnsTag();
            }

            return new List<Func<string>>();
        }

        private List<Func<string>> ExpectedCommentForReturnsTag()
        {
            var returnTypeName = GetTypeNameForReturnsComment();
            if (returnTypeName == null)
            {
                Info("Unknown content in return tag.");
                return new List<Func<string>>();
            }

            return new List<Func<string>> { () => string.Format("The <see cref=\"{0}\"/>.", returnTypeName) };
        }

        private List<Func<string>> ExpectedCommentForParamTag(XmlElementStartTagSyntax startTag)
        {
            string paramNameFromAttribute = "";
            foreach (var a in startTag.Attributes)
            {
                var attrName = a.Name.LocalName.ValueText;
                var attrValue = string.Join("", a.TextTokens.Select(t => t.ValueText));
                if ("name".Equals(attrName))
                {
                    paramNameFromAttribute = attrValue;
                }
                else
                {
                    // Unexpected attribute.
                    return new List<Func<string>>();
                }
            }

            // Sanity check for attribute value.
            var paramExists =
                It.ParameterList.Parameters.Any(
                    ps =>
                    string.Equals(
                        ps.Identifier.ValueText,
                        paramNameFromAttribute,
                        StringComparison.InvariantCultureIgnoreCase));

            if (!paramExists)
            {
                Info("! The method {0} does not have a parameter named {1}.", It.Identifier.ValueText, paramNameFromAttribute);                
            }

            var decomposedParamName = StyleCopDecompose(paramNameFromAttribute);

            return new List<Func<string>>
                       {
                           () => string.Format("The {0}.", decomposedParamName),
                           () => string.Format("The {0}.", paramNameFromAttribute)
                       };
        }

        private List<Func<string>> ExpectedCommentForSummaryTag()
        {
            // Pattern: The $(decomposed-name).
            // Alternative: The $(name).
            // Alternative: GhostDoc-style conjugation of verb: FooBar => Fooes the bar.
            // Alternative with parameters: GhostDoc-style: FooBar(thing) => Fooes the bar given the specified thing.

            var decomposed = StyleCopDecompose(Name);
            var ghostDocSentence = ToGhostDocSentence(BasicDecompose(Name));

            return new List<Func<string>>
                       {
                           () => string.Format("The {0}.", decomposed),
                           () => string.Format("The {0}.", Name),
                           () => string.Format(ghostDocSentence + ".")
                       };
        }

        private string ToGhostDocSentence(string[] basicDecompose)
        {
            return string.Join(" ", ToGhostDocSentenceParts(basicDecompose));
        }

        private IEnumerable<string> ToGhostDocSentenceParts(string[] decomposed)
        {
            if (decomposed.Length > 0)
            {
                var first = decomposed[0];
                var cap = Char.ToUpperInvariant(first[0]) + first.Substring(1);
                yield return Conjugate(cap);

                if (decomposed.Length > 1)
                {
                    yield return "the";

                    for (int i = 1; i < decomposed.Length; i++)
                    {
                        yield return decomposed[i];
                    }
                }
                else
                {
                    yield return "this";
                    yield return "instance";
                }
            }
        }

        private static string Conjugate(string s)
        {
            return s[s.Length - 1] == 's' ? s : s + "s";
        }

        protected TypeDeclarationSyntax DeclaringCodeElement
        {
            get
            {
                return (TypeDeclarationSyntax)It.Parent;
            }
        }

        protected override bool IsGeneratedCodeElement()
        {
            // Check locally first!
            return IsGeneratedCodeElement(DeclaringCodeElement);
        }
    }


}