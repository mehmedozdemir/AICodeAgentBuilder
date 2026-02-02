using AI.CodeAgent.Builder.UI.Commands;
using AI.CodeAgent.Builder.UI.Services;

namespace AI.CodeAgent.Builder.UI.ViewModels;

/// <summary>
/// ViewModel for the Home/Welcome screen.
/// Provides quick navigation to main workflows.
/// </summary>
public sealed class HomeViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    public HomeViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;

        NewProjectCommand = new RelayCommand(OnNewProject);
        ManageCategoriesCommand = new RelayCommand(OnManageCategories);
        ViewHistoryCommand = new RelayCommand(OnViewHistory);
    }

    public RelayCommand NewProjectCommand { get; }
    public RelayCommand ManageCategoriesCommand { get; }
    public RelayCommand ViewHistoryCommand { get; }

    private void OnNewProject()
    {
        _navigationService.NavigateTo<ProjectBuilderViewModel>();
    }

    private void OnManageCategories()
    {
        _navigationService.NavigateTo<CategoryManagementViewModel>();
    }

    private void OnViewHistory()
    {
        _navigationService.NavigateTo<HistoryViewModel>();
    }
}
