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
| `feat/*` | New features (alternative to release) | minor |
| `fix/*` | Bug fixes, patches | patch |
| `chore/*` | Tooling, CI, dependencies, docs — no release | none |
| `test/*` | Adding or updating tests — no release | none |
| `refactor/*` | Code restructuring — no release | none |

Only `release/*`, `feat/*` and `fix/*` branches can be merged to `main` (enforced by the release workflow).

### Examples

```
release/add-bind-method
feat/new-result-pattern
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

All commit messages **must** follow the [Conventional Commits](https://www.conventionalcommits.org/) specification, enforced by [commitlint](https://commitlint.js.org/).

## Format

```
<type>(<scope>): <description>

[optional body]

[optional footer(s)]
```

- Use a **colon** (`:`) as separator (standard conventional commits)
- Scope is **optional** in parentheses
- Description starts with uppercase (sentence-case, enforced by commitlint)
- Max header length: 100 characters

## Types

| Type | When to use |
|---|---|
| `feat` | A new feature or capability |
| `fix` | A bug fix |
| `docs` | Documentation-only changes |
| `style` | Formatting, whitespace, semicolons — no logic change |
| `refactor` | Code restructuring without adding features or fixing bugs |
| `test` | Adding or updating tests |
| `chore` | Build process, tooling, dependencies |
| `perf` | Performance improvements |
| `ci` | CI/CD configuration changes |
| `build` | Build system or external dependency changes |
| `revert` | Reverting a previous commit |

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
| `project` | Project-wide changes |

## Examples

```
feat(result): Add Bind method for railway chaining
fix(result-t): Handle null value in From factory
test(result): Add unit tests for SuccessIf overloads
chore(nuget): Update package description
docs(readme): Remove legacy API references
ci(workflow): Add lint step to CI pipeline
refactor(result-t): Extract common validation to private method
```

## Breaking Changes

Append `!` after the type/scope to indicate a breaking change:

```
feat(result)!: Replace Exception with custom Error type
```

Or include `BREAKING CHANGE:` in the footer:

```
refactor(result): Change Fail signature

BREAKING CHANGE: Fail no longer accepts null messages
```

## Setup

commitlint requires Node.js. Install dependencies:

```bash
npx @commitlint/cli --version   # runs without global install
```

Configuration is in `commitlint.config.mjs` at the project root, extending `@commitlint/config-conventional`.
