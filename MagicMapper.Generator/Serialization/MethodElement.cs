using System;
using System.Text;

namespace MagicMapper.Generator.Serialization
{
    internal sealed class MethodElement : Element
    {
        private readonly StringBuilder _source;
        
        public string Namespace { get; }
        
        public string ClassName { get; }
        
        public MethodElement(string @namespace, string className)
        {
            Namespace = @namespace;
            ClassName = className;
            _source = new StringBuilder();
        }

        public override string ToString()
        {
            var source = $@"
                using System;

                namespace {Namespace}
                {{
                    public partial class {ClassName}
                    {{
                        
                    }}
                }}
            ";

            return SourceFormatter.Format(source);
        }
    }
}