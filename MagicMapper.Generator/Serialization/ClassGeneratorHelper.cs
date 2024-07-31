using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace MagicMapper.Generator.Serialization
{
    internal sealed class ClassGeneratorHelper
    {
        private readonly GeneratorExecutionContext _context;
        private IList<ClassElement> _classes = new List<ClassElement>();

        public ClassGeneratorHelper(GeneratorExecutionContext context)
        {
            _context = context;
        }
        
        public ClassElement AddClassElement(ClassElement element)
        {
            ClassElement existingElement = _classes.FirstOrDefault(item => 
                item.Namespace == element.Namespace && item.ClassName == element.ClassName);
            
            if (existingElement != null)
            {
                return existingElement;
            }
            
            _classes.Add(element);
            
            return element;
        }

        public ClassElement AddClassElement(string @namespace, string className)
        {
            return AddClassElement(new ClassElement(@namespace, className));
        }

        public void Generate()
        {
            foreach (ClassElement element in _classes)
            {
                _context.AddSource($"{element.ClassName}.g.cs", SourceText.From(element.ToString(), Encoding.UTF8));
            }
        }
    }
}