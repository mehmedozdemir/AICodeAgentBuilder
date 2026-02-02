using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AI.CodeAgent.Builder.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AIResponses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Prompt = table.Column<string>(type: "TEXT", maxLength: 10000, nullable: false),
                    RawResponse = table.Column<string>(type: "TEXT", maxLength: 50000, nullable: false),
                    ProcessedContent = table.Column<string>(type: "TEXT", nullable: true),
                    RequestContext = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ValidatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ValidatedBy = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    ValidationErrors = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    AIModel = table.Column<string>(type: "TEXT", nullable: true),
                    TokenCount = table.Column<int>(type: "INTEGER", nullable: true),
                    ResponseTimeMs = table.Column<int>(type: "INTEGER", nullable: true),
                    Metadata = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AIResponses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArchitecturePatterns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Guidelines = table.Column<string>(type: "TEXT", nullable: false),
                    ComplexityLevel = table.Column<int>(type: "INTEGER", nullable: false),
                    SuitableForSmallTeams = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    SuitableForLargeScale = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    KeyPrinciples = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    AntiPatterns = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsAIGenerated = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchitecturePatterns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    IsAIGenerated = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EngineeringRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Rationale = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    Severity = table.Column<string>(type: "TEXT", nullable: false),
                    Scope = table.Column<string>(type: "TEXT", nullable: false),
                    ImplementationGuidance = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: true),
                    CommonViolations = table.Column<string>(type: "TEXT", nullable: true),
                    ExampleCode = table.Column<string>(type: "TEXT", maxLength: 10000, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsEnforced = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    IsAIGenerated = table.Column<bool>(type: "INTEGER", nullable: false),
                    Tags = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EngineeringRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    ProjectName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    TargetTeamSize = table.Column<int>(type: "INTEGER", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    ArchitecturePatternIds = table.Column<string>(type: "TEXT", nullable: false),
                    EngineeringRuleIds = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TechStacks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CategoryId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    DefaultVersion = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    DocumentationUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsAIGenerated = table.Column<bool>(type: "INTEGER", nullable: false),
                    Tags = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechStacks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProfileTechStacks",
                columns: table => new
                {
                    TechStackId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProjectProfileId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ParameterValues = table.Column<string>(type: "TEXT", nullable: false),
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileTechStacks", x => new { x.ProjectProfileId, x.TechStackId });
                    table.ForeignKey(
                        name: "FK_ProfileTechStacks_ProjectProfiles_ProjectProfileId",
                        column: x => x.ProjectProfileId,
                        principalTable: "ProjectProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StackParameters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    ParameterType = table.Column<string>(type: "TEXT", nullable: false),
                    DefaultValue = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IsRequired = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    AllowedValues = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    TechStackId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StackParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StackParameters_TechStacks_TechStackId",
                        column: x => x.TechStackId,
                        principalTable: "TechStacks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AIResponses_CreatedAt",
                table: "AIResponses",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AIResponses_RequestContext",
                table: "AIResponses",
                column: "RequestContext");

            migrationBuilder.CreateIndex(
                name: "IX_AIResponses_Status",
                table: "AIResponses",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ArchitecturePatterns_ComplexityLevel",
                table: "ArchitecturePatterns",
                column: "ComplexityLevel");

            migrationBuilder.CreateIndex(
                name: "IX_ArchitecturePatterns_Name",
                table: "ArchitecturePatterns",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_DisplayOrder",
                table: "Categories",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_IsActive",
                table: "Categories",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EngineeringRules_IsEnforced",
                table: "EngineeringRules",
                column: "IsEnforced");

            migrationBuilder.CreateIndex(
                name: "IX_EngineeringRules_Name",
                table: "EngineeringRules",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectProfiles_ProjectName",
                table: "ProjectProfiles",
                column: "ProjectName");

            migrationBuilder.CreateIndex(
                name: "IX_StackParameters_TechStackId",
                table: "StackParameters",
                column: "TechStackId");

            migrationBuilder.CreateIndex(
                name: "IX_StackParameters_TechStackId_Name",
                table: "StackParameters",
                columns: new[] { "TechStackId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TechStacks_CategoryId",
                table: "TechStacks",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TechStacks_CategoryId_Name",
                table: "TechStacks",
                columns: new[] { "CategoryId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AIResponses");

            migrationBuilder.DropTable(
                name: "ArchitecturePatterns");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "EngineeringRules");

            migrationBuilder.DropTable(
                name: "ProfileTechStacks");

            migrationBuilder.DropTable(
                name: "StackParameters");

            migrationBuilder.DropTable(
                name: "ProjectProfiles");

            migrationBuilder.DropTable(
                name: "TechStacks");
        }
    }
}
