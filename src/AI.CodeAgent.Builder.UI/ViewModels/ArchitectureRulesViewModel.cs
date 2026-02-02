using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AI.CodeAgent.Builder.UI.Services;
using AI.CodeAgent.Builder.Domain.Entities;

namespace AI.CodeAgent.Builder.UI.ViewModels;

/// <summary>
/// Represents a selectable engineering rule.
/// </summary>
public class SelectableRule : ViewModelBase
{
    private bool _isSelected;

    public SelectableRule(EngineeringRule rule)
    {
        Rule = rule;
    }

    public EngineeringRule Rule { get; }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}

/// <summary>
/// Step 4: Architecture & Engineering Rules Selection
/// </summary>
public sealed class ArchitectureRulesViewModel : WizardStepViewModel
{
    private readonly IArchitectureService _architectureService;
    private readonly IEngineeringRuleService _ruleService;
    private string _selectedArchitecture = string.Empty;
    private bool _isLoading;

    public ArchitectureRulesViewModel(
        IArchitectureService architectureService,
        IEngineeringRuleService ruleService)
    {
        _architectureService = architectureService;
        _ruleService = ruleService;

        AvailableArchitectures = new ObservableCollection<string>();
        AvailableRules = new ObservableCollection<SelectableRule>();

        _ = LoadDataAsync();
    }

    public override string StepTitle => "Architecture & Engineering Rules";
    public override string StepDescription => "Select architecture pattern and engineering principles";

    public ObservableCollection<string> AvailableArchitectures { get; }
    public ObservableCollection<SelectableRule> AvailableRules { get; }

    public string SelectedArchitecture
    {
        get => _selectedArchitecture;
        set => SetProperty(ref _selectedArchitecture, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        private set => SetProperty(ref _isLoading, value);
    }

    private async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            // Load architecture patterns
            var architectures = await _architectureService.GetAllPatternsAsync();
            AvailableArchitectures.Clear();
            foreach (var arch in architectures)
            {
                if (arch != null && !string.IsNullOrWhiteSpace(arch.Name))
                {
                    AvailableArchitectures.Add(arch.Name);
                }
            }

            // Load engineering rules
            var rules = await _ruleService.GetAllRulesAsync();
            AvailableRules.Clear();
            foreach (var rule in rules)
            {
                if (rule != null)
                {
                    AvailableRules.Add(new SelectableRule(rule));
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading data: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    public override Task<bool> ValidateAsync()
    {
        // Architecture must be selected
        if (string.IsNullOrWhiteSpace(SelectedArchitecture))
        {
            return Task.FromResult(false);
        }

        // At least one rule should be selected (optional validation)
        return Task.FromResult(true);
    }

    public EngineeringRule[] GetSelectedRules()
    {
        return AvailableRules
            .Where(r => r.IsSelected)
            .Select(r => r.Rule)
            .ToArray();
    }
}
