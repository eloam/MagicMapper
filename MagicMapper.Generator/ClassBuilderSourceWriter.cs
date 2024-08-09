using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace MagicMapper.Generator.Syntax;

public class ClassBuilderSourceWriter
{
    public IList<MapperClassSyntaxDeclarationBuilder> Classes { get; } = [];

    public MapperClassSyntaxDeclarationBuilder TryAddClass(IMethodSymbol methodSymbol)
    {
        string ns = methodSymbol.ContainingNamespace.Name;
        string className = methodSymbol.ContainingType.Name;

        MapperClassSyntaxDeclarationBuilder mapperClassSyntaxDeclarationBuilder = new MapperClassSyntaxDeclarationBuilder(ns, className);
        MapperClassSyntaxDeclarationBuilder? existingClass =
            Classes.FirstOrDefault(item => item.GetHashCode() == mapperClassSyntaxDeclarationBuilder.GetHashCode());

        MapperClassSyntaxDeclarationBuilder finalMapperClassSyntaxDeclaration = existingClass ?? mapperClassSyntaxDeclarationBuilder;

        if (existingClass == null)
            Classes.Add(finalMapperClassSyntaxDeclaration);

        return finalMapperClassSyntaxDeclaration;
    }
}