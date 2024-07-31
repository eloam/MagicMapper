using System;

namespace MagicMapper.Generator.Serialization
{
    internal sealed class UsingElement : Element
    {
        private readonly string _namespace;
        
        public UsingElement(string @namespace)
        {
            _namespace = @namespace;
        }

        public override string ToString()
        {
            return $"using {_namespace};";
        }
    }
}