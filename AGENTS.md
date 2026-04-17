# AGENTS.md

Guidance for autonomous coding agents working in this repository.

## Repository Overview

AutoFilterer is a .NET library that auto-generates LINQ expressions for entity filtering via DTOs.
Solution: `AutoFilterer.sln`. License: MIT.

### Project Map

| Project | Path | Targets |
|---|---|---|
| AutoFilterer (core) | `src/AutoFilterer` | `netstandard2.0;netstandard2.1;net9.0` |
| AutoFilterer.Generators | `src/AutoFilterer.Generators` | `netstandard2.0` (Roslyn source generator) |
| AutoFilterer.Dynamics | `src/AutoFilterer.Dynamics` | — |
| AutoFilterer.Swagger | `src/AutoFilterer.Swagger` | — |
| AutoFilterer.Tests | `tests/AutoFilterer.Tests` | `net9.0` |
| AutoFilterer.Tests.Core | `tests/AutoFilterer.Tests.Core` | `net9.0` (shared test infra) |
| AutoFilterer.Generators.Tests | `tests/AutoFilterer.Generators.Tests` | `net10.0` |
| AutoFilterer.Dynamics.Tests | `tests/AutoFilterer.Dynamics.Tests` | `net9.0` |
| AutoFilterer.Benchmark | `tests/AutoFilterer.Benchmark` | `net9.0` (BenchmarkDotNet) |
| Sandbox API | `sandbox/WebApplication.API` | ASP.NET Core app |

### Core Architecture (src/AutoFilterer)

