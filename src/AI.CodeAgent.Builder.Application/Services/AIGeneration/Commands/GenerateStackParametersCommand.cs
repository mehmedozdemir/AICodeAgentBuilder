namespace AI.CodeAgent.Builder.Application.Services.AIGeneration.Commands;

/// <summary>
/// Command to generate parameters for a tech stack from AI.
/// </summary>
public sealed class GenerateStackParametersCommand
{
    public Guid TechStackId { get; }

    private GenerateStackParametersCommand(Guid techStackId)
    {
        TechStackId = techStackId;
    }

    public static GenerateStackParametersCommand Create(Guid techStackId)
    {
        if (techStackId == Guid.Empty)
            throw new ArgumentException("Tech stack ID is required.", nameof(techStackId));

        return new GenerateStackParametersCommand(techStackId);
    }
}
