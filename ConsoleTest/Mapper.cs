using MagicMapper;

namespace ConsoleTest;

public partial class Mapper
{
    [Mapper]
    [UnderlyingType<AddressDto, Address>]
    public partial Person Map(PersonDto value);
    
    [Mapper]
    public partial Address Map(AddressDto value);
}

/*public partial class Mapper
{
    public partial Person Map(PersonDto value)
    {
        return null; 
    }

    public partial Address Map(AddressDto value)
    {
        return null;
    }
}
*/


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

public class Company(string Name, string Address)
{
    public string SIREN { get; set; }

    public DateTime CreatedAt { get; set; }

    public Company(string name, string address, DateTime createdAt) : this(name, address)
    {
        CreatedAt = createdAt;
    }
}

public class CompanyDto(string Name, string Address)
{
    public string SIREN { get; set; }

    public DateTime CreatedAt { get; set; }

    public CompanyDto(string name, string address, DateTime createdAt) : this(name, address)
    {
        CreatedAt = createdAt;
    }
}