using System.Collections.Generic;
using System.Linq;
using MagicMapper.Generator.Syntax;
using Microsoft.CodeAnalysis;

namespace MagicMapper.Generator;

public class ClassBuilderSourceFactory
{
    public IList<ClassBuilderSource> Classes { get; } = [];

    public ClassBuilderSource TryAddClass(IMethodSymbol methodSymbol)
    {
        string ns = methodSymbol.ContainingNamespace.Name;
        string className = methodSymbol.ContainingType.Name;

        ClassBuilderSource classBuilderSource = new ClassBuilderSource(ns, className);
        ClassBuilderSource? existingClass =
            Classes.FirstOrDefault(item => item.GetHashCode() == classBuilderSource.GetHashCode());

        ClassBuilderSource finalClass = existingClass ?? classBuilderSource;

        if (existingClass == null)
            Classes.Add(finalClass);

        return finalClass;
    }
}