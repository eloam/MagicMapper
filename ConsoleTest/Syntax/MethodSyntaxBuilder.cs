using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MagicMapper.Generator.Syntax;

internal sealed class MethodSyntaxBuilder : ISyntaxBuilder
{
    private readonly HashSet<string> _methods = [];
    
    public MethodSyntaxBuilder()
    {
        
    }

    public void Add(string methodName, Type[] inputTypes,  Type returnType, string body)
    {
        List<ParameterSyntax> parameters = [];
        foreach (Type inputType in inputTypes)
        {
            ParameterSyntax parameter = SyntaxFactory.Parameter(
                    SyntaxFactory.Identifier(inputType.Name.ToLower())).WithType(SyntaxFactory.ParseTypeName(inputType.Name));
            parameters.Add(parameter);
        }
        
        var methodDeclaration = SyntaxFactory.MethodDeclaration(
            SyntaxFactory.ParseTypeName(returnType.Name),
            identifier: methodName
        ).AddParameterListParameters(parameters.ToArray());
        
        _methods.Add(methodDeclaration.ToFullString());
    }
    
    public void AddMethod(IMethodSymbol methodSymbol)
    {
        var methodDeclaration = SyntaxFactory.MethodDeclaration(
            SyntaxFactory.ParseTypeName(methodSymbol.ReturnType.Name),
            identifier: methodSymbol.Name
        ).AddParameterListParameters(methodSymbol.Parameters.Select(p => SyntaxFactory.Parameter(
            SyntaxFactory.Identifier(p.Name)).WithType(SyntaxFactory.ParseTypeName(p.Type.Name))).ToArray());
        
        _methods.Add(methodDeclaration.ToFullString());
    }

    public string Build()
    {
        return string.Join(SyntaxFactory.CarriageReturnLineFeed.ToFullString(), _methods);
    }
}