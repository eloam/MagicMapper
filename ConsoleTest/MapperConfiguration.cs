using System.Diagnostics;
using MagicMapper;

namespace ConsoleTest;

public class MapperConfiguration : IMapperConfiguration
{
    public MapperConfiguration()
    {

    }

    public void Configure()
    {
        Debugger.Break();
        Console.WriteLine("123456");
    }
}