---
paths:
  - ".github/**"
  - "CLAUDE.md"
---

# Git Conventions

## Branch Naming

Branches **must** follow the pattern `<type>/<short-description>` using kebab-case.

| Prefix | Purpose | Version bump |
|---|---|---|
| `release/*` | New features, enhancements, breaking changes | minor (or major with `bump-major-version` marker) |
| `fix/*` | Bug fixes, patches | patch |
| `chore/*` | Tooling, CI, dependencies, docs — no release | none |
| `test/*` | Adding or updating tests — no release | none |
| `refactor/*` | Code restructuring — no release | none |

Only `release/*` and `fix/*` branches can be merged to `main` (enforced by the release workflow).

### Examples

```
release/add-bind-method
release/v2.0.0
fix/null-value-in-from
chore/update-ci-workflow
test/add-result-unit-tests
refactor/extract-validation-logic
```

### Rules

- Use **kebab-case** for the description (lowercase, hyphens)
- Keep descriptions short and descriptive (3-5 words)
- Never push directly to `main`

---

# Conventional Commits

All commit messages **must** follow the [Conventional Commits](https://www.conventionalcommits.org/) specification.

## Format

```
<type>(<scope>) - <description>

[optional body]

[optional footer(s)]
```

- Use a **dash** (`-`) as separator instead of colon (`:`)
- Scope is **required** in parentheses
- Description starts with lowercase

## Types

| Type | When to use |
|---|---|
| `feat` | A new feature or capability |
| `fix` | A bug fix |
| `docs` | Documentation-only changes |
| `style` | Formatting, whitespace, semicolons — no logic change |
| `refactor` | Code restructuring without adding features or fixing bugs |
| `test` | Adding or updating tests |
| `chore` | Build process, tooling, dependencies, CI/CD changes |
| `perf` | Performance improvements |
| `ci` | CI/CD configuration changes |

## Scope

Scope describes **what** is affected. Common scopes for this project:

| Scope | When to use |
|---|---|
| `result` | Changes to `Result` (void) type |
| `result-t` | Changes to `Result<TValue>` type |
| `nuget` | NuGet package metadata, versioning |
| `ci` | GitHub Actions workflows |
| `makefile` | Makefile targets |
| `docs` | README, CLAUDE.md, rules |
| `tests` | Test project, test files |

## Examples

```
feat(result) - add Bind method for railway chaining
fix(result-t) - handle null value in From factory
test(result) - add unit tests for SuccessIf overloads
chore(nuget) - update package description
docs(readme) - remove legacy API references
ci(workflow) - add lint step to CI pipeline
refactor(result-t) - extract common validation to private method
```

## Breaking Changes

Append `!` after the type/scope to indicate a breaking change:

```
feat(result)! - replace Exception with custom Error type
```

Or include `BREAKING CHANGE:` in the footer:

```
refactor(result) - change Fail signature

BREAKING CHANGE: Fail no longer accepts null messages
```
