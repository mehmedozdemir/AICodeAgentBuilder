using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using AI.CodeAgent.Builder.Application.DTOs;
using AI.CodeAgent.Builder.Application.Services;
using AI.CodeAgent.Builder.UI.Commands;
using AI.CodeAgent.Builder.UI.Services;

namespace AI.CodeAgent.Builder.UI.ViewModels;

/// <summary>
/// Base class for wizard step ViewModels.
/// </summary>
public abstract class WizardStepViewModel : ViewModelBase
{
    public int StepNumber { get; set; }
    public abstract string StepTitle { get; }
    public abstract string StepDescription { get; }
    public abstract Task<bool> ValidateAsync();
}

/// <summary>
/// Main coordinator ViewModel for the 5-step Project Builder wizard.
/// </summary>
public sealed class ProjectBuilderViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly IDialogService _dialogService;
    private int _currentStepIndex;
    private WizardStepViewModel? _currentStep;

    public ProjectBuilderViewModel(
        ProjectMetadataViewModel metadataViewModel,
        StackSelectionViewModel stackSelectionViewModel,
        StackParameterViewModel stackParameterViewModel,
        ArchitectureRulesViewModel architectureRulesViewModel,
        PreviewGenerateViewModel previewGenerateViewModel,
        INavigationService navigationService,
        IDialogService dialogService)
    {
        _navigationService = navigationService;
        _dialogService = dialogService;

        // Initialize wizard steps
        Steps = new ObservableCollection<WizardStepViewModel>
        {
            metadataViewModel,
            stackSelectionViewModel,
            stackParameterViewModel,
            architectureRulesViewModel,
            previewGenerateViewModel
        };
        // Set step numbers
        for (int i = 0; i < Steps.Count; i++)
        {
            Steps[i].StepNumber = i + 1;
        }
        NextCommand = new AsyncRelayCommand(NextAsync, () => CanGoNext);
        BackCommand = new RelayCommand(Back, () => CanGoBack);
        CancelCommand = new AsyncRelayCommand(CancelAsync);

        // Start at first step
        CurrentStepIndex = 0;
    }

    public ObservableCollection<WizardStepViewModel> Steps { get; }

    public int CurrentStepIndex
    {
        get => _currentStepIndex;
        private set
        {
            if (SetProperty(ref _currentStepIndex, value))
            {
                CurrentStep = Steps[value];
                OnPropertyChanged(nameof(CanGoNext));
                OnPropertyChanged(nameof(CanGoBack));
                OnPropertyChanged(nameof(IsLastStep));
                OnPropertyChanged(nameof(NextButtonText));
                NextCommand.RaiseCanExecuteChanged();
                BackCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public WizardStepViewModel? CurrentStep
    {
        get => _currentStep;
        private set => SetProperty(ref _currentStep, value);
    }

    public bool CanGoNext => CurrentStepIndex < Steps.Count - 1 || IsLastStep;
    public bool CanGoBack => CurrentStepIndex > 0;
    public bool IsLastStep => CurrentStepIndex == Steps.Count - 1;
    public string NextButtonText => IsLastStep ? "Generate" : "Next";

    public AsyncRelayCommand NextCommand { get; }
    public RelayCommand BackCommand { get; }
    public AsyncRelayCommand CancelCommand { get; }

    private async Task NextAsync()
    {
        if (CurrentStep == null)
            return;

        // Validate current step
        var isValid = await CurrentStep.ValidateAsync();
        if (!isValid)
        {
            await _dialogService.ShowWarningAsync(
                "Validation Error",
                "Please correct the errors on this step before continuing.");
            return;
        }

        // If last step, trigger generation
        if (IsLastStep)
        {
            if (CurrentStep is PreviewGenerateViewModel previewVm)
            {
                await previewVm.GenerateAsync();
            }
            return;
        }

        // Move to next step
        CurrentStepIndex++;
    }

    private void Back()
    {
        if (CanGoBack)
        {
            CurrentStepIndex--;
        }
    }

    private async Task CancelAsync()
    {
        var confirm = await _dialogService.ShowConfirmationAsync(
            "Cancel Project",
            "Are you sure you want to cancel? All progress will be lost.");

        if (confirm)
        {
            _navigationService.NavigateTo<HomeViewModel>();
        }
    }
}
