# ============================================================================
#  RETURNABLES — Makefile
# ============================================================================

SHELL := /bin/bash
.DEFAULT_GOAL := help

# ----------------------------------------------------------------------------
#  Theme detection
# ----------------------------------------------------------------------------
# Override with: make <target> THEME=light  or  THEME=dark
# Auto-detects via COLORFGBG (format "fg;bg" — bg >= 8 means light background)
ifndef THEME
  THEME := $(shell echo light; )
endif

# ----------------------------------------------------------------------------
#  Colors & Symbols
# ----------------------------------------------------------------------------
R  := \033[0m
B  := \033[1m
DM := \033[2m

ifeq ($(THEME),light)
  RD := \033[38;5;160m
  GR := \033[38;5;28m
  YL := \033[38;5;130m
  BL := \033[38;5;25m
  MG := \033[38;5;127m
  CY := \033[38;5;30m
  OR := \033[38;5;166m
  WH := \033[38;5;16m
  GY := \033[38;5;240m
else
  RD := \033[38;5;196m
  GR := \033[38;5;82m
  YL := \033[38;5;220m
  BL := \033[38;5;39m
  MG := \033[38;5;213m
  CY := \033[38;5;51m
  OR := \033[38;5;208m
  WH := \033[38;5;255m
  GY := \033[38;5;245m
endif

OK := $(GR)✔$(R)
KO := $(RD)✖$(R)
AR := $(CY)→$(R)
WN := $(YL)⚠$(R)

# ----------------------------------------------------------------------------
#  Project
# ----------------------------------------------------------------------------
SLN        := Hrz.Returnables.slnx
CORE_PROJ  := src/Core/Core.csproj
CORE_TEST  := tests/Core.Tests/Core.Tests.csproj
NUPKG_DIR  := nupkg

# ----------------------------------------------------------------------------
#  Helpers
# ----------------------------------------------------------------------------
define header
	@printf "\n$(B)$(BL)━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━$(R)\n"
	@printf "  $(B)$(WH)$(1)$(R)\n"
	@printf "$(B)$(BL)━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━$(R)\n\n"
endef

define step
	@printf "  $(AR) $(WH)$(1)$(R)\n"
endef

define success
	@printf "\n  $(OK) $(GR)$(1)$(R)\n\n"
endef

define fail
	@printf "\n  $(KO) $(RD)$(1)$(R)\n\n"
endef

define warn
	@printf "\n  $(WN) $(YL)$(1)$(R)\n\n"
endef

# ============================================================================
#  TARGETS
# ============================================================================

## ── Build ───────────────────────────────────────────────────

.PHONY: build
build: ## 🔨 Build the entire solution
	$(call header,Building solution)
	@dotnet build $(SLN) --verbosity quiet 2>&1 | tail -5
	$(call success,Build complete)

.PHONY: clean
clean: ## 🧹 Clean build artifacts
	$(call header,Cleaning build artifacts)
	$(call step,Cleaning solution...)
	@dotnet clean $(SLN) --verbosity quiet 2>/dev/null || true
	$(call step,Removing bin/ and obj/ directories...)
	@find . -type d \( -name bin -o -name obj \) -not -path "./.git/*" -exec rm -rf {} + 2>/dev/null || true
	$(call step,Removing nupkg/ directory...)
	@rm -rf $(NUPKG_DIR)
	$(call success,Clean complete)

.PHONY: restore
restore: ## 📦 Restore NuGet packages
	$(call header,Restoring packages)
	@dotnet restore $(SLN) --verbosity quiet
	$(call success,Restore complete)

## ── Testing ─────────────────────────────────────────────────

.PHONY: test
test: ## 🧪 Run all tests
	$(call header,Running all tests)
	@dotnet test $(SLN) --verbosity quiet --nologo 2>&1 \
		| grep -v "TestAdapterPath" \
		|| true
	$(call success,Tests complete)

.PHONY: test-verbose
test-verbose: ## 🧪 Run all tests with detailed output
	$(call header,Running all tests — verbose)
	@dotnet test $(SLN) --verbosity normal --nologo

.PHONY: test-coverage
test-coverage: ## 🧪 Run tests with coverage report
	$(call header,Running tests with coverage)
	@dotnet test $(SLN) --no-build --verbosity normal --configuration Release \
		/p:CollectCoverage=true /p:CoverletOutputFormat=opencover
	$(call success,Coverage report generated)

.PHONY: test-filter
test-filter: ## 🧪 Run filtered tests (usage: make test-filter F="ClassName")
	$(call header,Running filtered tests — $(CY)$(F)$(R))
	@dotnet test $(SLN) --filter "FullyQualifiedName~$(F)" --verbosity quiet --nologo
	$(call success,Filtered tests complete)

## ── Quality ─────────────────────────────────────────────────

.PHONY: lint
lint: ## 🎨 Lint — analyze code without modifying anything
	$(call header,Linting code)
	$(call step,Checking style rules...)
	@dotnet format style $(SLN) --verify-no-changes --verbosity quiet 2>&1 \
		&& printf "  $(OK) $(GR)Style rules passed$(R)\n" \
		|| { printf "  $(KO) $(RD)Style issues found$(R)\n"; LINT_FAIL=1; }
	$(call step,Checking analyzers...)
	@dotnet format analyzers $(SLN) --verify-no-changes --verbosity quiet 2>&1 \
		&& printf "  $(OK) $(GR)Analyzer rules passed$(R)\n" \
		|| { printf "  $(KO) $(RD)Analyzer issues found$(R)\n"; LINT_FAIL=1; }
	$(call step,Checking whitespace and formatting...)
	@dotnet format whitespace $(SLN) --verify-no-changes --verbosity quiet 2>&1 \
		&& printf "  $(OK) $(GR)Whitespace rules passed$(R)\n" \
		|| { printf "  $(KO) $(RD)Whitespace issues found$(R)\n"; LINT_FAIL=1; }
	@printf "\n"

.PHONY: branch-lint
branch-lint: ## 🔍 Validate branch name (override: make branch-lint BRANCH=fix/foo)
	$(call header,Validating branch name)
	@BRANCH=$${BRANCH:-$$(git rev-parse --abbrev-ref HEAD)}; \
	PATTERN='^(release|feat|fix|chore|test|refactor)/[a-z0-9-]+$$'; \
	if [ "$$BRANCH" = "main" ]; then \
		printf "  $(OK) main (protected branch)\n"; \
	elif echo "$$BRANCH" | grep -qE "$$PATTERN"; then \
		printf "  $(OK) $$BRANCH\n"; \
	else \
		printf "  $(KO) $$BRANCH\n"; \
		printf "\n  $(WN) $(YL)Expected: <type>/<short-description> (kebab-case)$(R)\n"; \
		printf "  $(GY)Types: release fix chore test refactor$(R)\n\n"; \
		exit 1; \
	fi
	$(call success,Branch name is valid)

.PHONY: commit-lint
commit-lint: ## 🔍 Validate commit messages (conventional commits)
	$(call header,Validating commit messages)
	@BASE=$$(git merge-base HEAD main 2>/dev/null || echo HEAD~1); \
	npx commitlint --from $$BASE --to HEAD --verbose || ( \
		echo "\n❌ Alguns commits não seguem o padrão Conventional Commits" && \
		echo "Tipos permitidos: feat, fix, docs, style, refactor, test, chore, perf, ci, build, revert" && \
		exit 1 \
	)
	$(call success,All commit messages are valid ✅)

.PHONY: check
check: ## ✅ Full pre-PR check (build + lint + branch + commits + tests)
	$(call header,Running full PR checklist)
	$(call step,[1/5] Building solution...)
	@dotnet build $(SLN) --verbosity quiet 2>&1 | grep -E "Error|succeeded" | tail -1
	$(call step,[2/5] Linting...)
	@dotnet format $(SLN) --verify-no-changes --verbosity quiet 2>&1 \
		&& printf "    $(OK) $(GR)Lint passed$(R)\n" \
		|| printf "    $(KO) $(RD)Lint issues found$(R)\n"
	$(call step,[3/5] Validating branch name...)
	@BRANCH=$${BRANCH:-$$(git rev-parse --abbrev-ref HEAD)}; \
	PATTERN='^(release|feat|fix|chore|test|refactor)/[a-z0-9-]+$$'; \
	if [ "$$BRANCH" = "main" ]; then \
		printf "    $(OK) $(GR)main$(R)\n"; \
	elif echo "$$BRANCH" | grep -qE "$$PATTERN"; then \
		printf "    $(OK) $(GR)$$BRANCH$(R)\n"; \
	else \
		printf "    $(KO) $(RD)$$BRANCH — expected <type>/<description>$(R)\n"; \
	fi
	$(call step,[4/5] Validating commit messages...)
	@BASE=$$(git merge-base HEAD main 2>/dev/null || echo HEAD~1); \
	npx commitlint --from $$BASE --to HEAD 2>&1 \
		&& printf "    $(OK) $(GR)Commit messages valid$(R)\n" \
		|| printf "    $(KO) $(RD)Invalid commit messages found$(R)\n"
	$(call step,[5/5] Running tests...)
	@dotnet test $(SLN) --verbosity quiet --nologo 2>&1 \
		| grep -E "Passed|Failed|Total" | tail -1 || echo "  No tests found"
	$(call success,PR checklist complete)

## ── Package ─────────────────────────────────────────────────

.PHONY: pack
pack: ## 📦 Create NuGet package
	$(call header,Creating NuGet package)
	@dotnet pack $(CORE_PROJ) --configuration Release -o $(NUPKG_DIR)
	$(call success,Package created in $(NUPKG_DIR)/)

## ── Help ────────────────────────────────────────────────────

.PHONY: help
help: ## 📖 Show this help
	@printf "\n"
	@printf "  $(B)$(MG)██████$(BL) ███████$(CY) ████████$(GR) ██    ██$(YL) ██████$(OR) ███    ██$(R)\n"
	@printf "  $(B)$(MG)██   █$(BL) ██     $(CY)    ██   $(GR) ██    ██$(YL) ██   █$(OR) ████   ██$(R)\n"
	@printf "  $(B)$(MG)██████$(BL) █████  $(CY)    ██   $(GR) ██    ██$(YL) ██████$(OR) ██ ██  ██$(R)\n"
	@printf "  $(B)$(MG)██  █ $(BL) ██     $(CY)    ██   $(GR) ██    ██$(YL) ██  █ $(OR) ██  ██ ██$(R)\n"
	@printf "  $(B)$(MG)██   █$(BL) ███████$(CY)    ██   $(GR)  ██████$(YL) ██   █$(OR) ██   ████$(R)\n"
	@printf "\n"
	@printf "  $(DM).NET · NuGet Library · Result/Error Pattern$(R)\n"
	@printf "\n"
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) \
		| awk 'BEGIN {FS = ":.*?## "}; { \
			split($$2, parts, " "); \
			emoji = parts[1]; \
			desc = substr($$2, length(emoji)+2); \
			printf "  $(WH)%-18s$(R) %s $(DM)%s$(R)\n", $$1, emoji, desc \
		}'
	@printf "\n"
