using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace AI.CodeAgent.Builder.UI.ViewModels;

/// <summary>
/// Represents a dynamic parameter field for a tech stack.
/// </summary>
public class ParameterField : ViewModelBase
{
    private string _value = string.Empty;

    public ParameterField(string parameterName, string displayName, string? description, bool isRequired)
    {
        ParameterName = parameterName;
        DisplayName = displayName;
        Description = description;
        IsRequired = isRequired;
    }

    public string ParameterName { get; }
    public string DisplayName { get; }
    public string? Description { get; }
    public bool IsRequired { get; }

    public string Value
    {
        get => _value;
        set => SetProperty(ref _value, value);
    }
}

/// <summary>
/// Represents a tech stack with its parameter fields.
/// </summary>
public class StackParameterGroup : ViewModelBase
{
    public StackParameterGroup(string stackName)
    {
        StackName = stackName;
        Parameters = new ObservableCollection<ParameterField>();
    }

    public string StackName { get; }
    public ObservableCollection<ParameterField> Parameters { get; }
}

/// <summary>
/// Step 3: Stack Parameters (Dynamic Form Generation)
/// </summary>
public sealed class StackParameterViewModel : WizardStepViewModel
{
    public StackParameterViewModel()
    {
        StackGroups = new ObservableCollection<StackParameterGroup>();
    }

    public override string StepTitle => "Configure Stack Parameters";
    public override string StepDescription => "Provide specific configuration for each selected technology stack";

    public ObservableCollection<StackParameterGroup> StackGroups { get; }

    public void LoadParameters(Domain.Entities.TechStack[] selectedStacks)
    {
        StackGroups.Clear();

        foreach (var stack in selectedStacks)
        {
            var group = new StackParameterGroup(stack.Name);

            // Load parameters from stack
            foreach (var param in stack.Parameters)
            {
                // Use Name property for both ParameterName and DisplayName
                // TODO: Update StackParameter entity to have DisplayName and Description properties
                group.Parameters.Add(new ParameterField(
                    param.Name,
                    param.Name, // Using Name as DisplayName for now
                    null, // Description not available yet
                    false)); // IsRequired not available yet
            }

            if (group.Parameters.Count > 0)
            {
                StackGroups.Add(group);
            }
        }
    }

    public override Task<bool> ValidateAsync()
    {
        // Validate all required fields are filled
        foreach (var group in StackGroups)
        {
            foreach (var param in group.Parameters)
            {
                if (param.IsRequired && string.IsNullOrWhiteSpace(param.Value))
                {
                    return Task.FromResult(false);
                }
            }
        }

        return Task.FromResult(true);
    }
}
