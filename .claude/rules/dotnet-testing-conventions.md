---
paths:
  - "tests/**/*.cs"
---

# .NET Testing Conventions

Rules for writing tests in this project. Apply these when creating or modifying any test file.

## Frameworks and Libraries

| Purpose | Library |
|---|---|
| Test framework | xUnit |
| Assertions | FluentAssertions |
| Coverage | Coverlet |

Never mix assertion styles — always use FluentAssertions. Never use `Assert.Equal`, `Assert.True`, etc.

This is a pure library with no external dependencies — mocking frameworks (Moq) are not needed.

## Project Organization

Mirror the `src/Core/` structure under `tests/Core.Tests/`:

```
src/Core/
├── Result.cs
└── Result.T.cs

tests/Core.Tests/
├── ResultUnitTests/
│   ├── FailUnitTests.cs
│   ├── FromUnitTests.cs
│   ├── TryUnitTests.cs
│   ├── TryAsyncUnitTests.cs
│   ├── SwitchUnitTests.cs
│   ├── OnSuccessUnitTests.cs
│   ├── OnFailureUnitTests.cs
│   ├── SuccessIfUnitTests.cs
│   ├── FailIfUnitTests.cs
│   └── ImplicitOperatorUnitTests.cs
└── ResultTUnitTests/
    ├── FailUnitTests.cs
    ├── FromUnitTests.cs
    ├── TryUnitTests.cs
    ├── TryAsyncUnitTests.cs
    ├── SwitchUnitTests.cs
    ├── OnSuccessUnitTests.cs
    ├── OnFailureUnitTests.cs
    ├── SuccessIfUnitTests.cs
    ├── FailIfUnitTests.cs
    ├── SucceededUnitTests.cs
    └── ImplicitOperatorUnitTests.cs
```

Each component gets its own `{Component}UnitTests/` folder. **Each public method or constructor of the class under test must have its own dedicated test class** — if a class has 5 public methods, there will be 5 corresponding test classes.

## File and Class Naming

- **Test class**: `{MethodName}UnitTests` — one class per method under test.
- Classes must be `sealed`.
- Namespaces mirror the folder structure with `.Tests` segment.

```csharp
// File: tests/Core.Tests/ResultUnitTests/FailUnitTests.cs
namespace Horizon.Returnables.Core.Tests.ResultUnitTests;

[Trait(nameof(Result), nameof(Result.Fail))]
public sealed class FailUnitTests { }
```

## Test Method Naming

Use `When_{Condition}_Should_{Expected}` pattern:

```csharp
When_A_Message_Is_Provided_Should_Create_Failed_Result()
When_Typed_Exception_Is_Null_Should_Throw_ArgumentNullException()
When_Result_Is_Success_Should_Execute_Action()
```

Method names must be fully descriptive — avoid abbreviations. Use underscores only as word separators.

## Test Structure — Arrange/Act/Assert

Always structure test methods with explicit `// arrange`, `// act`, `// assert` comments. When arrange and act are combined, use `// arrange | act`:

```csharp
[Fact]
public void When_A_Message_Is_Provided_Should_Create_Failed_Result()
{
    // arrange
    var message = "Something went wrong";

    // act
    var result = Result.Fail(message);

    // assert
    result.Failed.Should().BeTrue();
    result.Error.Should().NotBeNull();
    result.Error!.Message.Should().Be(message);
}
```

## Assertions — FluentAssertions

Use the fluent API. Never use raw xUnit assertions.

```csharp
// Exception assertions
FluentActions
    .Invoking(() => Result.Fail<InvalidOperationException>(null))
    .Should()
    .ThrowExactly<ArgumentNullException>();

// Also valid for inline lambdas
Action act = () => Result<int>.Try(null!);
act.Should()
    .ThrowExactly<ArgumentNullException>();

// Value assertions
result.Succeeded.Should().BeTrue();
result.Value.Should().Be(42);
result.Error.Should().BeNull();
```

## Parameterized Tests

Use `[Theory]` + `[InlineData]` for simple value variations:

```csharp
[Theory]
[InlineData(null)]
[InlineData("")]
[InlineData("   ")]
public void When_Message_Is_Null_Or_Whitespace_Should_Return_Default_Message(string? message) { }
```

## Traits

Apply `[Trait]` to every test class with the component class name as the key:

```csharp
[Trait(nameof(Result), nameof(Result.Fail))]
public sealed class FailUnitTests { }

[Trait(nameof(Result), nameof(Result.Try))]
public sealed class TryUnitTests { }

[Trait("Result<TValue>", nameof(Result<int>.OnSuccess))]
public sealed class OnSuccessUnitTests { }
```
