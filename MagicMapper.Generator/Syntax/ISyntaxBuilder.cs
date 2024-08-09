using System.Diagnostics.Contracts;

namespace MagicMapper.Generator.Syntax;

/// <summary>
/// Interface for building syntax representations.
/// </summary>
public interface ISyntaxBuilder
{
    /// <summary>
    /// Builds the syntax representation and returns it as a string.
    /// </summary>
    /// <returns>A string representing the built syntax.</returns>
    [Pure]
    public string Build();
}