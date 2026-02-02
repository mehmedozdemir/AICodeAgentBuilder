# Domain Layer Architecture - v0.2.0

## Overview

The Domain layer represents the core business logic of the AI Code Agent Builder application. It follows **Domain-Driven Design (DDD)** principles and maintains complete independence from infrastructure concerns.

## Structure

```
Domain/
├── Common/
│   ├── BaseEntity.cs              # Base class for all entities
│   ├── ValueObject.cs             # Base class for value objects
│   ├── IRepository.cs             # Generic repository interface
│   └── IUnitOfWork.cs             # Unit of Work pattern
├── Enums/
│   ├── ParameterType.cs           # Parameter data types
│   ├── RuleSeverity.cs            # Rule severity levels
│   ├── AIResponseStatus.cs        # AI validation status
│   └── RuleScope.cs               # Rule application scope
├── ValueObjects/
│   ├── ParameterValue.cs          # Strongly-typed parameter values
│   ├── RuleConstraint.cs          # Rule severity + scope
│   └── TechStackVersion.cs        # Semantic versioning
├── Entities/
│   ├── Category.cs                # Technology categories
│   ├── TechStack.cs               # Technology stacks (owns StackParameters)
│   ├── StackParameter.cs          # Configurable parameters
│   ├── ArchitecturePattern.cs     # Architectural patterns
│   ├── EngineeringRule.cs         # Engineering constraints
│   ├── ProjectProfile.cs          # Aggregate Root
│   └── AIResponse.cs              # AI audit entity
└── Exceptions/
    ├── DomainException.cs                          # Base domain exception
    ├── EntityNotFoundException.cs
    ├── DomainValidationException.cs
    ├── InvalidProfileConfigurationException.cs
    ├── DuplicateEntityException.cs
    ├── InvalidParameterException.cs
    ├── RuleConflictException.cs
    └── AIValidationException.cs
```

## Aggregate Boundaries

### ProjectProfile (Primary Aggregate Root)
**Aggregate Members:**
- ProjectProfile (root)
- ProfileTechStack (owned entity)

**Purpose:** Ensures consistency of project configuration selections

**Invariants:**
- At least one tech stack must be selected
- All parameter values must be valid
- No duplicate tech stacks

### Category (Aggregate Root)
**Standalone entity**

**Invariants:**
- Unique name within system
- Name and description required

### TechStack (Aggregate Root)
**Aggregate Members:**
- TechStack (root)
- StackParameter collection (owned)

**Invariants:**
- Must belong to a category
- Unique name within category
- No duplicate parameter names

### ArchitecturePattern (Aggregate Root)
**Standalone entity**

**Invariants:**
- Unique name
- Complexity level between 1-5

### EngineeringRule (Aggregate Root)
**Standalone entity**

**Invariants:**
- Unique name
- Valid constraint configuration

### AIResponse (Aggregate Root)
**Standalone audit entity**

**Invariants:**
- Prompt and raw response required
- Status transitions are controlled

## Key Design Patterns

### 1. Value Objects
**ParameterValue**
- Encapsulates type and value
- Factory methods for type-safe creation
- Immutable once created

**RuleConstraint**
- Combines severity and scope
- Comparison logic for rule hierarchy
- Immutable

**TechStackVersion**
- Semantic versioning support
- Compatibility checking
- Version comparison logic

### 2. Factory Methods
All entities use static `Create()` methods:
```csharp
var category = Category.Create(name, description, isAIGenerated);
var techStack = TechStack.Create(categoryId, name, description);
var rule = EngineeringRule.Create(name, description, rationale, constraint);
```

### 3. Encapsulation
**Private Setters:**
```csharp
public string Name 
{ 
    get => _name; 
    private set => _name = value; 
}
```

**Encapsulated Collections:**
```csharp
private readonly List<StackParameter> _parameters = new();
public IReadOnlyCollection<StackParameter> Parameters => _parameters.AsReadOnly();
```

### 4. Business Logic in Entities

