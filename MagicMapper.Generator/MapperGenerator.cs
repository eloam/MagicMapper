using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

            ClassBuilderSourceFactory classBuilderSourceFactory = new ClassBuilderSourceFactory();

            foreach (var method in receiver.CandidateMethods)
            {
                var model = context.Compilation.GetSemanticModel(method.SyntaxTree);
                var methodSymbol = model.GetDeclaredSymbol(method) as IMethodSymbol;

                if (methodSymbol == null)
                    continue;

                ClassBuilderSource classBuilderSource = classBuilderSourceFactory.TryAddClass(methodSymbol);
                classBuilderSource.AddMapperMethod(methodSymbol);
            }

            foreach (ClassBuilderSource classDeclaration in classBuilderSourceFactory.Classes)
            {
                context.AddSource($"{classDeclaration.Name}.g.cs", SourceText.From(classDeclaration.Build(), Encoding.UTF8));
            }
        }

        class SyntaxReceiver : ISyntaxReceiver
        {
            public List<MethodDeclarationSyntax> CandidateMethods { get; } = [];

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                Func<AttributeListSyntax, bool> visitorAttribute = attrList => attrList.Attributes.Any(attr => attr.Name.ToString() == "Mapper");

                if (syntaxNode is MethodDeclarationSyntax methodDeclarationSyntax && methodDeclarationSyntax.AttributeLists.Any(visitorAttribute))
                {
                    CandidateMethods.Add(methodDeclarationSyntax);
                }

                if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax && classDeclarationSyntax.AttributeLists.Any(visitorAttribute))
                {
                    foreach (var method in classDeclarationSyntax.Members.OfType<MethodDeclarationSyntax>())
                    {
                        CandidateMethods.Add(method);
                    }
                }
            }
        }
    }
}