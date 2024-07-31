namespace MagicMapper.Generator.Serialization
{
    internal sealed class NamespaceElement : Element
    {
        private readonly string _namespace;
        private string _source = null;

        public NamespaceElement(string @namespace)
        {
            _namespace = @namespace;
        }

        public NamespaceElement UseBlockSyntax(string source)
        {
            _source = source;
            return this;
        }
        
        public override string ToString()
        {
            string output = $"namespace {_namespace}";

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