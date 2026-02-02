namespace AI.CodeAgent.Builder.Application.Services.ProjectProfiles.Commands;

/// <summary>
/// Command to add a tech stack to a project profile with parameter values.
/// </summary>
public sealed class AddTechStackToProfileCommand
{
    public Guid ProfileId { get; }
    public Guid TechStackId { get; }
    public Dictionary<string, string> ParameterValues { get; }

    private AddTechStackToProfileCommand(
        Guid profileId,
        Guid techStackId,
        Dictionary<string, string> parameterValues)
    {
        ProfileId = profileId;
        TechStackId = techStackId;
        ParameterValues = parameterValues;
    }

    public static AddTechStackToProfileCommand Create(
        Guid profileId,
        Guid techStackId,
        Dictionary<string, string>? parameterValues = null)
    {
        if (profileId == Guid.Empty)
            throw new ArgumentException("Profile ID is required.", nameof(profileId));

        if (techStackId == Guid.Empty)
            throw new ArgumentException("Tech stack ID is required.", nameof(techStackId));

        return new AddTechStackToProfileCommand(
            profileId,
            techStackId,
            parameterValues ?? new Dictionary<string, string>());
    }
}
