using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicMapper.Generator.Serialization
{
    internal sealed class ClassElement : Element
    {
        private readonly string _namespace;
        private readonly string _className;
        private readonly StringBuilder _source;
        private readonly List<UsingElement> _usings;
        private readonly List<string> _methods;
        
        public ClassElement(string @namespace, string className)
        {
            _namespace = @namespace;
            _className = className;
            _source = new StringBuilder();
            _usings = new List<UsingElement>();
            _methods = new List<string>();
        }
        
        public ClassElement AddUsing(UsingElement element)
        {
            _usings.Add(element);
            return this;
        }

        public ClassElement AddMethod(string element)
        {
            _methods.Add(element);
            return this;
        }
        
        private string GenerateUsings()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (UsingElement @using in _usings)
            {
                stringBuilder.AppendLine(@using.ToString());
            }

            return stringBuilder.ToString();
        }
        
        private string GenerateMethods()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string method in _methods)
            {
                stringBuilder.AppendLine(method.ToString());
                if (_methods.Last() == method)
                    stringBuilder.AppendLine();
            }

            return stringBuilder.ToString();
        }
        
  
        public override string ToString()
        {
            var source = $@"
                {new UsingElement("System")}
                {GenerateUsings()}
                
                {new NamespaceElement(_namespace)}

                public partial class {_className}
                {{
                    {GenerateMethods()}
                }}
            ";

            return SourceFormatter.Format(source);
        }
    }
}