**Rich Domain Model Example:**
```csharp
// TechStack.AddParameter() enforces uniqueness
public void AddParameter(StackParameter parameter)
{
    if (_parameters.Any(p => p.Name.Equals(parameter.Name, StringComparison.OrdinalIgnoreCase)))
        throw new InvalidOperationException($"Parameter '{parameter.Name}' already exists.");
    
    _parameters.Add(parameter);
    SetUpdatedAt();
}
```

**Validation in Domain:**
```csharp
// StackParameter validates values based on type
public void ValidateValue(string value)
{
    switch (ParameterType)
    {
        case ParameterType.Boolean:
            if (!bool.TryParse(value, out _))
                throw new ArgumentException($"'{value}' is not a valid boolean.");
            break;
        // ... more validations
    }
}
```

### 5. Domain Events (Future)
Architecture supports domain events for:
- ProjectProfile configuration changes
- AI response validation
- Rule conflict detection

## Entity Relationships

```
Category (1) ──── (N) TechStack
                       │
                       └── (N) StackParameter

ProjectProfile (1) ──── (N) ProfileTechStack
                │
                ├─── (N) ArchitecturePattern (reference)
                └─── (N) EngineeringRule (reference)

AIResponse (audit, standalone)
```

## Domain Rules

### Category
1. Name must be unique
2. Name max 100 characters
3. Description max 500 characters
4. Can be deactivated but not deleted (historical integrity)

### TechStack
1. Must belong to a category
2. Name unique within category
3. Can have 0 to N parameters
4. Parameters must have unique names
5. Version format validated

### StackParameter
1. Name unique within tech stack
2. Type determines validation rules
3. Enum types must have allowed values
4. Default value must match type
5. Required parameters enforced at profile level

### ArchitecturePattern
1. Complexity level 1-5
2. Team size suitability flags
3. Guidelines required (max 2000 chars)

### EngineeringRule
1. Must have constraint (severity + scope)
2. Can detect conflicts with other rules
3. Enforcement can be toggled
4. Example code max 5000 chars

### ProjectProfile
1. Must have at least one tech stack
2. Must have at least one architecture pattern
3. ProjectName required for generation
4. All parameter values must be valid
5. No duplicate tech stacks

### AIResponse
1. Prompt and raw response immutable
2. Status can only transition forward
3. Validation requires validator identity
4. Metrics are optional but encouraged

## Validation Strategy

### Constructor Validation
All entities validate in constructors:
```csharp
private Category(string name, string description)
{
    SetName(name);      // Validates and assigns
    SetDescription(description);
    IsActive = true;
}
```

### Method-Level Validation
Public methods validate inputs:
```csharp
public void SetName(string name)
{
    if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException("Name cannot be empty.");
    
    if (name.Length > 100)
        throw new ArgumentException("Name cannot exceed 100 characters.");
    
    _name = name.Trim();
    SetUpdatedAt();
}
```

### Value Object Validation
Value objects validate on creation:
```csharp
public static ParameterValue CreateNumber(decimal value)
{
    // Validation happens here
    return new ParameterValue(ParameterType.Number, value.ToString());
}
```

## Exception Hierarchy

```
Exception
└── DomainException (abstract)
    ├── EntityNotFoundException
    ├── DomainValidationException
    ├── InvalidProfileConfigurationException
    ├── DuplicateEntityException
    ├── InvalidParameterException
    ├── RuleConflictException
    └── AIValidationException
```

## Key Behaviors

### TechStack Ownership
TechStack owns StackParameters:
- Parameters cannot exist without a TechStack
- Adding/removing parameters updates TechStack timestamp
- Parameter validation uses TechStack context

### ProjectProfile Composition
ProjectProfile composes references:
- Holds IDs of ArchitecturePatterns and EngineeringRules
- Owns ProfileTechStack instances with parameter values
- Validates completeness before generation

### AIResponse Lifecycle
1. Created with Pending status
2. Can be marked as RequiresReview
3. Must be Validated before use
4. Can be Rejected with reasons
5. Immutable prompt/response for audit

## Design Principles Applied

### SOLID
- **Single Responsibility**: Each entity has one reason to change
- **Open/Closed**: Entities can be extended via inheritance
- **Liskov Substitution**: Base classes (BaseEntity, ValueObject) are substitutable
- **Interface Segregation**: Small, focused interfaces (IRepository, IUnitOfWork)
- **Dependency Inversion**: Domain depends on abstractions only

