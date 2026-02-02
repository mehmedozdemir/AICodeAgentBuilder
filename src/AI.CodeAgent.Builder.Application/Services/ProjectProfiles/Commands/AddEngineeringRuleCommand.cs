namespace AI.CodeAgent.Builder.Application.Services.ProjectProfiles.Commands;

/// <summary>
/// Command to add an engineering rule to a project profile.
/// </summary>
public sealed class AddEngineeringRuleCommand
{
    public Guid ProfileId { get; }
    public Guid RuleId { get; }

    private AddEngineeringRuleCommand(Guid profileId, Guid ruleId)
    {
        ProfileId = profileId;
        RuleId = ruleId;
    }

    public static AddEngineeringRuleCommand Create(Guid profileId, Guid ruleId)
    {
        if (profileId == Guid.Empty)
            throw new ArgumentException("Profile ID is required.", nameof(profileId));

        if (ruleId == Guid.Empty)
            throw new ArgumentException("Rule ID is required.", nameof(ruleId));

        return new AddEngineeringRuleCommand(profileId, ruleId);
    }
}
