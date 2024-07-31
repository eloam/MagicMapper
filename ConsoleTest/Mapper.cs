using MagicMapper;

namespace ConsoleTest;

public partial class Mapper
{
    [Mapper]
    public partial Person Map(PersonDto value);
    
    [Mapper]
    public partial Address Map(AddressDto value);
}





public class Person
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
    public Address Address { get; set; }
}

public class PersonDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
    public AddressDto Address { get; set; }
}


public class Address 
{
    public string Street { get; set; }
    public string City { get; set; }
    public string ZipCode { get; set; }
}

public class AddressDto
{
    public string Street { get; set; }
    public string City { get; set; }
    public string ZipCode { get; set; }
}