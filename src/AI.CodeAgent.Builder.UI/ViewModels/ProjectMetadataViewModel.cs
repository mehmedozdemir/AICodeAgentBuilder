using System;
using System.Threading.Tasks;
using AI.CodeAgent.Builder.Application.DTOs;

namespace AI.CodeAgent.Builder.UI.ViewModels;

/// <summary>
/// Step 1: Project Metadata (Name, Description, Platform)
/// </summary>
public sealed class ProjectMetadataViewModel : WizardStepViewModel
{
    private string _projectName = string.Empty;
    private string _description = string.Empty;
    private string _targetPlatform = string.Empty;

    public ProjectMetadataViewModel()
    {
    }

    public override string StepTitle => "Project Information";
    public override string StepDescription => "Enter basic information about your project";

    public string ProjectName
    {
        get => _projectName;
        set => SetProperty(ref _projectName, value);
    }

    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public string TargetPlatform
    {
        get => _targetPlatform;
        set => SetProperty(ref _targetPlatform, value);
    }

    public override Task<bool> ValidateAsync()
    {
        if (string.IsNullOrWhiteSpace(ProjectName))
        {
            return Task.FromResult(false);
        }

        if (string.IsNullOrWhiteSpace(TargetPlatform))
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(true);
    }

    public ProjectMetadataDto ToDto()
    {
        return new ProjectMetadataDto
        {
            ProjectName = ProjectName,
            Description = Description,
            TargetPlatform = TargetPlatform
        };
    }
}
