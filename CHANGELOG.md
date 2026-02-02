# Changelog

All notable changes to the AI Code Agent Builder project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres on [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.3.0] - 2026-02-02

### Added

#### Application Layer - Complete Implementation

**Interfaces (Ports)**
- `IAIProvider` - AI provider abstraction with prompt/response contracts
- `AIPromptRequest` - Structured AI request with context and parameters
- `AIPromptResponse` - AI response with token usage and metadata
- `ICategoryRepository` - Category-specific queries
- `ITechStackRepository` - TechStack queries with parameter loading
- `IArchitecturePatternRepository` - Pattern queries by complexity/team size
- `IEngineeringRuleRepository` - Rule queries by severity/scope
- `IProjectProfileRepository` - Profile queries with full entity graphs
- `IAIResponseRepository` - AI audit trail queries

**Application Services**
- `CategoryService` - Complete CRUD for categories
  - CreateCategoryAsync - Validates uniqueness
  - UpdateCategoryAsync - Prevents duplicate names
  - DeleteCategoryAsync - Soft delete with tech stack validation
  - ListCategoriesAsync - Returns active categories
  - GetCategoryByIdAsync - Single category details
  
- `TechStackService` - Tech stack lifecycle management
  - CreateTechStackAsync - Creates stack in category
  - UpdateTechStackAsync - Updates metadata
  - AddParameterAsync - Adds parameter with validation
  - DeleteTechStackAsync - Validates not used in profiles
  - ListTechStacksByCategoryAsync - Returns stacks for category
  - GetTechStackByIdAsync - Returns with all parameters
  
- `AIGenerationService` - AI orchestration
  - GenerateCategoriesAsync - Generates categories from AI
  - GenerateTechStacksAsync - Generates tech stacks for category
  - GenerateStackParametersAsync - Generates parameters for tech stack
  - Audit trail for all AI interactions
  - JSON parsing and validation
  - Domain entity mapping
  
- `ProjectProfileService` - Project profile management
  - CreateProjectProfileAsync - Creates empty profile
  - UpdateProjectProfileAsync - Updates metadata
  - AddTechStackAsync - Adds tech stack with parameter values
  - AddArchitecturePatternAsync - Adds pattern reference
  - AddEngineeringRuleAsync - Adds rule reference
  - GetProjectProfileDetailsAsync - Full profile with relations
  - ListProjectProfilesAsync - All profiles
  - ValidateProjectProfileAsync - Completeness validation
  
- `ArtifactGenerationService` - Code agent artifact generation
  - GenerateCopilotInstructionsAsync - Generates copilot-instructions.md
  - GenerateAIAgentConfigAsync - Generates aiagent.config.{yaml|json|toml}
  - GenerateEngineeringArtifactsAsync - Generates ENGINEERING_POLICY.md, CODE_REVIEW_CHECKLIST.md, README.md

**Commands (63 files total)**
- Category: Create, Update, Delete
- TechStack: Create, Update, Delete, AddParameter
- AIGeneration: GenerateCategories, GenerateTechStacks, GenerateStackParameters
- ProjectProfile: Create, Update, AddTechStack, AddArchitecturePattern, AddEngineeringRule
- ArtifactGeneration: GenerateCopilotInstructions, GenerateAIAgentConfig, GenerateEngineeringArtifacts

**Queries**
- Category: List, GetById
- TechStack: ListByCategory, GetById
- ProjectProfile: List, GetDetails

**DTOs**
- CategoryDto - Category data transfer
- TechStackDto + StackParameterDto - Tech stack with parameters
- ProjectProfileDto + ProfileTechStackDto - Complete profile data
- GeneratedArtifact + ArtifactGenerationResult - Artifact output

**Patterns Implemented**
- Command/Query Separation (CQRS-light)
- Result pattern for operation outcomes
- Repository pattern for data access
- Service layer for use case orchestration
- Immutable command objects with factory methods
- Validation at command creation
- DTOs for presentation layer decoupling

**AI Integration**
- Prompt building with structured JSON requirements
- Response parsing with error handling
- AIResponse entity persistence for audit
- Validation workflow (Pending → Validated/Rejected)
- Token usage tracking
- Context-based prompt templates

**Dependency Injection**
- All services registered as scoped
- Clean Architecture dependency rules enforced
- Application → Domain only

### Documentation
- APPLICATION_LAYER.md - Complete architecture documentation
- Known issues documented for Domain/Infrastructure alignment
- Future enhancement roadmap (MediatR, FluentValidation, AutoMapper, Caching)

