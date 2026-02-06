# Tests

## Purpose
Automated test projects to ensure code quality.

## Expected Structure
```
tests/
├── KernelMind.UnitTests/           # Unit tests
├── KernelMind.IntegrationTests/    # Integration tests
└── KernelMind.E2ETests/            # End-to-end tests
```

## Test Types

### Unit Tests
- Test isolated business logic
- Mock external dependencies
- Fast and deterministic

### Integration Tests
- Test component integration
- Use in-memory or container database
- Test repositories and services

### E2E Tests
- Test complete flows
- Simulate user interaction
- Test integrated API and frontend

## Useful Commands
```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific tests
dotnet test --filter "FullyQualifiedName~UnitTests"

# Verbose output
dotnet test --logger "console;verbosity=detailed"
```

## Code Coverage
Goal: minimum 70% coverage
```bash
# Generate coverage report
dotnet test --collect:"XPlat Code Coverage"
reportgenerator -reports:**/coverage.cobertura.xml -targetdir:coveragereport
```
