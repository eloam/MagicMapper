using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using MagicMapper.Generator.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace MagicMapper.Generator;

[Generator]
internal sealed class MapperIncrementalGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        Debugger.Launch();

        var methodDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (syntaxNode, _) => IsMethodWithMapperAttribute(syntaxNode),
                transform: static (context, _) => GetMethodSymbol(context))
            .Where(static methodSymbol => methodSymbol is not null);

        context.RegisterSourceOutput(methodDeclarations, (sourceContext, methodSymbols) =>
        {
            ClassBuilderSourceFactory classBuilderSourceFactory = new ClassBuilderSourceFactory();
            foreach (IMethodSymbol methodSymbol in methodSymbols)
            {
                ClassBuilderSource classBuilderSource = classBuilderSourceFactory.FindOrCreateClass(methodSymbol);
                classBuilderSource.AddMapperMethod(methodSymbol);
            }

#if !NOSOURCE
            foreach (ClassBuilderSource? classDeclaration in classBuilderSourceFactory.Classes)
            {
                sourceContext.AddSource($"{classDeclaration.Name}.g.cs", SourceText.From(classDeclaration.Build(), Encoding.UTF8));
            }
#endif
        });
    }

    private static bool IsMethodWithMapperAttribute(SyntaxNode syntaxNode)
    {
        return syntaxNode switch
        {
            MethodDeclarationSyntax methodDeclarationSyntax => HasMapperAttribute(methodDeclarationSyntax.AttributeLists),
            ClassDeclarationSyntax classDeclarationSyntax => HasMapperAttribute(classDeclarationSyntax.AttributeLists),
            _ => false
        };
    }
    
    private static bool HasMapperAttribute(SyntaxList<AttributeListSyntax> attributes)
    {
        return attributes.SelectMany(attrList => attrList.Attributes).Any(attrSyntax => attrSyntax.Name.ToString() == "Mapper");
    }

    private static IEnumerable<IMethodSymbol> GetMethodSymbol(GeneratorSyntaxContext context)
    {
        List<IMethodSymbol> methodSymbols = [];
        
        switch (context.Node)
        {
            case MethodDeclarationSyntax methodDeclarationSyntax:
            {
                methodSymbols.AddIfNotNull(item: CandidateMethod(context, methodDeclarationSyntax));
                break;
            }
            case ClassDeclarationSyntax classDeclarationSyntax:
            {
                foreach (MethodDeclarationSyntax methodDeclarationSyntax in classDeclarationSyntax.Members.OfType<MethodDeclarationSyntax>())
                {
                    methodSymbols.AddIfNotNull(item: CandidateMethod(context, methodDeclarationSyntax));
                }
                break;
            }
        }
        
        return methodSymbols;
    }
    
    private static IMethodSymbol? CandidateMethod(GeneratorSyntaxContext context, MethodDeclarationSyntax methodDeclarationSyntax)
    {
        // Is a public or internal method
        if (!methodDeclarationSyntax.Modifiers.Any(token => token.IsKind(SyntaxKind.PublicKeyword) || token.IsKind(SyntaxKind.InternalKeyword))) 
            return null;
        
        // Is a public or internal class
        if (methodDeclarationSyntax.Parent is ClassDeclarationSyntax classDeclarationSyntax && 
            !classDeclarationSyntax.Modifiers.Any(token => token.IsKind(SyntaxKind.PublicKeyword) || token.IsKind(SyntaxKind.InternalKeyword))) 
            return null;
        
        // Is a partial method
        if(!methodDeclarationSyntax.Modifiers.Any(SyntaxKind.PartialKeyword))
            return null;
        
        if (IsNotAllowedType(methodDeclarationSyntax.ReturnType))
            return null;
        
        IMethodSymbol? methodSymbol = ModelExtensions.GetDeclaredSymbol(context.SemanticModel, methodDeclarationSyntax) as IMethodSymbol;
        
        if (methodSymbol is null)
            return null;

        if (!IsCustomType(methodSymbol.ReturnType))
            return null;
        
        return methodSymbol;
    }
    
    private static bool IsNotAllowedType(TypeSyntax typeSyntax)
    {
        var primitiveTypes = new HashSet<string>
        {
            "bool", "byte", "sbyte", "char", "decimal", "double", "float", "int", "uint", "long", "ulong", "short", "ushort", "string", "void"
        };

        return primitiveTypes.Contains(typeSyntax.ToString());
    }
    
    private static bool IsCustomType(ITypeSymbol typeSymbol)
    {
        return typeSymbol.TypeKind is TypeKind.Class or TypeKind.Struct;
    }
}