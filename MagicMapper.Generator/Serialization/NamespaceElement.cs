using System;

namespace MagicMapper.Generator.Serialization
{
    internal sealed class NamespaceElement : Element
    {
        private string _source = null;
        
        public string Namespace { get; }
        
        public NamespaceElement(string @namespace)
        {
            Namespace = @namespace;
        }

        public NamespaceElement UseBlockSyntax(string source)
        {
            _source = source;
            return this;
        }
        
        public override string ToString()
        {
            string output = $"namespace {Namespace}";

            if (_source != null)
            {
                output += $@"
                    {{
                        {_source}
                    }}                 
                ";
            }
            else
            {
                output += ";";
            }
            
            return output;
        }
    }
}