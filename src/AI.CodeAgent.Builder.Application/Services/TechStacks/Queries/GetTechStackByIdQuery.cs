namespace AI.CodeAgent.Builder.Application.Services.TechStacks.Queries;

/// <summary>
/// Query to retrieve a tech stack by ID with all parameters.
/// </summary>
public sealed class GetTechStackByIdQuery
{
    public Guid TechStackId { get; }

    private GetTechStackByIdQuery(Guid techStackId)
    {
        TechStackId = techStackId;
    }

    public static GetTechStackByIdQuery Create(Guid techStackId)
    {
        if (techStackId == Guid.Empty)
            throw new ArgumentException("Tech stack ID is required.", nameof(techStackId));

        return new GetTechStackByIdQuery(techStackId);
    }
}
