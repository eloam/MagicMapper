using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicMapper.Generator.Serialization
{
    internal sealed class ObjectElement : Element
    {
        public string Name { get; }
        
        public IDictionary<string, string> Properties { get; } = new Dictionary<string, string>();
        
        public ObjectElement(string name)
        {
            Name = name;
        }

        public void AddProperty(string propertyName, string value)
        {
            Properties.Add(propertyName, value);
        }

        private string GenerateProperties()
        {
            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<string, string> property in Properties)
            {
                string mappingString = $@"{property.Key} = {property.Value}";
                
                if (!Properties.Last().Equals(property))
                    mappingString += ",";
                
                builder.AppendLine(mappingString);
            }

            return builder.ToString();
        }

        public override string ToString()
        {
            return $@"
                new {Name}()
                {{
                    {GenerateProperties()}
                }}
            ";
        }
    }
}