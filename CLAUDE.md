# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Horizon Returnables (`Hrz.Returnables`) is a .NET 10 NuGet library implementing the Result/Error pattern for .NET applications — replacing exception-driven control flow with explicit return values.

## Build Commands

A `Makefile` wraps all common operations:

```bash
make build          # Build the solution
make test           # Run all tests
make test-verbose   # Run tests with detailed output
make test-coverage  # Run tests with coverage (Coverlet/OpenCover)
make test-filter F="ClassName"  # Run filtered tests
make lint           # Check style, analyzers, and whitespace (no auto-fix)
make branch-lint    # Validate branch name
make commit-lint    # Validate commit messages (conventional commits)
make check          # Full pre-PR check (build + lint + branch + commits + tests)
make pack           # Create NuGet package in nupkg/
make clean          # Clean build artifacts
make help           # Show all targets
```

Or use dotnet directly:

```bash
dotnet build --configuration Release
dotnet test --filter "FullyQualifiedName~ClassName"
dotnet format --verify-no-changes
```

## Architecture

### `Hrz.Returnables` namespace

Lightweight `readonly struct` types using `Error` as the error carrier:

- `Result.cs` — contains both `Result` (void) and `Result<TValue>` (generic) types
- `Error.cs` — Introducing a proper Error class with Code and Reason enables meaningful error hierarchies

Features: `Code`, `Message`, `Fail`, `From`, `Try`/`TryAsync`, `Switch`, `OnSuccess`/`OnFailure`, `SuccessIf`/`FailIf`, implicit operators, `[MemberNotNullWhen]` for nullability flow analysis.

### Tests

- **`tests/Core.Tests/`** — xUnit tests organized by class and method

Zero external NuGet dependencies in the library itself.

## Key Patterns

- **`readonly struct`** Result types — stack-allocated, zero heap overhead on success path
- **Implicit operators** for ergonomic conversions
- **`[MemberNotNullWhen]`** on `Succeeded`/`Failed` for compiler nullability flow
- **Nullable reference types** enabled project-wide

## Testing

- xUnit with FluentAssertions, Arrange-Act-Assert (AAA) comment pattern
- Tests use `[Trait]` attributes for categorization
- One test class per public method (`{MethodName}UnitTests.cs`)
- Coverage via Coverlet, reported to Codecov

## CI/CD

- **CI** (`.github/workflows/ci.yml`): runs on PRs and pushes to `main` — build + test + coverage
- **Release** (`.github/workflows/release.yml`): triggered when a PR is merged to `main` — builds, tests, creates NuGet package with auto-incremented version, publishes to NuGet, creates GitHub release with tag
- **Version bumping**: determined by source branch — `release/*` → minor (or major with `bump-major-version` marker), `fix/*` → patch
- **Branching**: `main` (production), `release/*` (features/minor), `fix/*` (patches)

## Code Style

Enforced via `.editorconfig` and `.claude/rules/`. Key rules: file-scoped namespaces, explicit access modifiers, sealed classes by default, braces on all conditionals, camelCase private fields (no underscore prefix).

## Git Conventions

**Branches**: `<type>/<short-description>` in kebab-case. Types: `release`, `feat`, `fix`, `chore`, `test`, `refactor`. Only `release/*`, `feat/*` and `fix/*` merge to `main`.

**Commits**: [Conventional Commits](https://www.conventionalcommits.org/) enforced by [commitlint](https://commitlint.js.org/): `<type>(<scope>): <description>` (sentence-case, max 100 chars). Types: `feat`, `fix`, `docs`, `style`, `refactor`, `test`, `chore`, `perf`, `ci`, `build`, `revert`. Config in `commitlint.config.mjs`.

Validated locally via `make branch-lint` / `make commit-lint` (`npx commitlint`) and in CI via `wagoid/commitlint-github-action`.
