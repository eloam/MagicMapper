using ConsoleTest.Folder4Dto;

namespace ConsoleTest.Folder3Dto;

public class PersonDto
{
    public string FirstName { get; set; }
    public required string LastName { get; set; }
    public int Age { get; set; }
    public AddressDto Address { get; set; }
}
