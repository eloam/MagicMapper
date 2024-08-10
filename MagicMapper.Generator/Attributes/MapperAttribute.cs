using System;
using MagicMapper.Generator.Common;

namespace MagicMapper.Generator.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
public sealed class MapperAttribute : Attribute
{
}