# Tests

## 📋 Propósito
Projetos de teste automatizados para garantir qualidade do código.

## 📁 Estrutura Esperada
```
tests/
├── KernelMind.UnitTests/           # Testes unitários
├── KernelMind.IntegrationTests/    # Testes de integração
└── KernelMind.E2ETests/            # Testes end-to-end
```

## 🧪 Tipos de Testes

### Unit Tests
- Testam lógica de negócio isolada
- Mockam dependências externas
- Rápidos e determinísticos

### Integration Tests
- Testam integração entre componentes
- Usam banco de dados em memória ou container
- Testam repositories e services

### E2E Tests
- Testam fluxos completos
- Simulam interação do usuário
- Testam API e frontend integrados

## 🚀 Comandos Úteis
```bash
# Executar todos os testes
dotnet test

# Executar com cobertura
dotnet test --collect:"XPlat Code Coverage"

# Executar testes específicos
dotnet test --filter "FullyQualifiedName~UnitTests"

# Verbose output
dotnet test --logger "console;verbosity=detailed"
```

## 📊 Cobertura de Código
Meta: mínimo 70% de cobertura
```bash
# Gerar relatório de cobertura
dotnet test --collect:"XPlat Code Coverage"
reportgenerator -reports:**/coverage.cobertura.xml -targetdir:coveragereport
```
