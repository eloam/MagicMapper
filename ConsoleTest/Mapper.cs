using ConsoleTest.Folder1;
using ConsoleTest.Folder2;
using ConsoleTest.Folder3Dto;
using ConsoleTest.Folder4Dto;
using MagicMapper.Generator;
using MagicMapper.Generator.Attributes;

namespace ConsoleTest;


[Mapper]
public partial class Mapper
{
    public partial Address Map(AddressDto value);
}



/*
[Mapper]
public partial class Mapper2
{
    public Person Map(PersonDto value)
    {
        return new Person
        {
            FirstName = value.FirstName,
            LastName = value.LastName,
            Age = value.Age,
            Address = new Address
            {
                Street = value.Address.Street,
                City = value.Address.City,
                ZipCode = value.Address.ZipCode
            }
        };
    }
}*/


public class CompanyDto(string Name, string Address)
{
    public string SIREN { get; set; }

    public DateTime CreatedAt { get; set; }

    public CompanyDto(string name, string address, DateTime createdAt) : this(name, address)
    {
        CreatedAt = createdAt;
    }
}