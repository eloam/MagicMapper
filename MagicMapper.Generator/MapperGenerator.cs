using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MagicMapper.Generator.Serialization;
using MagicMapper.Generator.Syntax;
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
            Debugger.Launch();
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (!(context.SyntaxReceiver is SyntaxReceiver receiver))
                return;

            var attributeSymbol = context.Compilation.GetTypeByMetadataName("MagicMapper.MapperAttribute");

            ClassGeneratorHelper classGeneratorHelper = new ClassGeneratorHelper(context);
            
            ClassBuilderSourceWriter classBuilderSourceWriter = new ClassBuilderSourceWriter();

            foreach (var method in receiver.CandidateMethods)
            {
                var model = context.Compilation.GetSemanticModel(method.SyntaxTree);
                var methodSymbol = model.GetDeclaredSymbol(method) as IMethodSymbol;
                
                if (methodSymbol == null)
                    continue;
                
                MapperClassSyntaxDeclarationBuilder classBuilder = classBuilderSourceWriter.TryAddClass(methodSymbol);
                classBuilder.AddMapperMethod(methodSymbol);
            }

            foreach (MapperClassSyntaxDeclarationBuilder classDeclaration in classBuilderSourceWriter.Classes)
            {
                context.AddSource($"{classDeclaration.Name}.g.cs", SourceText.From(classDeclaration.Build(), Encoding.UTF8));
            }
            
            classGeneratorHelper.Generate();
        }
        

        class SyntaxReceiver : ISyntaxReceiver
        {
            public List<MethodDeclarationSyntax> CandidateMethods { get; } = [];

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