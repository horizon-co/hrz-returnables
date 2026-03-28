<p align="center">
  <img src="icon.png" alt="Horizon Returnables" width="120" />
</p>

<h1 align="center">Horizon Returnables</h1>

<p align="center">
  A lightweight, zero-dependency Result/Error pattern library for .NET.<br/>
  Replace exception-driven control flow with explicit, type-safe return values.
</p>

<p align="center">
  <a href="https://nuget.org/packages/HorizonCo.Returnables"><img src="https://img.shields.io/nuget/v/HorizonCo.Returnables.svg?style=flat-square&logo=nuget&label=NuGet" alt="NuGet Version" /></a>
  <a href="https://nuget.org/packages/HorizonCo.Returnables"><img src="https://img.shields.io/nuget/dt/HorizonCo.Returnables.svg?style=flat-square&logo=nuget&label=Downloads" alt="NuGet Downloads" /></a>
  <a href="https://github.com/horizon-co/hrz-returnables/actions/workflows/ci.yml"><img src="https://img.shields.io/github/actions/workflow/status/horizon-co/hrz-returnables/ci.yml?branch=main&style=flat-square&logo=github&label=CI" alt="CI Status" /></a>
  <a href="https://github.com/horizon-co/hrz-returnables/blob/main/LICENSE"><img src="https://img.shields.io/github/license/horizon-co/hrz-returnables?style=flat-square&label=License" alt="License" /></a>
</p>

---

## Why Returnables?

Exceptions are expensive, invisible in method signatures, and easy to forget. The Result pattern makes success and failure **explicit** — your code tells the caller exactly what can happen, and the compiler helps enforce it.

Returnables gives you this pattern with **zero boilerplate**:

```csharp
using Horizon.Returnables;

public Result<User> GetUser(int id)
{
    var user = repository.Find(id);

    if (user is null)
    {
        return Result<User>.Fail("User not found");
    }

    return user; // implicit conversion
}
```

No wrapping. No `.Success(value)` calls. No factory noise. Just return the value or an exception — Returnables figures out which one it is.

### Key Features

- **Implicit operators** — return `TValue` or `Exception` directly from any `Result<TValue>` method
- **Zero dependencies** — pure .NET BCL, no third-party packages
- **Stack-allocated** — `readonly struct` types with zero heap overhead on success paths
- **Null-safe** — `[MemberNotNullWhen]` attributes give the compiler full nullability flow analysis
- **Try/TryAsync** — wrap any operation in a try/catch and get a `Result` back
- **Fluent side-effects** — `OnSuccess` / `OnFailure` for logging and telemetry without breaking chains
- **Conditional factories** — `SuccessIf` / `FailIf` for guard-clause style results

## Installation

```bash
dotnet add package HorizonCo.Returnables
```

Or via the Package Manager:

```
PM> Install-Package HorizonCo.Returnables
```

## Quick Start

### `Result` — void operations

For commands and side-effects that don't return a value:

```csharp
using Horizon.Returnables;

public Result DeleteUser(int id)
{
    if (!repository.Exists(id))
    {
        return Result.Fail("User not found");
    }

    repository.Delete(id);
    return Result.Success;
}
```

### `Result<TValue>` — operations with data

For operations that return a value on success:

```csharp
public Result<decimal> Divide(decimal dividend, decimal divisor)
{
    if (divisor == 0)
    {
        return Result<decimal>.Fail("Division by zero");
    }

    return dividend / divisor; // implicit conversion
}
```

### Consuming a Result

Check `Succeeded` or `Failed`, then access `Value` or `Error`:

```csharp
var result = Divide(10, 3);

if (result.Succeeded)
{
    Console.WriteLine($"Answer: {result.Value}");
}
else
{
    Console.WriteLine($"Error: {result.Error.Message}");
}
```

Or use `Switch` for exhaustive handling:

```csharp
result.Switch(
    onSuccess: value => Console.WriteLine($"Answer: {value}"),
    onFailure: error => Console.WriteLine($"Error: {error.Message}"));
```

### Implicit Conversions

Results convert implicitly to and from their inner types:

