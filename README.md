# Event Horizon Resulting Library ![resulting](https://github.com/lucasRafaell95/hrz-operations-resulting/actions/workflows/development-ci.yml/badge.svg)

This package contains a generic, extensible implementation of a base result class.

| Package                             |  Version         | Downloads       |
| ----------------------------------- | ---------------- | --------------- |
| `Horizon.Returnables` | [![NuGet](https://img.shields.io/nuget/v/Horizon.Returnables.svg)](https://nuget.org/packages/Horizon.Returnables) | [![Nuget](https://img.shields.io/nuget/dt/Horizon.Returnables.svg)](https://nuget.org/packages/Horizon.Returnables) |


 ## Installation

 The Horizon.Returnables package is available on [Nuget](https://nuget.org/packages/Horizon.Returnables). You can use it in the following ways:

 - Package Manager
```
PM> NuGet\Install-Package Horizon.Returnables -Version 0.1.0
```

 - .NET Cli
```
dotnet add package Horizon.Returnables --version 0.1.0
```

 - PackageReference
```
<PackageReference Include="Horizon.Returnables" Version="0.1.0" />
```

 ## How to use

 - Result

 Result was developed to encompass the result of operations in a generic and extensible way. With it it is possible to return from these literal types, such as an int for example, to complex objects.
 ```C#
 public Result<string> GetFullName(string firstName, string lastName)
 {
    var fullName = firstName + lastName;

    return fullName;
 }
 ```

 - Error

 It is also possible to specify an error object to signal/return the status of an operation. Essentially, an error has two properties: code and a message. To use it, just do as in the example below:
 ```C#
 public Result<float> DivideNumber(float dividend, float divisor)
 {
    if(divisor = 0)
    {
        var error = new Error("400-1", "You can never divide by zero");

        return error;
    }

    var result = dividend / divisor;

    return new Result<float>(result);
 }
 ```