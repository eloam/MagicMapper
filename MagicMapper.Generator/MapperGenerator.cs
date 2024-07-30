using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace MagicMapper.Generator
{
    [Generator]
    public class MapperGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            Debugger.Launch();
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (!(context.SyntaxReceiver is SyntaxReceiver receiver))
                return;

            var attributeSymbol = context.Compilation.GetTypeByMetadataName("MagicMapper.MapperAttribute");

            foreach (var method in receiver.CandidateMethods)
            {
                var model = context.Compilation.GetSemanticModel(method.SyntaxTree);
                var methodSymbol = ModelExtensions.GetDeclaredSymbol(model, method) as IMethodSymbol;

                if (methodSymbol == null || !methodSymbol.GetAttributes().Any(ad => ad.AttributeClass.Equals(attributeSymbol, SymbolEqualityComparer.Default)))
                    continue;

                var source = GenerateMapperClass(methodSymbol);
                context.AddSource($"{methodSymbol.ContainingType.Name}_{Guid.NewGuid()}.g.cs", SourceText.From(source, Encoding.UTF8));
            }
        }

        private string GenerateMapperClass(IMethodSymbol methodSymbol)
        {
            var sourceType = methodSymbol.Parameters[0].Type;
            var targetType = methodSymbol.ReturnType;

            var sourceProperties = sourceType.GetMembers().OfType<IPropertySymbol>();
            var targetProperties = targetType.GetMembers().OfType<IPropertySymbol>();

            var mappings = new StringBuilder();
            foreach (var sourceProperty in sourceProperties)
            {
                var targetProperty = targetProperties.FirstOrDefault(p => p.Name == sourceProperty.Name);
                if (targetProperty != null)
                {
                    if (targetProperty.Type.Equals(sourceProperty.Type, SymbolEqualityComparer.Default))
                    {
                        mappings.AppendLine($"result.{targetProperty.Name} = value.{sourceProperty.Name};");
                    }
                    else if (targetProperty.Type.TypeKind == TypeKind.Class && !targetProperty.Type.IsValueType)
                    {
                        mappings.AppendLine($"result.{targetProperty.Name} = Map(value.{sourceProperty.Name});");
                    }
                }
            }

            var source = $@"
using System;

namespace {methodSymbol.ContainingNamespace}
{{
    public partial class {methodSymbol.ContainingType.Name}
    {{
        public partial {methodSymbol.ReturnType} {methodSymbol.Name}({sourceType} value)
        {{
            var result = new {methodSymbol.ReturnType}();
            {mappings}
            return result;
        }}
    }}
}}
                ";
            
            source = CSharpSyntaxTree.ParseText(source).GetRoot().NormalizeWhitespace().ToFullString();
            
            return source;
        }

        class SyntaxReceiver : ISyntaxReceiver
        {
            public List<MethodDeclarationSyntax> CandidateMethods { get; } = new List<MethodDeclarationSyntax>();

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is MethodDeclarationSyntax methodDeclarationSyntax &&
                    methodDeclarationSyntax.AttributeLists.Any(attrList =>
                        attrList.Attributes.Any(attr =>
                            attr.Name.ToString() == "Mapper")))
                {
                    CandidateMethods.Add(methodDeclarationSyntax);
                }
            }
        }
    }
}