- **Abstractions/**: Interfaces — `IFilter`, `IFilterableType`, `IRange`, `IOrderable`, `IPaginationFilter`.
- **Types/**: Base filter classes — `FilterBase`, `OrderableFilterBase`, `PaginationFilterBase`, `Range<T>`, `StringFilter`, `OperatorFilter<T>`.
- **Attributes/**: Expression-building attributes — `CompareToAttribute`, `OperatorComparisonAttribute`, `StringFilterOptionsAttribute`, `IgnoreFilterAttribute`, `CollectionFilterAttribute`, etc.
- **Enums/**: `CombineType`, `OperatorType`, `StringFilterOption`, `Sorting`, `CollectionFilterType`.
- **Extensions/**: `ExpressionExtensions` (combine expressions), `QueryExtensions` (`.ApplyFilter()`, `.ToPaged()`).

Expression flow: `FilterBase.BuildExpression()` iterates DTO properties → resolves `CompareToAttribute` → delegates to `FilteringOptionsBaseAttribute.BuildExpression()` → combines via `CombineType`.

### Shared Build Config

`common.props` applies to all projects:
- `LangVersion`: `latest`
- Defines `LEGACY_NAMESPACE` constant (used in `#if` blocks for backward-compatible using directives).
- Version: managed centrally.

## Cursor/Copilot Rules

No `.cursorrules`, `.cursor/rules/`, or `.github/copilot-instructions.md` files exist.
If added later, treat them as higher-priority than this document.

## Build, Test, Pack Commands

CI uses .NET SDK `10.0.201`. Run all commands from repo root.

```sh
# Restore
dotnet restore

# Build (CI-equivalent)
dotnet build --configuration Release --no-restore

# Test all
dotnet test --no-restore --verbosity normal

# Test one project
dotnet test tests/AutoFilterer.Tests/AutoFilterer.Tests.csproj --no-restore

# Test single test (fully qualified name — recommended)
dotnet test tests/AutoFilterer.Tests/AutoFilterer.Tests.csproj --filter "FullyQualifiedName=AutoFilterer.Tests.Types.FilterBaseTests.ApplyFilterTo_WithEmptyFilterBase_ShouldMatchExpected"

# Test by class name
dotnet test tests/AutoFilterer.Tests/AutoFilterer.Tests.csproj --filter "FullyQualifiedName~FilterBaseTests"

# Test by display name substring
dotnet test tests/AutoFilterer.Tests/AutoFilterer.Tests.csproj --filter "DisplayName~BuildExpression"

# Discover test names
dotnet test tests/AutoFilterer.Tests/AutoFilterer.Tests.csproj --list-tests

# Pack (do NOT push unless explicitly asked)
dotnet pack -o packages --configuration Release
```

### Validation Matrix

- Core filtering: `dotnet test tests/AutoFilterer.Tests/AutoFilterer.Tests.csproj --no-restore`
- Source generators: `dotnet test tests/AutoFilterer.Generators.Tests/AutoFilterer.Generators.Tests.csproj --no-restore` (requires net10 SDK)
- Dynamics/web binding: `dotnet test tests/AutoFilterer.Dynamics.Tests/AutoFilterer.Dynamics.Tests.csproj --no-restore`
- Broad changes: restore → build Release → test all (see commands above).

## Lint / Formatting

No dedicated lint or `dotnet format` step in CI. No `.editorconfig`.
Quality gate: restore + Release build + tests pass. Keep style consistent with surrounding files.

## Code Style Conventions

### Namespaces

- File-scoped namespaces (`namespace X.Y;`).
- One public type per file unless helper types are tightly coupled.

### Using Directives

- Top of file. Preserve existing order in edited files.
- Preserve `#if LEGACY_NAMESPACE` / `using AutoFilterer.Enums;` / `#endif` blocks where present.

### Naming

- Types, methods, properties: `PascalCase`.
- Private fields: match file-local convention (`_camelCase` or `camelCase` both exist).
- Test methods: `Action_WithCondition_ShouldResult` (e.g., `ApplyFilterTo_WithEmptyFilterBase_ShouldMatchExpected`).

### Formatting

- 4-space indentation.
- Allman brace style (opening brace on new line).
- Do not reformat entire files unless asked.

### Types and Language

- `var` when RHS type is obvious; explicit types also acceptable.
- Nullable reference types not enforced globally; don't mass-introduce annotations.
- Respect multi-target constraints: code in `src/AutoFilterer` must compile on `netstandard2.0`.

### Expressions

- Core logic uses `System.Linq.Expressions` and composable expression builders.
- Reuse existing abstractions (`CompareToAttribute`, `OperatorType`, `CombineType`, `ExpressionExtensions.Combine()`) before adding new mechanics.

### Error Handling

- Public argument validation with explicit exceptions (e.g., `ArgumentOutOfRangeException`).
- Respect `FilterBase.IgnoreExceptions` — when `true`, expression-building errors are caught and logged via `Debug.WriteLine`.
- No silent swallowing unless consistent with existing design.

## Test Conventions

- Framework: **xUnit** (`[Fact]`, `[Theory]`, `[InlineData]`).
- Assertions: **xUnit `Assert.*`** methods (e.g., `Assert.Equal`, `Assert.Contains`, `Assert.True`). FluentAssertions is referenced in one project but not actively used.
- Data generation: **AutoFixture + AutoMoq** via custom `[AutoMoqData(count: N)]` attribute (defined in `AutoFilterer.Tests.Core`). Default count is 3. Common counts: 16, 32, 64, 128.
- Shared test infrastructure (models, DTOs, `AutoMoqDataAttribute`) lives in `tests/AutoFilterer.Tests.Core`.
- Structure: Arrange / Act / Assert with deterministic assertions.
- Test classes may implement `IDisposable` for mock verification teardown.

## Agent Change Strategy

- Make minimal, focused diffs. Avoid unrelated cleanup.
- Preserve backward compatibility for public APIs unless explicitly allowed to break.
- Add or update tests in the nearest relevant test project when behavior changes.
- Always run the appropriate test suite from the validation matrix before considering work complete.

## CI Parity

Workflows: `.github/workflows/dotnetcore.yml` (build+test on push/PR to master/develop), `dotnetcore-nuget.yml` (manual pack+push), `deploy-sandbox.yml` (manual Heroku deploy).
Keep this file aligned with CI. If pipelines change, update this document.
