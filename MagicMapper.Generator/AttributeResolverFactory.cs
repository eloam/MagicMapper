using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;


namespace MagicMapper.Generator;

internal static class AttributeResolverFactory
{
    private static AttributeData? GetAttributeData<T>(ISymbol symbol) where T : Attribute
    {
        return symbol.GetAttributes().FirstOrDefault(
            attr => attr.AttributeClass?.Name == typeof(T).Name &&
                    attr.AttributeClass.ContainingNamespace.ToDisplayString() == typeof(T).Namespace);
    }
    
    private static object?[] GetConstructorArguments(AttributeData attributeData)
    {
        return attributeData.ConstructorArguments.Select(arg => arg.Value).ToArray();
    }

    public static T? CreateAttribute<T>(IMethodSymbol methodSymbol) where T : Attribute
    {
        try
        {
            AttributeData? attributeData = GetAttributeData<T>(methodSymbol) ?? GetAttributeData<T>(methodSymbol.ContainingType);
            if (attributeData != null)
            {
                object?[] constructorArgs = GetConstructorArguments(attributeData);
                return (T?)Activator.CreateInstance(typeof(T), constructorArgs);
            }
        }
        finally
        {
            Debug.WriteLine($"Couldn't create `{typeof(T).Name}` attribute.");
        }

        return null;
    }
}