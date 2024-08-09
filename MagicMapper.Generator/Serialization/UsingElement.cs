using System;

namespace MagicMapper.Generator.Serialization
{
    internal sealed class UsingElement : Element
    {
        public string Namespace { get; }
        
        public UsingElement(string @namespace)
        {
            Namespace = @namespace;
        }

        public override string ToString()
        {
            return $"using {Namespace};";
        }
    }
}