using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MagicMapper.Generator.Syntax;

public class ObjectMapper
{
    private readonly IMethodSymbol _methodSymbol;
    
    public event EventHandler<INamespaceSymbol[]>? NamespaceAdded;

    public ObjectMapper(IMethodSymbol methodSymbol)
    {
        _methodSymbol = methodSymbol;
    }

    public StatementSyntax Map()
    {
        if (_methodSymbol.ContainingType.TypeKind != TypeKind.Class)
            throw new Exception("Only classes are supported");

        IParameterSymbol inputSymbol = _methodSymbol.Parameters.FirstOrDefault() ?? throw new Exception("No parameter found");

        ITypeSymbol outputType = _methodSymbol.ReturnType;

        ObjectCreationExpressionSyntax objectCreation = MapObject(inputSymbol.Name, inputSymbol.Type, outputType);

        return SyntaxFactory.ReturnStatement(objectCreation);
    }

    private ObjectCreationExpressionSyntax MapObject(string inputSymbolName, ITypeSymbol inputType, ITypeSymbol outputType)
    {
        OnNamespaceAdded([inputType.ContainingNamespace, outputType.ContainingNamespace]);
        
        AssignmentExpressionSyntax?[] assignments = outputType.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(outputProperty => outputProperty.SetMethod != null)
            .Select(outputProperty => AssignProperty(inputSymbolName, inputType, outputProperty))
            .Where(assignment => assignment != null)
            .ToArray();

        ObjectCreationExpressionSyntax objectCreation = SyntaxFactory.ObjectCreationExpression(SyntaxFactory.ParseTypeName(outputType.Name))
            .WithInitializer(SyntaxFactory.InitializerExpression(SyntaxKind.ObjectInitializerExpression,
                SyntaxFactory.SeparatedList<ExpressionSyntax>((assignments ?? throw new Exception("Assignments array is null."))!)));

        return objectCreation;
    }

    private AssignmentExpressionSyntax? AssignProperty(string inputSymbolName, ITypeSymbol inputTypeSymbol, IPropertySymbol outputProperty)
    {
        IPropertySymbol? inputProperty = inputTypeSymbol.GetMembers()
            .OfType<IPropertySymbol>()
            .FirstOrDefault(p => p.Name == outputProperty.Name);

        if (inputProperty == null) return null;

        ExpressionSyntax valueExpression = IsAssignableObject(inputProperty, outputProperty)
            ? MapObject($"{inputSymbolName}.{inputProperty.Name}", inputProperty.Type, outputProperty.Type)
            : SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName(inputSymbolName),
                SyntaxFactory.IdentifierName(inputProperty.Name));

        return SyntaxFactory.AssignmentExpression(
            SyntaxKind.SimpleAssignmentExpression,
            SyntaxFactory.IdentifierName(outputProperty.Name),
            valueExpression
        );
    }
    
    private bool IsAssignableObject(IPropertySymbol inputProperty, IPropertySymbol outputProperty)
    {
        return inputProperty.Type.Equals(outputProperty.Type, SymbolEqualityComparer.Default) == false &&
               (inputProperty.Type.TypeKind is TypeKind.Class or TypeKind.Struct && 
                outputProperty.Type.TypeKind is TypeKind.Class or TypeKind.Struct);
    }
    
    protected virtual void OnNamespaceAdded(INamespaceSymbol[] namespaceSymbols)
    {
        NamespaceAdded?.Invoke(this, namespaceSymbols);
    }
}