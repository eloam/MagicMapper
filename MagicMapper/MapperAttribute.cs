namespace ConsoleTest
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class MapperAttribute : Attribute
    {
        public MapperAttribute() { }
    }
}