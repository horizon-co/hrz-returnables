# Architecture Rules — hrz-returnables

## Project Type

This is a **NuGet library package** (`HorizonCo.Returnables`), not a web application. There are no controllers, middleware, DI, or infrastructure layers. The library provides Result/Error types consumed by other Horizon projects.

## Project Structure

```
src/Core/          → library source
tests/Core.Tests/  → unit tests
```

## Result Types — `Hrz.Returnables` namespace

Lightweight `readonly struct` types using `Exception` as the error carrier:

| File | Type | Purpose |
|---|---|---|
| `Result.cs` | `Result` | Void result for commands/side-effects |
| `Result.cs` | `Result<TValue>` | Generic result for operations returning data |

Both types live in the same file (`Result.cs`).

Features: `Fail`, `From`, `Try`/`TryAsync`, `Switch`, `OnSuccess`/`OnFailure`, `SuccessIf`/`FailIf`, implicit operators, `[MemberNotNullWhen]`.

## `.csproj` Standard Properties

Production project:

```xml
<PropertyGroup>
  <TargetFramework>net10.0</TargetFramework>
  <Nullable>enable</Nullable>
  <WarningsAsErrors>Nullable</WarningsAsErrors>
  <ImplicitUsings>enable</ImplicitUsings>
  <GenerateDocumentationFile>true</GenerateDocumentationFile>
  <RootNamespace>Hrz.Returnables.Core</RootNamespace>
  <AssemblyName>$(RootNamespace)</AssemblyName>
</PropertyGroup>
```

Test project:

```xml
<PropertyGroup>
  <TargetFramework>net10.0</TargetFramework>
  <Nullable>enable</Nullable>
  <WarningsAsErrors>Nullable</WarningsAsErrors>
  <ImplicitUsings>enable</ImplicitUsings>
  <RootNamespace>Hrz.Returnables.Core.Tests</RootNamespace>
</PropertyGroup>
```

## Dependencies

The library has **zero external NuGet dependencies** — it relies only on the .NET BCL. Keep it that way.

## Design Rules

- Result types (`Result`, `Result<TValue>`) are `readonly struct` — stack-allocated, no heap on success path
- Use `[MemberNotNullWhen]` on `Succeeded`/`Failed` for compiler nullability flow analysis
- All constructors are `private` — creation goes through `From`, `Fail`, or implicit operators
- Implicit operators enable ergonomic conversions between Result, TValue, and Exception

## Naming

To understand the naming conventions, see [C# Naming Conventions](csharp-naming-conventions.md).

## C# Style

To understand the project code style, see [C# Code Style](csharp-code-style.md).

## Testing

To understand the testing strategy and style, see [Testing](dotnet-testing-conventions.md).
