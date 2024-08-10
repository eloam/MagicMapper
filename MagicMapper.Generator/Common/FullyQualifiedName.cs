using System;
using System.Text;

namespace MagicMapper.Generator.Common;

public sealed class FullyQualifiedName
{
    public string Name { get; private set; }

    public FullyQualifiedName(Type type)
    {
        Name = GetFullyQualifiedName(type);
    }
    
    private string GetFullyQualifiedName(Type type)
    {
        if (type == null)
            throw new ArgumentNullException(nameof(type));

        StringBuilder fullyQualifiedNameBuilder = new StringBuilder();

        if (!string.IsNullOrEmpty(type.Namespace))
        {
            fullyQualifiedNameBuilder.Append(type.Namespace).Append(".");
        }

        if (type.IsNested)
        {
            fullyQualifiedNameBuilder.Append(GetFullyQualifiedName(type.DeclaringType)).Append("+");
        }

        fullyQualifiedNameBuilder.Append(type.Name);

        if (type.IsGenericType)
        {
            AppendGenericArguments(fullyQualifiedNameBuilder, type.GetGenericArguments());
        }

        return fullyQualifiedNameBuilder.ToString();
    }

    private void AppendGenericArguments(StringBuilder builder, Type[] genericArguments)
    {
        builder.Append("<");

        for (int i = 0; i < genericArguments.Length; i++)
        {
            if (i > 0)
                builder.Append(", ");

            builder.Append(GetFullyQualifiedName(genericArguments[i]));
        }

        builder.Append(">");
    }
    
    public static implicit operator FullyQualifiedName(Type type)
    {
        return new FullyQualifiedName(type);
    }
}