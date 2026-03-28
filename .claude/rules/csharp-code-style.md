---
paths:
  - "src/**/*.cs"
  - "tests/**/*.cs"
---

# C# Code Style Rules

Rules for writing C# code in this project. Apply these when generating, reviewing, or modifying any C# source file.

## Namespaces

- Always use **file-scoped namespaces** (C# 10+). Never wrap code in `namespace Foo { }` blocks.
- Namespace must mirror the folder structure relative to the project root.

```csharp
// correct
namespace Horizon.Returnables.Core.Errors;

public record Error { }
```

## File Organization

- Keep exactly one top-level type declaration per file.
- This rule applies to `class`, `interface`, `enum`, `record`, and `struct`.

## Nullable Reference Types

- `<Nullable>enable</Nullable>` must be set in every `.csproj`.
- Mark non-nullable reference properties that are injected/initialized later with `= null!`.
- Mark truly optional reference properties with `?`.

## Implicit Usings

- Enable `<ImplicitUsings>enable</ImplicitUsings>` in every `.csproj`.

## Access Modifiers

- Always specify access modifiers explicitly — never rely on defaults.
- Use `sealed` for classes that should not be subclassed.
- This is a library — types consumed externally must be `public`. Internal helpers use `internal sealed`.

## Documentation & XML Comments

- Every public method and constructor must include XML documentation with `<summary>`.
- Document parameters with `<param>` and expected exceptions with `<exception>`.
- Enums must include brief XML summaries on the enum type and every member.

```csharp
/// <summary>
/// Initialize the error with code and message description
/// </summary>
/// <exception cref="ArgumentNullException"></exception>
public Error(string code, string message, ErrorType type = ErrorType.BusinessError)
```

## Immutability

- Use `record` for core types (Result, Error) — they are immutable by design.
- Use `sealed record` for specialized subtypes (CommonError, HttpResult).
- Use `init` accessors for properties that must be set at construction time.

## Classes and Sealing

- Seal all concrete implementation classes unless inheritance is explicitly required.
- `Result<TResult>` is intentionally **not** sealed — `HttpResult<TResult>` extends it.
- `HttpResult<TResult>` and `CommonError` are sealed.

## Expression Bodies

- Use expression bodies for simple single-expression properties and methods.
- Method expression bodies must always break line before `=>`.
- Property expression bodies can stay inline when simple.

```csharp
// correct for methods
public static Result<TResult> Create(TResult? result)
    => new(result);

// correct for properties
public bool IsEmpty => Data == null;
```

## Control Flow

- Always use braces for conditionals, including single-line `if`, `else if`, and `else` blocks.

## Null Checks in Constructors

- Validate constructor parameters with `?? throw new ArgumentNullException(nameof(param))` for inline assignment.
- For string parameters, use `string.IsNullOrWhiteSpace(value) ? throw new ArgumentNullException(nameof(value)) : value`.

```csharp
Code = string.IsNullOrWhiteSpace(code) ? throw new ArgumentNullException(nameof(code)) : code;
```

## Static Factory Methods

- Expose `Create(...)` static factory methods alongside constructors for ergonomic API usage.
- Expose pre-built static instances for common states (e.g., `HttpResult.OK(data)`).

## Implicit Operators

- Use implicit conversion operators to reduce boilerplate when converting between closely related types.

```csharp
public static implicit operator Result<TResult>(TResult result)
    => new(result);

public static implicit operator Result<TResult>(Error error)
    => new(error);
```

## var Usage

- Use `var` when the type is immediately obvious from the right-hand side.
- Use explicit types when the assigned type is not immediately clear.
