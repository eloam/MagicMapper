using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MagicMapper.Generator.Serialization;
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

                var source = GenerateMapperMethod(methodSymbol);
                
                context.AddSource($"{methodSymbol.ContainingType.Name}_{Guid.NewGuid()}.g.cs", SourceText.From(source, Encoding.UTF8));
            }
        }

        private string GenerateMapperMethod(IMethodSymbol methodSymbol)
        {
            var sourceVariableName = methodSymbol.Parameters[0].Name;
            var targetVariableName = "result";
            
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
                        mappings.AppendLine($"{targetVariableName}.{targetProperty.Name} = {sourceVariableName}.{sourceProperty.Name};");
                    }
                    else if (targetProperty.Type.TypeKind == TypeKind.Class && !targetProperty.Type.IsValueType)
                    {
                        mappings.AppendLine($"{targetVariableName}.{targetProperty.Name} = {methodSymbol.Name}({sourceVariableName}.{sourceProperty.Name});");
                    }
                }
            }
            

            var source = $@"
        public partial {methodSymbol.ReturnType} {methodSymbol.Name}({sourceType} value)
        {{
            var result = new {methodSymbol.ReturnType}();
            {mappings}
            return result;
        }}
                ";
            
            ClassElement classElement = new ClassElement(@namespace: methodSymbol.ContainingNamespace.Name, className: methodSymbol.ContainingType.Name);
            classElement.AddMethod(source);
            
            source = CSharpSyntaxTree.ParseText(classElement.ToString()).GetRoot().NormalizeWhitespace().ToFullString();
            
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