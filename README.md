# AI Code Agent Builder

## Project Overview

A production-grade desktop application for generating AI Code Agent infrastructure, including copilot-instructions, AI configuration files, policies, templates, and governance artifacts.

## Solution Structure

The solution follows **Clean Architecture** principles with clear separation of concerns:

```
AI.CodeAgent.Builder/
├── src/
│   ├── AI.CodeAgent.Builder.Domain/          # Core business entities and rules
│   ├── AI.CodeAgent.Builder.Application/     # Use cases and business logic
│   ├── AI.CodeAgent.Builder.Infrastructure/  # Data access and external services
│   └── AI.CodeAgent.Builder.UI/              # Avalonia UI (MVVM)
└── AI.CodeAgent.Builder.sln
```

## Architecture

### Domain Layer (Innermost)
- **Dependencies**: None
- **Contains**:
  - Base entities (`BaseEntity`, `ValueObject`)
  - Domain interfaces (`IRepository`, `IUnitOfWork`)
  - Domain exceptions
- **Purpose**: Core business logic, no dependencies on other layers

### Application Layer
- **Dependencies**: Domain
- **Contains**:
  - Application interfaces (`IApplicationDbContext`, `ITemplateService`)
  - Result wrappers
  - Dependency injection configuration
- **Purpose**: Use cases, orchestration, business workflows

### Infrastructure Layer
- **Dependencies**: Domain, Application
- **Contains**:
  - Entity Framework Core DbContext
  - Repository implementations
  - Unit of Work implementation
  - Scriban template service
  - SQLite database configuration
- **Purpose**: Data persistence, external service implementations

### UI Layer (Outermost)
- **Dependencies**: Application, Infrastructure
- **Contains**:
  - Avalonia views and viewmodels
  - MVVM infrastructure (`BaseViewModel`, `NavigationService`)
  - Dependency injection composition root
- **Purpose**: User interface, presentation logic

## Technology Stack

- **Framework**: .NET 8
- **UI**: Avalonia UI 11.2.1
- **Database**: SQLite (Entity Framework Core 8.0.11)
- **Template Engine**: Scriban 5.10.0
- **DI**: Microsoft.Extensions.DependencyInjection 8.0.1
- **Hosting**: Microsoft.Extensions.Hosting 8.0.1
- **Pattern**: MVVM (Model-View-ViewModel)

## Key Features

### Already Implemented

1. **Clean Architecture Structure**
   - Strict layer separation
   - Dependency rules enforced via project references
   - No circular dependencies

2. **Domain Foundation**
   - Base entity with audit fields (Id, CreatedAt, UpdatedAt)
   - Value Object base class for DDD patterns
   - Generic repository interface
   - Unit of Work pattern
   - Domain exceptions (DomainException, EntityNotFoundException, DomainValidationException)

3. **Application Layer Infrastructure**
   - Result wrapper for operation responses
   - IApplicationDbContext interface
   - ITemplateService interface for Scriban templates

4. **Infrastructure Implementation**
   - SQLite DbContext with automatic UpdatedAt handling
   - Generic Repository<T> implementation
   - Unit of Work with transaction support
   - Template service using Scriban
   - Dependency injection configuration

5. **MVVM Infrastructure**
   - BaseViewModel with INotifyPropertyChanged
   - Navigation service for view management
   - Dependency injection integrated with Avalonia
   - MainWindowViewModel as composition root

6. **Dependency Injection**
   - Configured in App.axaml.cs
   - Application layer registration
   - Infrastructure layer registration
   - Navigation service registration
   - ViewModel registration

## Building and Running

### Prerequisites
- .NET 8 SDK
- Visual Studio 2022 / VS Code / Rider

### Build
```bash
dotnet build
```

### Run
```bash
dotnet run --project src/AI.CodeAgent.Builder.UI
```

## Configuration

The application uses `appsettings.json` in the UI project:

```json
{
  "Database": {
    "FileName": "aiagentbuilder.db"
  }
}
```

Database location: `%LocalAppData%\AI.CodeAgent.Builder\aiagentbuilder.db`

## Design Principles

### SOLID Principles Applied
- **Single Responsibility**: Each class has one reason to change
- **Open/Closed**: Entities open for extension, closed for modification
- **Liskov Substitution**: Interfaces used throughout
- **Interface Segregation**: Small, focused interfaces
- **Dependency Inversion**: Depends on abstractions, not concretions

### Clean Architecture Rules
1. Domain layer has no dependencies
2. Application layer depends only on Domain
3. Infrastructure implements interfaces from Application
4. UI layer is the composition root

### Code Quality Standards
- No hardcoded business data
- Explicit, readable code
- All public methods are meaningful
- Production-ready quality
- No demo or placeholder code

## Next Steps

The following features are planned but not yet implemented:

- [ ] Database schema and migrations
- [ ] Entity definitions (Category, TechStack, etc.)
- [ ] Feature-specific ViewModels
- [ ] Feature-specific Views
- [ ] AI integration services
- [ ] Template management
- [ ] Project profile management

## Contributing

This project follows enterprise standards:
- No prototype code
- No magic values
- Document assumptions in comments
- Follow existing patterns

## License

[To be determined]
