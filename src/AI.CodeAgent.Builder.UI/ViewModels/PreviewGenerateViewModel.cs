using System;
using System.Linq;
using System.Threading.Tasks;
using AI.CodeAgent.Builder.Application.DTOs;
using AI.CodeAgent.Builder.Application.Services;
using AI.CodeAgent.Builder.UI.Commands;
using AI.CodeAgent.Builder.UI.Services;

namespace AI.CodeAgent.Builder.UI.ViewModels;

/// <summary>
/// Step 5: Preview & Generate
/// Shows read-only preview of configuration and triggers generation.
/// </summary>
public sealed class PreviewGenerateViewModel : WizardStepViewModel
{
    private readonly IArtifactGenerationService _generationService;
    private readonly IDialogService _dialogService;
    private readonly ProjectMetadataViewModel _metadataVm;
    private readonly StackSelectionViewModel _stackSelectionVm;
    private readonly StackParameterViewModel _stackParameterVm;
    private readonly ArchitectureRulesViewModel _architectureRulesVm;

    private string _previewText = string.Empty;
    private bool _isGenerating;

    public PreviewGenerateViewModel(
        IArtifactGenerationService generationService,
        IDialogService dialogService,
        ProjectMetadataViewModel metadataVm,
        StackSelectionViewModel stackSelectionVm,
        StackParameterViewModel stackParameterVm,
        ArchitectureRulesViewModel architectureRulesVm)
    {
        _generationService = generationService;
        _dialogService = dialogService;
        _metadataVm = metadataVm;
        _stackSelectionVm = stackSelectionVm;
        _stackParameterVm = stackParameterVm;
        _architectureRulesVm = architectureRulesVm;

        GenerateCommand = new AsyncRelayCommand(GenerateAsync, () => !IsGenerating);

        BuildPreview();
    }

    public override string StepTitle => "Review & Generate";
    public override string StepDescription => "Review your selections and generate AI configuration artifacts";

    public string PreviewText
    {
        get => _previewText;
        private set => SetProperty(ref _previewText, value);
    }

    public bool IsGenerating
    {
        get => _isGenerating;
        private set
        {
            if (SetProperty(ref _isGenerating, value))
            {
                GenerateCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public AsyncRelayCommand GenerateCommand { get; }

    private void BuildPreview()
    {
        var metadata = _metadataVm.ToDto();
        var selectedStacks = _stackSelectionVm.GetSelectedStacks();
        var selectedRules = _architectureRulesVm.GetSelectedRules();

        var preview = $@"PROJECT CONFIGURATION SUMMARY
═══════════════════════════════════════

Project Name: {metadata.ProjectName}
Description: {metadata.Description}
Target Platform: {metadata.TargetPlatform}

Architecture Pattern: {_architectureRulesVm.SelectedArchitecture}

Selected Technology Stacks ({selectedStacks.Length}):
{string.Join("\n", selectedStacks.Select(s => $"  • {s.Name}"))}

Selected Engineering Rules ({selectedRules.Length}):
{string.Join("\n", selectedRules.Select(r => $"  • {r.Name}"))}

───────────────────────────────────────
Click 'Generate' to create:
  ✓ copilot-instructions.md
  ✓ aiagent.config.yaml
  ✓ AI policy documents
  ✓ Template configurations
";

        PreviewText = preview;
    }

    public async Task GenerateAsync()
    {
        IsGenerating = true;
        try
        {
            var metadata = _metadataVm.ToDto();
            var selectedStacks = _stackSelectionVm.GetSelectedStacks();
            var selectedRules = _architectureRulesVm.GetSelectedRules();

            var projectDto = new ProjectProfileDto
            {
                ProjectName = metadata.ProjectName,
                Description = metadata.Description,
                TargetPlatform = metadata.TargetPlatform,
                ArchitecturePattern = _architectureRulesVm.SelectedArchitecture,
                SelectedStackIds = selectedStacks.Select(s => s.Id).ToArray(),
                SelectedRuleIds = selectedRules.Select(r => r.Id).ToArray()
            };

            var result = await _generationService.GenerateArtifactsAsync(projectDto);

            await _dialogService.ShowInformationAsync(
                "Generation Complete",
                $"Successfully generated {result.GeneratedFiles.Count} files:\n\n" +
                string.Join("\n", result.GeneratedFiles.Select(f => $"  • {f}")));
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync(
                "Generation Failed",
                $"An error occurred during artifact generation:\n\n{ex.Message}");
        }
        finally
        {
            IsGenerating = false;
        }
    }

    public override Task<bool> ValidateAsync()
    {
        // Final validation - all previous steps should be valid
        return Task.FromResult(true);
    }
}
