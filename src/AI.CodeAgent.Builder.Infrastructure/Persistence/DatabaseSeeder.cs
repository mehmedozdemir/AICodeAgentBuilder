using AI.CodeAgent.Builder.Domain.Entities;
using AI.CodeAgent.Builder.Domain.Enums;
using AI.CodeAgent.Builder.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace AI.CodeAgent.Builder.Infrastructure.Persistence;

/// <summary>
/// Seeds initial data for the application following domain-driven design principles.
/// All data respects domain entity validations and business rules.
/// </summary>
public static class DatabaseSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Check if data already exists
        if (await context.Set<Category>().AnyAsync())
        {
            return; // Database already seeded
        }

        await SeedCategoriesAsync(context);
        await SeedTechStacksAsync(context);
        await SeedArchitecturePatternsAsync(context);
        await SeedEngineeringRulesAsync(context);

        await context.SaveChangesAsync();
    }

    private static async Task SeedCategoriesAsync(ApplicationDbContext context)
    {
        var categories = new[]
        {
            CreateCategory("Backend", 
                "Server-side frameworks, APIs, and microservices for building scalable business logic and data processing", 
                1),
            
            CreateCategory("Frontend", 
                "Client-side frameworks, libraries, and tools for building modern, responsive user interfaces", 
                2),
            
            CreateCategory("Database", 
                "Relational and NoSQL database systems, ORMs, and data persistence technologies", 
                3),
            
            CreateCategory("DevOps & Infrastructure", 
                "CI/CD tools, containerization, orchestration, and infrastructure-as-code platforms", 
                4),
            
            CreateCategory("Cloud Services", 
                "Cloud platforms, serverless computing, and managed services for scalable deployments", 
                5),
            
            CreateCategory("Mobile Development", 
                "Native and cross-platform frameworks for iOS, Android, and hybrid mobile applications", 
                6),
            
            CreateCategory("Testing & QA", 
                "Testing frameworks, test runners, mocking libraries, and quality assurance tools", 
                7),
            
            CreateCategory("Security & Authentication", 
                "Security frameworks, authentication providers, encryption libraries, and identity management", 
                8),
            
            CreateCategory("Messaging & Event Streaming", 
                "Message brokers, event streaming platforms, and pub/sub systems for distributed architectures", 
                9),
            
            CreateCategory("Monitoring & Observability", 
                "Application performance monitoring, logging, tracing, and metrics collection tools", 
                10),
            
            CreateCategory("AI & Machine Learning", 
                "Machine learning frameworks, AI platforms, and data science tools for intelligent applications", 
                11),
            
            CreateCategory("Real-time & WebSockets", 
                "Real-time communication libraries, WebSocket frameworks, and push notification services", 
                12)
        };

        await context.Set<Category>().AddRangeAsync(categories);
    }

    private static async Task SeedTechStacksAsync(ApplicationDbContext context)
    {
        // Get category IDs
        var categories = await context.Set<Category>().ToListAsync();
        var backend = categories.FirstOrDefault(c => c.Name == "Backend");
        var frontend = categories.FirstOrDefault(c => c.Name == "Frontend");
        var database = categories.FirstOrDefault(c => c.Name == "Database");
        var devops = categories.FirstOrDefault(c => c.Name == "DevOps & Infrastructure");
        var cloud = categories.FirstOrDefault(c => c.Name == "Cloud Services");
        var mobile = categories.FirstOrDefault(c => c.Name == "Mobile Development");

        var stacks = new List<TechStack>();

        // Backend stacks
        if (backend != null)
        {
            stacks.Add(TechStack.Create(backend.Id, "ASP.NET Core", 
                "Modern, cross-platform framework for building web APIs and applications"));
            stacks.Add(TechStack.Create(backend.Id, "Spring Boot", 
                "Popular Java framework for microservices and enterprise applications"));
            stacks.Add(TechStack.Create(backend.Id, "Node.js (Express)", 
                "JavaScript runtime with Express framework for building scalable server applications"));
            stacks.Add(TechStack.Create(backend.Id, "Django", 
                "High-level Python web framework for rapid development"));
            stacks.Add(TechStack.Create(backend.Id, "FastAPI", 
                "Modern, fast Python framework for building APIs with automatic OpenAPI documentation"));
        }

        // Frontend stacks
        if (frontend != null)
        {
            stacks.Add(TechStack.Create(frontend.Id, "React", 
                "JavaScript library for building component-based user interfaces"));
            stacks.Add(TechStack.Create(frontend.Id, "Angular", 
                "Full-featured TypeScript framework by Google for enterprise applications"));
            stacks.Add(TechStack.Create(frontend.Id, "Vue.js", 
                "Progressive JavaScript framework for building user interfaces"));
            stacks.Add(TechStack.Create(frontend.Id, "Svelte", 
                "Modern compiler-based framework with minimal runtime overhead"));
        }

        // Database stacks
        if (database != null)
        {
            stacks.Add(TechStack.Create(database.Id, "PostgreSQL", 
                "Advanced open-source relational database with robust features"));
            stacks.Add(TechStack.Create(database.Id, "MongoDB", 
                "Popular NoSQL document database for flexible schema design"));
            stacks.Add(TechStack.Create(database.Id, "SQL Server", 
                "Microsoft's enterprise-grade relational database management system"));
            stacks.Add(TechStack.Create(database.Id, "Redis", 
                "In-memory data store for caching, session management, and real-time applications"));
        }

        // DevOps stacks
        if (devops != null)
        {
            stacks.Add(TechStack.Create(devops.Id, "Docker", 
                "Containerization platform for packaging and deploying applications"));
            stacks.Add(TechStack.Create(devops.Id, "Kubernetes", 
                "Container orchestration platform for automating deployment and scaling"));
            stacks.Add(TechStack.Create(devops.Id, "GitHub Actions", 
                "CI/CD platform integrated with GitHub for automated workflows"));
            stacks.Add(TechStack.Create(devops.Id, "Terraform", 
                "Infrastructure-as-code tool for provisioning cloud resources"));
        }

        // Cloud stacks
        if (cloud != null)
        {
            stacks.Add(TechStack.Create(cloud.Id, "Azure", 
                "Microsoft's cloud platform with comprehensive services for building, deploying, and managing applications"));
            stacks.Add(TechStack.Create(cloud.Id, "AWS", 
                "Amazon Web Services - comprehensive cloud platform with vast ecosystem"));
            stacks.Add(TechStack.Create(cloud.Id, "Google Cloud", 
                "Google's cloud platform with strengths in AI/ML and data analytics"));
        }

        // Mobile stacks
        if (mobile != null)
        {
            stacks.Add(TechStack.Create(mobile.Id, "React Native", 
                "Cross-platform mobile framework using React for iOS and Android"));
            stacks.Add(TechStack.Create(mobile.Id, "Flutter", 
                "Google's UI toolkit for building natively compiled mobile applications"));
            stacks.Add(TechStack.Create(mobile.Id, ".NET MAUI", 
                "Microsoft's cross-platform framework for iOS, Android, macOS, and Windows"));
        }

        await context.Set<TechStack>().AddRangeAsync(stacks);
    }

    private static async Task SeedArchitecturePatternsAsync(ApplicationDbContext context)
    {
        var patterns = new[]
        {
            ArchitecturePattern.Create(
                "Clean Architecture",
                "Separation of concerns with dependency inversion principle at its core",
                "Domain entities at the center, Application layer for use cases, Infrastructure for external concerns. Dependencies point inward."
            ),
            
            ArchitecturePattern.Create(
                "Hexagonal Architecture (Ports & Adapters)",
                "Isolates core business logic from external systems through ports and adapters",
                "Core domain defines ports (interfaces), adapters implement external system integrations. Enables technology-agnostic business logic."
            ),
            
            ArchitecturePattern.Create(
                "Microservices",
                "Distributed architecture with independently deployable, loosely-coupled services",
                "Each service owns its data, communicates via APIs, scales independently. Enables organizational autonomy and technology diversity."
            ),
            
            ArchitecturePattern.Create(
                "Event-Driven Architecture",
                "System components communicate through asynchronous event publishing and consumption",
                "Decoupled services react to domain events. Enables scalability, eventual consistency, and complex event processing."
            ),
            
            ArchitecturePattern.Create(
                "CQRS (Command Query Responsibility Segregation)",
                "Separates read and write operations into distinct models",
                "Commands modify state, queries read state. Enables independent scaling, optimized read models, and event sourcing integration."
            ),
            
            ArchitecturePattern.Create(
                "Layered (N-Tier) Architecture",
                "Traditional multi-layered separation with presentation, business, data access, and database layers",
                "Each layer depends only on layers below it. Simple to understand but can lead to tight coupling if not carefully managed."
            ),
            
            ArchitecturePattern.Create(
                "Serverless Architecture",
                "Event-driven, ephemeral compute with managed infrastructure and automatic scaling",
                "Functions-as-a-Service (FaaS) with stateless execution. Pay-per-use pricing, infinite scale, minimal operational overhead."
            ),
            
            ArchitecturePattern.Create(
                "Domain-Driven Design (DDD)",
                "Software design approach focused on complex domain modeling and ubiquitous language",
                "Bounded contexts, aggregates, entities, value objects, domain events. Strategic and tactical patterns for complex business domains."
            )
        };

        await context.Set<ArchitecturePattern>().AddRangeAsync(patterns);
    }

    private static async Task SeedEngineeringRulesAsync(ApplicationDbContext context)
    {
        var rules = new[]
        {
            // SOLID Principles
            EngineeringRule.Create(
                "Single Responsibility Principle (SRP)",
                "A class should have only one reason to change",
                "Each class should focus on a single responsibility. This improves maintainability, testability, and reduces coupling.",
                RuleConstraint.Create(RuleSeverity.Error, RuleScope.Global)
            ),
            
            EngineeringRule.Create(
                "Open/Closed Principle (OCP)",
                "Software entities should be open for extension but closed for modification",
                "Design classes that can be extended through inheritance or composition without modifying existing code.",
                RuleConstraint.Create(RuleSeverity.Warning, RuleScope.Global)
            ),
            
            EngineeringRule.Create(
                "Liskov Substitution Principle (LSP)",
                "Derived classes must be substitutable for their base classes",
                "Subtypes must maintain the behavioral contract of their parent types without breaking client expectations.",
                RuleConstraint.Create(RuleSeverity.Error, RuleScope.Backend)
            ),
            
            EngineeringRule.Create(
                "Interface Segregation Principle (ISP)",
                "Clients should not depend on interfaces they don't use",
                "Create focused, cohesive interfaces rather than large, monolithic ones. Prevents unnecessary coupling.",
                RuleConstraint.Create(RuleSeverity.Warning, RuleScope.Global)
            ),
            
            EngineeringRule.Create(
                "Dependency Inversion Principle (DIP)",
                "Depend on abstractions, not concretions",
                "High-level modules should not depend on low-level modules. Both should depend on abstractions.",
                RuleConstraint.Create(RuleSeverity.Critical, RuleScope.Global)
            ),
            
            // Code Quality Principles
            EngineeringRule.Create(
                "DRY (Don't Repeat Yourself)",
                "Every piece of knowledge should have a single, unambiguous representation",
                "Eliminate code duplication through abstraction, composition, and reusable components.",
                RuleConstraint.Create(RuleSeverity.Warning, RuleScope.Global)
            ),
            
            EngineeringRule.Create(
                "YAGNI (You Aren't Gonna Need It)",
                "Don't implement features until they are actually needed",
                "Avoid speculative generality and premature abstraction. Build what is required now, not what might be needed later.",
                RuleConstraint.Create(RuleSeverity.Info, RuleScope.Global)
            ),
            
            EngineeringRule.Create(
                "KISS (Keep It Simple, Stupid)",
                "Favor simplicity over complexity in design and implementation",
                "Simple solutions are easier to understand, test, and maintain. Avoid over-engineering.",
                RuleConstraint.Create(RuleSeverity.Warning, RuleScope.Global)
            ),
            
            // Security Rules
            EngineeringRule.Create(
                "Input Validation",
                "All external input must be validated before processing",
                "Validate, sanitize, and escape all user input. Prevent injection attacks, XSS, and data corruption.",
                RuleConstraint.Create(RuleSeverity.Critical, RuleScope.Global)
            ),
            
            EngineeringRule.Create(
                "Secure Authentication & Authorization",
                "Implement secure authentication and role-based authorization",
                "Use industry-standard protocols (OAuth 2.0, OIDC). Never store passwords in plain text. Enforce principle of least privilege.",
                RuleConstraint.Create(RuleSeverity.Critical, RuleScope.Backend)
            ),
            
            // Testing Rules
            EngineeringRule.Create(
                "Unit Test Coverage",
                "Maintain minimum 80% code coverage with meaningful tests",
                "Write unit tests for business logic, edge cases, and error handling. Focus on behavior, not implementation details.",
                RuleConstraint.Create(RuleSeverity.Warning, RuleScope.Global)
            ),
            
            EngineeringRule.Create(
                "Test Pyramid",
                "Follow the test pyramid: many unit tests, fewer integration tests, minimal E2E tests",
                "Unit tests should be fast and isolated. Integration tests verify component interactions. E2E tests cover critical user journeys.",
                RuleConstraint.Create(RuleSeverity.Info, RuleScope.Testing)
            ),
            
            // Database Rules
            EngineeringRule.Create(
                "Database Migrations",
                "All schema changes must be versioned and reversible",
                "Use migration tools for database evolution. Never modify production schemas manually. Support rollback scenarios.",
                RuleConstraint.Create(RuleSeverity.Critical, RuleScope.Database)
            ),
            
            EngineeringRule.Create(
                "Repository Pattern",
                "Isolate data access logic behind repository abstractions",
                "Repositories encapsulate query logic and provide a collection-like interface. Enables testability and technology swapping.",
                RuleConstraint.Create(RuleSeverity.Error, RuleScope.Backend)
            ),
            
            // Frontend Rules
            EngineeringRule.Create(
                "Component Composition",
                "Build UI from small, reusable, single-purpose components",
                "Components should be composable, testable, and follow single responsibility. Avoid monolithic page components.",
                RuleConstraint.Create(RuleSeverity.Warning, RuleScope.Frontend)
            ),
            
            EngineeringRule.Create(
                "Accessibility (a11y)",
                "Ensure UI is accessible to users with disabilities",
                "Follow WCAG 2.1 guidelines. Use semantic HTML, ARIA attributes, keyboard navigation, and screen reader support.",
                RuleConstraint.Create(RuleSeverity.Error, RuleScope.Frontend)
            ),
            
            // DevOps Rules
            EngineeringRule.Create(
                "Infrastructure as Code",
                "Define and manage infrastructure using code and version control",
                "Use tools like Terraform, CloudFormation, or Bicep. Infrastructure should be reproducible, testable, and auditable.",
                RuleConstraint.Create(RuleSeverity.Error, RuleScope.DevOps)
            ),
            
            EngineeringRule.Create(
                "Continuous Integration",
                "Integrate code changes frequently with automated builds and tests",
                "Commits trigger automated builds, tests, and quality checks. Catch issues early and maintain a releasable main branch.",
                RuleConstraint.Create(RuleSeverity.Critical, RuleScope.DevOps)
            )
        };

        await context.Set<EngineeringRule>().AddRangeAsync(rules);
    }

    private static Category CreateCategory(string name, string description, int displayOrder)
    {
        var category = Category.Create(name, description, isAIGenerated: false);
        category.SetDisplayOrder(displayOrder);
        return category;
    }
}