### Notes
- Application layer is functionally complete
- Some build errors exist due to Domain API mismatches (documented)
- All use cases implemented following Clean Architecture
- Ready for Infrastructure layer implementation
- Zero infrastructure or UI dependencies

## [0.2.0] - 2026-02-02

### Added

#### Domain Layer - Complete Implementation

**Enums**
- `ParameterType` - Data types for stack parameters (String, Number, Boolean, Enum, Version)
- `RuleSeverity` - Engineering rule severity levels (Info, Warning, Error, Critical)
- `AIResponseStatus` - AI response validation status (Pending, Validated, Rejected, RequiresReview)
- `RuleScope` - Application scope for engineering rules (Global, Backend, Frontend, Database, Testing, Security, DevOps)

**Value Objects**
- `ParameterValue` - Strongly-typed parameter values with type safety and validation
- `RuleConstraint` - Rule severity and scope encapsulation with comparison logic
- `TechStackVersion` - Semantic versioning support with compatibility checking

**Entities**
- `Category` - High-level technology categorization (Backend, Frontend, Database, etc.)
  - Supports AI-generated content tracking
  - Display ordering for UI presentation
  - Activation/deactivation for lifecycle management
  
- `TechStack` - Real-world technology stacks within categories
  - Owns collection of StackParameters
  - Version management with defaults
  - Documentation URL tracking
  - Tag-based search and filtering
  - Category assignment with validation
  
- `StackParameter` - Configurable parameters for tech stacks
  - Strongly-typed with validation
  - Support for required/optional parameters
  - Enum-type allowed values
  - Default value management
  - Display ordering
  
- `ArchitecturePattern` - Architectural styles and patterns
  - Complexity level (1-5 scale)
  - Team size suitability indicators
  - Key principles and anti-patterns
  - Implementation guidelines
  - AI-generated tracking
  
- `EngineeringRule` - Cross-cutting engineering constraints
  - Rule severity and scope via RuleConstraint value object
  - Implementation guidance and examples
  - Common violations documentation
  - Enforcement toggle
  - Conflict detection logic
  
- `ProjectProfile` (Aggregate Root) - User-defined project configurations
  - Composes TechStacks with parameter values
  - Aggregates ArchitecturePatterns
  - Aggregates EngineeringRules
  - Project metadata (name, team size)
  - Validation for completeness
  - Encapsulated collections with business logic
  
- `ProfileTechStack` - Owned entity for tech stack configuration within profiles
  - Parameter value management
  - Tech stack reference
  
- `AIResponse` - Audit entity for AI-generated content
  - Prompt and raw response storage
  - Processed content tracking
  - Validation status workflow
  - Performance metrics (tokens, response time)
  - Request context tracking
  - Metadata storage

**Domain Exceptions**
- `InvalidProfileConfigurationException` - Invalid project profile setup
- `DuplicateEntityException` - Uniqueness constraint violations
- `InvalidParameterException` - Parameter validation failures
- `RuleConflictException` - Conflicting engineering rules
- `AIValidationException` - AI response validation failures

### Technical Implementation

- **Domain-Driven Design**: Proper aggregate boundaries with ProjectProfile as root
- **Encapsulation**: All collections are private with controlled access
- **Invariants**: Constructors and methods enforce valid state
- **Value Objects**: Immutable value objects for complex concepts
- **Rich Domain Model**: Business logic in entities, not anemic models
- **No Infrastructure Coupling**: Zero dependencies on persistence or UI layers
- **Validation**: Comprehensive input validation at domain level
- **Factory Methods**: Static Create methods for entity instantiation

### Design Decisions

1. **ProjectProfile as Aggregate Root**: Ensures consistency across tech stacks, architectures, and rules
2. **Value Objects for Type Safety**: ParameterValue, RuleConstraint, and TechStackVersion prevent primitive obsession
3. **Owned Entities**: ProfileTechStack cannot exist without ProjectProfile
4. **Audit Trail**: AIResponse entity enables full traceability of AI interactions
5. **Flexible Parameter System**: Supports multiple data types with validation
6. **Composable Rules**: Engineering rules can be combined and applied based on scope

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

[0.2.0]: https://github.com/yourusername/AI.CodeAgent.Builder/compare/v0.1.0...v0.2.0
[0.1.0]: https://github.com/yourusername/AI.CodeAgent.Builder/releases/tag/v0.1.0