### DDD Tactical Patterns
- ✅ Entities
- ✅ Value Objects
- ✅ Aggregates
- ✅ Repositories (interface only)
- ✅ Domain Exceptions
- ✅ Factory Methods
- ⏳ Domain Events (future)
- ⏳ Domain Services (if needed)

### Clean Architecture
- ✅ No dependencies on outer layers
- ✅ No infrastructure concerns
- ✅ No UI concerns
- ✅ No persistence attributes
- ✅ Pure business logic

## Usage Examples

### Creating a Project Profile
```csharp
// Create profile
var profile = ProjectProfile.Create("My API Project", "REST API with microservices");
profile.SetProjectName("CompanyName.Api");
profile.SetTargetTeamSize(8);

// Add tech stack with parameters
var paramValues = new Dictionary<string, ParameterValue>
{
    ["Version"] = ParameterValue.CreateVersion("8.0"),
    ["UseHttps"] = ParameterValue.CreateBoolean(true)
};
profile.AddTechStack(aspNetCoreId, paramValues);

// Add architecture and rules
profile.AddArchitecturePattern(microservicesPatternId);
profile.AddEngineeringRule(testCoverageRuleId);

// Validate
if (!profile.IsValid())
    throw new InvalidProfileConfigurationException(profile.Name, "Profile is incomplete");
```

### Working with TechStack
```csharp
// Create tech stack
var techStack = TechStack.Create(backendCategoryId, "ASP.NET Core", "Microsoft web framework");
techStack.SetDefaultVersion("8.0");
techStack.SetDocumentationUrl("https://docs.microsoft.com/aspnet/core");

// Add parameters
var versionParam = StackParameter.Create("Version", "Framework version", ParameterType.Version);
versionParam.SetRequired(true);
versionParam.SetDefaultValue("8.0");
techStack.AddParameter(versionParam);

var portParam = StackParameter.Create("Port", "HTTP port", ParameterType.Number);
portParam.SetDefaultValue("5000");
techStack.AddParameter(portParam);
```

### Engineering Rule with Constraint
```csharp
var constraint = RuleConstraint.Create(RuleSeverity.Error, RuleScope.Testing);
var rule = EngineeringRule.Create(
    "Unit Test Coverage > 80%",
    "All code must have at least 80% unit test coverage",
    "Prevents regressions and ensures code quality",
    constraint
);
rule.SetImplementationGuidance("Use xUnit and Coverlet for .NET projects");
rule.SetEnforced(true);
```

## Future Enhancements

1. **Domain Events**
   - ProjectProfileConfigured
   - TechStackAdded
   - AIResponseValidated

2. **Domain Services**
   - ProfileValidationService
   - RuleConflictResolver
   - CompatibilityChecker

3. **Specifications Pattern**
   - Complex query logic
   - Reusable business rules

4. **Audit Trail**
   - Track all entity changes
   - Who/when/what changed

## Testing Strategy

The Domain layer should be tested with:
- Unit tests for all entity behaviors
- Value object equality tests
- Exception scenario tests
- Invariant validation tests
- Business rule tests

Example:
```csharp
[Fact]
public void TechStack_ShouldNotAllowDuplicateParameters()
{
    var techStack = TechStack.Create(categoryId, "Test", "Test");
    var param1 = StackParameter.Create("Version", "V1", ParameterType.String);
    var param2 = StackParameter.Create("Version", "V2", ParameterType.String);
    
    techStack.AddParameter(param1);
    
    Assert.Throws<InvalidOperationException>(() => techStack.AddParameter(param2));
}
```

## Conclusion

The Domain layer is production-ready and follows enterprise-grade patterns:
- ✅ Complete DDD implementation
- ✅ Clean Architecture compliance
- ✅ SOLID principles applied
- ✅ Comprehensive validation
- ✅ Rich domain model
- ✅ Zero infrastructure coupling
- ✅ Extensive documentation

**Next Steps:**
- Implement database mappings in Infrastructure layer
- Create use cases in Application layer
- Build UI for entity management
