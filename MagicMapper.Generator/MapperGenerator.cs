using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace MagicMapper.Generator
{
    [Generator]
    public class MapperGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (!(context.SyntaxReceiver is SyntaxReceiver))
                return;
                
            SyntaxReceiver receiver = (SyntaxReceiver)context.SyntaxReceiver;
            
            INamedTypeSymbol attributeSymbol = context.Compilation.GetTypeByMetadataName("MagicMapper.MapperAttribute");
            
            foreach (var method in receiver.CandidateMethods)
            {
                SemanticModel model = context.Compilation.GetSemanticModel(method.SyntaxTree);
                IMethodSymbol methodSymbol = model.GetDeclaredSymbol(method) as IMethodSymbol;

                if (methodSymbol == null/* || !methodSymbol.GetAttributes().Any(ad => ad.AttributeClass.Equals(attributeSymbol, SymbolEqualityComparer.Default))*/)
                    continue;

                string source = GenerateMapperClass(methodSymbol);
                context.AddSource($"{methodSymbol.ContainingType.Name}.g.cs", SourceText.From(source, Encoding.UTF8));
            }
        }

        private string GenerateMapperClass(IMethodSymbol methodSymbol)
        {
            ITypeSymbol sourceType = methodSymbol.Parameters[0].Type;
            ITypeSymbol targetType = methodSymbol.ReturnType;

            IEnumerable<IPropertySymbol> sourceProperties = sourceType.GetMembers().OfType<IPropertySymbol>();
            IEnumerable<IPropertySymbol> targetProperties = targetType.GetMembers().OfType<IPropertySymbol>();

            StringBuilder mappings = new StringBuilder();
            foreach (IPropertySymbol sourceProperty in sourceProperties)
            {
                IPropertySymbol targetProperty = targetProperties.FirstOrDefault(p => p.Name == sourceProperty.Name);
                if (targetProperty != null && targetProperty.Type.Equals(sourceProperty.Type, SymbolEqualityComparer.Default))
                {
                    mappings.AppendLine($"            result.{targetProperty.Name} = value.{sourceProperty.Name};");
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