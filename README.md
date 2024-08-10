# MagicMapper

MagicMapper is a source generator for C# that automatically generates mapping code between different types. It uses attributes to mark classes and methods that
require mapping, and generates the necessary code to map properties between source and destination types.

**Features**:
- Automatically generates mapping code for classes and methods.
- Supports nested object mapping.
- Ensures type safety and correct property assignments.

**Installation**:

To install MagicMapper, add the following package reference to your project file:

```xml
<ItemGroup>
    <PackageReference Include="MagicMapper.Generator" Version="1.0.0" />
</ItemGroup>
```

**Usage**:

1. Add the Mapper attribute to your class or method:

```csharp

using MagicMapper.Generator.Attributes;

[Mapper]
public partial class Mapper
{
    public partial Person Map(PersonDto value);
}

```

2. Build your project to generate the mapping code.
3. Use the generated code to map between types:
   - When you build your project, MagicMapper will generate the necessary mapping code. The generated code will be placed in a file with the .g.cs extension.

**Contributing**:
Contributions are welcome! Please open an issue or submit a pull request on GitHub.  

**License**:
This project is licensed under the Apache License.