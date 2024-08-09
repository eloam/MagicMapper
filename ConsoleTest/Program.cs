using ConsoleTest;
using ConsoleTest.Syntax;
using MagicMapper.Generator.Syntax;
using Microsoft.CodeAnalysis.CSharp;


var toto = new Toto() { Name = "Name", Age = 10 };
var toto1 = new Toto() { Name = "Name", Age = 10 };


HashSet<Toto> totos = [];
totos.Add(toto);
totos.Add(toto1);

Console.WriteLine(toto.GetHashCode() == toto1.GetHashCode());


var company = new Company(Name: "Name", Address: "Address")
{
    SIREN = "123456789"
};


Console.ReadKey();


public class Toto
{
    public required string Name { get; init; }

    public required int Age { get; init; }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Age);
    }
}