using System.Text;

namespace MagicMapper.Generator.Serialization
{
    internal sealed class MethodElement : Element
    {
        private readonly string _namespace;
        private readonly string _className;
        private readonly StringBuilder _source;
        
        public MethodElement(string @namespace, string className)
        {
            _namespace = @namespace;
            _className = className;
            _source = new StringBuilder();
        }

        public override string ToString()
        {
            var source = $@"
                using System;

                namespace {_namespace}
                {{
                    public partial class {_className}
                    {{
                        
                    }}
                }}
            ";

            return SourceFormatter.Format(source);
        }
    }
}