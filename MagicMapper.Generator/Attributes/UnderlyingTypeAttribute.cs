using System;
using MagicMapper.Generator.Common;

namespace MagicMapper.Generator.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
public sealed class UnderlyingTypeAttribute<TInput, TOutput> : Attribute 
    where TInput : new() 
    where TOutput : new()
{
    public Type InputType { get; private set; } = typeof(TInput);

    public Type OutputType { get; private set; } = typeof(TOutput);
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
public sealed class UnderlyingTypeTestAttribute(Type inputType, Type outputType): Attribute
{
    public FullyQualifiedName InputType { get; set; } = inputType;

    public FullyQualifiedName OutputType { get; set; } = outputType;
}