```csharp
// Value → Result (success)
Result<string> result = "Hello, world!";

// Exception → Result (failure)
Result<string> result = new InvalidOperationException("Something broke");

// Result → Value (unwrap)
string value = result;

// Result → Exception (extract error)
Exception? error = result;

// Result<T> → Result (discard value, keep outcome)
Result voidResult = result;
```

## Core API

### `Result` (void)

| Member | Description |
|--------|-------------|
| `Result.Success` | Pre-built success instance |
| `Result.Failure` | Pre-built failure with "Process failed" |
| `Result.Fail(string?)` | Failure from a message |
| `Result.Fail<TError>(TError)` | Failure from a typed exception |
| `Result.Fail()` | Failure with default message |
| `Result.From<T>(T?)` | Creates a `Result<T>` from a value |
| `Result.Try(Action)` | Wraps action in try/catch |
| `Result.TryAsync(Func<Task>)` | Wraps async action in try/catch |
| `Result.SuccessIf(bool, ...)` | Success when condition is true |
| `Result.FailIf(bool, ...)` | Failure when condition is true |
| `.Succeeded` | `true` on success |
| `.Failed` | `true` on failure |
| `.Error` | The exception, or `null` on success |
| `.Switch(onSuccess, onFailure)` | Exhaustive handling |
| `.OnSuccess(Action)` | Side-effect on success |
| `.OnFailure(Action<Exception>)` | Side-effect on failure |

### `Result<TValue>`

All members from `Result` (void) plus:

| Member | Description |
|--------|-------------|
| `Result<T>.Failure` | Pre-built failure with "Process failed" |
| `Result<T>.From(T?)` | Success from value, failure if `Exception` |
| `Result<T>.Fail(...)` | Same overloads as void Result |
| `Result<T>.Try(Func<T>)` | Wraps function in try/catch |
| `Result<T>.TryAsync(Func<Task<T>>)` | Wraps async function in try/catch |
| `Result<T>.SuccessIf(bool, T?, ...)` | Conditional success with value |
| `Result<T>.FailIf(bool, T?, ...)` | Conditional failure with value |
| `.Value` | The success value, or `default` on failure |
| `.Switch(onSuccess, onFailure)` | Exhaustive handling with value |
| `.OnSuccess(Action<T>)` | Side-effect with value on success |
| `.OnFailure(Action<Exception>)` | Side-effect on failure |

### Try / TryAsync

Wrap any operation — exceptions become failed results instead of propagating:

```csharp
// Synchronous
Result<Config> config = Result<Config>.Try(
    () => JsonSerializer.Deserialize<Config>(json)!);

// Async
Result<User> user = await Result<User>.TryAsync(
    async () => await httpClient.GetFromJsonAsync<User>(url));

// Void
Result result = Result.Try(() => fileSystem.Delete(path));
```

### SuccessIf / FailIf

Guard-clause style conditional results:

```csharp
// With a message
Result result = Result.FailIf(string.IsNullOrEmpty(name), "Name is required");

// With a typed exception
Result result = Result.SuccessIf(user.IsActive, new UnauthorizedAccessException("Account disabled"));

// With a value
Result<int> result = Result<int>.SuccessIf(age >= 18, value: age, errorMessage: "Must be 18+");
```

### OnSuccess / OnFailure

Fluent side-effects for logging, telemetry, or caching without breaking the chain:

```csharp
var result = GetUser(42)
    .OnSuccess(user => logger.LogInformation("Loaded user {Id}", user.Id))
    .OnFailure(error => logger.LogError(error, "User lookup failed"));
```

## Contributing

1. Fork the repository
2. Create your feature branch from `main` (`release/your-feature` for new features, `fix/your-fix` for patches)
3. Run the full check before submitting: `make check`
4. Open a Pull Request against `main`

### Development Commands

```bash
make build          # Build the solution
make test           # Run all tests
make lint           # Check code style
make branch-lint    # Validate branch name
make commit-lint    # Validate commit messages
make check          # Full pre-PR validation (build + lint + branch + commits + tests)
make pack           # Create NuGet package locally
```

## License

This project is licensed under the [MIT License](LICENSE).
