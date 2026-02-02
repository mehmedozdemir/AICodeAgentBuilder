# Copilot AI Code Agent – Project Instructions

## Project Name
AI.CodeAgent.Builder

## Project Purpose
This is a production-grade desktop application that allows software engineers
to generate AI Code Agent infrastructure (copilot-instructions, AI config files,
policies, templates) by selecting categorized technology stacks, architectures,
engineering principles, and rules from a UI.

The application itself is NOT a code generator for business apps.
It is a meta-tool that generates:
- AI instructions
- AI configuration files
- Engineering governance artifacts

## Target Users
- Senior software engineers
- Tech leads
- Architecture teams
- Platform teams

## Technology Constraints
- Language: C# (.NET 8 or higher)
- UI: Avalonia UI
- Pattern: MVVM (strict)
- Architecture: Clean Architecture
- Persistence: SQLite
- Template engine: Scriban

## Architectural Rules (MANDATORY)
1. Follow Clean Architecture strictly
   - Domain layer must not reference UI or Infrastructure
   - UI layer must contain no business logic
2. Apply SOLID principles
3. Prefer composition over inheritance
4. No static mutable state
5. No hardcoded business data

## Domain Rules
- Categories, TechStacks, Architectures, and EngineeringRules are data-driven
- Nothing is hardcoded in code that should be configurable
- AI-generated data must be validated before persistence
- All AI outputs must be persisted for traceability

## AI Responsibilities
AI is responsible for:
- Generating categories
- Researching and listing widely used tech stacks
- Defining parameters for tech stacks
- Producing structured outputs (JSON / YAML)

AI is NOT allowed to:
- Invent fictional technologies
- Use demo-level assumptions
- Skip validation or error handling

## UI Rules
- MVVM only
- No code-behind business logic
- Views bind only to ViewModels
- Navigation handled via services

## Persistence Rules
- SQLite database
- Schema must support:
  - Categories
  - TechStacks
  - StackParameters
  - ArchitecturePatterns
  - EngineeringRules
  - ProjectProfiles
  - AIResponses (audit)

## When Requirements Are Ambiguous
- Choose the most common enterprise-standard approach
- Document the assumption in comments

## Code Quality
- Readable, explicit code
- Avoid magic values
- All public methods must be meaningful and intentional

This project is long-term and production-focused.
Do not generate prototype or demo code.
