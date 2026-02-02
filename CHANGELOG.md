# Changelog

All notable changes to the AI Code Agent Builder project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.1.0] - 2026-02-02

### Added

#### Solution Structure
- Created .NET 8 solution with Clean Architecture
- Implemented four-layer architecture: Domain, Application, Infrastructure, and UI

#### Domain Layer (AI.CodeAgent.Builder.Domain)
- Added `BaseEntity` class with audit fields (Id, CreatedAt, UpdatedAt)
- Added `ValueObject` base class for Domain-Driven Design patterns
- Implemented `IRepository<T>` generic repository interface
- Implemented `IUnitOfWork` interface for transaction management
- Added domain exceptions:
  - `DomainException` (base class)
  - `EntityNotFoundException`
  - `DomainValidationException`

#### Application Layer (AI.CodeAgent.Builder.Application)
- Added `Result<T>` and `Result` wrappers for operation responses
- Created `IApplicationDbContext` interface
- Created `ITemplateService` interface for Scriban templates
- Implemented dependency injection configuration

#### Infrastructure Layer (AI.CodeAgent.Builder.Infrastructure)
- Implemented `ApplicationDbContext` with Entity Framework Core
- Implemented generic `Repository<T>` with full CRUD operations
- Implemented `UnitOfWork` with transaction support
- Implemented `TemplateService` using Scriban template engine
- Configured SQLite database with automatic timestamp updates
- Added dependency injection configuration for infrastructure services

#### UI Layer (AI.CodeAgent.Builder.UI)
- Created Avalonia UI project with MVVM pattern
- Implemented `BaseViewModel` with INotifyPropertyChanged
- Implemented `NavigationService` for view navigation
- Created `MainWindowViewModel` as application entry point
- Configured dependency injection with Microsoft.Extensions.Hosting
- Added application configuration (appsettings.json)

#### Dependencies
- Microsoft.EntityFrameworkCore.Sqlite 8.0.11
- Microsoft.EntityFrameworkCore.Design 8.0.11
- Scriban 5.10.0
- Microsoft.Extensions.DependencyInjection 8.0.1
- Microsoft.Extensions.DependencyInjection.Abstractions 8.0.2
- Microsoft.Extensions.Hosting 8.0.1
- Avalonia 11.2.1
- Avalonia.Desktop 11.2.1
- Avalonia.Themes.Fluent 11.2.1

#### Documentation
- Created comprehensive README.md
- Created SOLUTION_STRUCTURE.md with detailed architecture documentation
- Added .gitignore for .NET projects

### Technical Details
- Target Framework: .NET 8
- Architecture: Clean Architecture with SOLID principles
- UI Pattern: MVVM (Model-View-ViewModel)
- Database: SQLite with Entity Framework Core
- Template Engine: Scriban
- Dependency Injection: Microsoft.Extensions.DependencyInjection

### Build Status
- ✅ All projects compile successfully
- ✅ Clean Architecture dependencies enforced
- ✅ Production-ready code quality
- ✅ No warnings or errors

[0.1.0]: https://github.com/yourusername/AI.CodeAgent.Builder/releases/tag/v0.1.0
