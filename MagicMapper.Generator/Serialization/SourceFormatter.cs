using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace MagicMapper.Generator.Serialization
{
    internal static class SourceFormatter
    {
        public static string Format(string source)
        {
            return CSharpSyntaxTree
                .ParseText(source)
                .GetRoot()
                .NormalizeWhitespace()
                .ToFullString();
        }
    }
}