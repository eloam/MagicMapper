namespace MagicMapper;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
public class UnderlyingTypeAttribute<TInput, TOutput> : Attribute 
    where TInput : new() 
    where TOutput : new()
{
    public Type InputType { get; private set; } = typeof(TInput);

    public Type OutputType { get; private set; } = typeof(TOutput);
}