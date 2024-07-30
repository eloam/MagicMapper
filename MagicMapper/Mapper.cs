

using ConsoleTest;

namespace MagicMapper;

public partial class Mapper
{
    [Mapper]
    public partial Person Map(PersonDto value);
}

public class Person
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    
    public int Age { get; set; }
}

public class PersonDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    
    public int Age { get; set; } 
}
