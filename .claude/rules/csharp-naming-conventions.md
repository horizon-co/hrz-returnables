---
paths:
  - "src/**/*.cs"
  - "tests/**/*.cs"
---

# C# Naming Conventions

Rules for naming all C# constructs. Apply consistently across the entire codebase.

## Classes and Records

- PascalCase always.
- Never abbreviate class names. Prefer clarity over brevity.

## Interfaces

- PascalCase with `I` prefix.
- Name should describe the capability, not the implementation.

## Methods

- PascalCase always.
- Use verb or verb-noun pairs: `Create`, `ToString`.
- All async methods must end with `Async`.

## Private Fields

- camelCase without leading underscore.
- Always `readonly` when set only in the constructor.

```csharp
private readonly string code;
private readonly ErrorType type;
```

## Properties

- PascalCase always.
- Use `init` for immutable properties.
- Boolean properties start with `Is`, `Has`, `Can`, or `Should`.

```csharp
public bool Success { get; }
public TResult? Data { get; }
```

## Constants and Static Readonly Fields

- PascalCase (not SCREAMING_SNAKE_CASE).
- `const` for compile-time primitives; `static readonly` for complex types.

## Enums

- PascalCase for the type and all members.
- Assign explicit numeric values to all members.

```csharp
public enum ErrorType
{
    BusinessError = 1,
    ServerError = 2
}
```

## Records and DTOs

- Use `sealed record` for specialized subtypes.
- Core base types (Result, Error) use `record` without `sealed` when inheritance is needed.

## Parameters

- camelCase always.
- Avoid single-letter names except for well-known short-lived variables (`i`, `j` in loops).

## Local Variables

- camelCase.
- Prefer descriptive names. Only use `var` when the type is obvious from the right-hand side.

## Namespaces

- PascalCase for each segment.
- Mirror the project's folder structure exactly.
- Format: `Horizon.Returnables`.

```csharp
namespace Horizon.Returnables;
```

## Test Naming

See `dotnet-testing-conventions.md` for test-specific naming rules.
