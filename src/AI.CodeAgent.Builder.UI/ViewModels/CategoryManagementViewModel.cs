using System.Collections.ObjectModel;
using System.Threading.Tasks;
using AI.CodeAgent.Builder.Application.Services;
using AI.CodeAgent.Builder.Domain.Entities;
using AI.CodeAgent.Builder.UI.Commands;
using AI.CodeAgent.Builder.UI.Services;

namespace AI.CodeAgent.Builder.UI.ViewModels;

/// <summary>
/// ViewModel for Category and Tech Stack management screen.
/// Allows viewing, adding, editing, deleting categories and stacks.
/// Supports AI-based refresh of data.
/// </summary>
public sealed class CategoryManagementViewModel : ViewModelBase
{
    private readonly ICategoryService _categoryService;
    private readonly ITechStackService _techStackService;
    private readonly IAIGenerationService _aiGenerationService;
    private readonly IDialogService _dialogService;

    private Category? _selectedCategory;
    private TechStack? _selectedTechStack;
    private bool _isLoading;
    private bool _isRefreshing;

    public CategoryManagementViewModel(
        ICategoryService categoryService,
        ITechStackService techStackService,
        IAIGenerationService aiGenerationService,
        IDialogService dialogService)
    {
        _categoryService = categoryService;
        _techStackService = techStackService;
        _aiGenerationService = aiGenerationService;
        _dialogService = dialogService;

        Categories = new ObservableCollection<Category>();
        TechStacks = new ObservableCollection<TechStack>();

        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
        AddCategoryCommand = new AsyncRelayCommand(AddCategoryAsync);
        EditCategoryCommand = new AsyncRelayCommand(EditCategoryAsync, () => SelectedCategory != null);
        DeleteCategoryCommand = new AsyncRelayCommand(DeleteCategoryAsync, () => SelectedCategory != null);
        AddTechStackCommand = new AsyncRelayCommand(AddTechStackAsync, () => SelectedCategory != null);
        EditTechStackCommand = new AsyncRelayCommand(EditTechStackAsync, () => SelectedTechStack != null);
        DeleteTechStackCommand = new AsyncRelayCommand(DeleteTechStackAsync, () => SelectedTechStack != null);
        AIRefreshCommand = new AsyncRelayCommand(AIRefreshAsync);

        _ = LoadDataAsync();
    }

    public ObservableCollection<Category> Categories { get; }
    public ObservableCollection<TechStack> TechStacks { get; }

    public Category? SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            if (SetProperty(ref _selectedCategory, value))
            {
                Task.Run(async () => await LoadTechStacksForCategoryAsync()).ConfigureAwait(false);
                EditCategoryCommand.RaiseCanExecuteChanged();
                DeleteCategoryCommand.RaiseCanExecuteChanged();
                AddTechStackCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public TechStack? SelectedTechStack
    {
        get => _selectedTechStack;
        set
        {
            if (SetProperty(ref _selectedTechStack, value))
            {
                EditTechStackCommand.RaiseCanExecuteChanged();
                DeleteTechStackCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        private set => SetProperty(ref _isLoading, value);
    }

    public bool IsRefreshing
    {
        get => _isRefreshing;
        private set => SetProperty(ref _isRefreshing, value);
    }

    public AsyncRelayCommand LoadDataCommand { get; }
    public AsyncRelayCommand AddCategoryCommand { get; }
    public AsyncRelayCommand EditCategoryCommand { get; }
    public AsyncRelayCommand DeleteCategoryCommand { get; }
    public AsyncRelayCommand AddTechStackCommand { get; }
    public AsyncRelayCommand EditTechStackCommand { get; }
    public AsyncRelayCommand DeleteTechStackCommand { get; }
    public AsyncRelayCommand AIRefreshCommand { get; }

    private async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            Categories.Clear();
            foreach (var category in categories)
            {
                Categories.Add(category);
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadTechStacksForCategoryAsync()
    {
        if (SelectedCategory == null)
        {
            TechStacks.Clear();
            return;
        }

        var stacks = await _techStackService.GetStacksByCategoryAsync(SelectedCategory.Id);
        TechStacks.Clear();
        foreach (var stack in stacks)
        {
            TechStacks.Add(stack);
        }
    }

    private async Task AddCategoryAsync()
    {
        var name = await _dialogService.ShowInputDialogAsync(
            "Add Category",
            "Enter category name:",
            "New Category");

        if (string.IsNullOrWhiteSpace(name))
            return;

        var description = await _dialogService.ShowInputDialogAsync(
            "Add Category",
            "Enter category description (optional):",
            "");

        // TODO: Use factory method or constructor to create Category
        // var category = Category.Create(name, description ?? string.Empty, true, Categories.Count + 1);
        // await _categoryService.CreateCategoryAsync(category);
        
        await _dialogService.ShowInformationAsync("Not Implemented", "Category creation needs proper factory method in Domain layer.");
        await LoadDataAsync();
    }

    private async Task EditCategoryAsync()
    {
        if (SelectedCategory == null)
            return;

        var name = await _dialogService.ShowInputDialogAsync(
            "Edit Category",
            "Enter new name:",
            SelectedCategory.Name);

        if (string.IsNullOrWhiteSpace(name))
            return;

        var description = await _dialogService.ShowInputDialogAsync(
            "Edit Category",
            "Enter new description (optional):",
            SelectedCategory.Description);

        // TODO: Domain entities should have Update methods or be mutable
        // SelectedCategory.UpdateDetails(name, description ?? string.Empty);
        // await _categoryService.UpdateCategoryAsync(SelectedCategory);
        
        await _dialogService.ShowInformationAsync("Not Implemented", "Category editing needs proper update method in Domain layer.");
        await LoadDataAsync();
    }

    private async Task DeleteCategoryAsync()
    {
        if (SelectedCategory == null)
            return;

        var confirm = await _dialogService.ShowConfirmationAsync(
            "Delete Category",
            $"Are you sure you want to delete category '{SelectedCategory.Name}'? This will also delete all associated tech stacks.");

        if (!confirm)
            return;

        await _categoryService.DeleteCategoryAsync(SelectedCategory.Id);
        await LoadDataAsync();
        await _dialogService.ShowInformationAsync("Success", "Category deleted successfully.");
    }

    private async Task AddTechStackAsync()
    {
        if (SelectedCategory == null)
            return;

        var name = await _dialogService.ShowInputDialogAsync(
            "Add Tech Stack",
            "Enter tech stack name:",
            "New Stack");

        if (string.IsNullOrWhiteSpace(name))
            return;

        var description = await _dialogService.ShowInputDialogAsync(
            "Add Tech Stack",
            "Enter tech stack description (optional):",
            "");

        // TODO: Use factory method or constructor to create TechStack
        // var stack = TechStack.Create(name, description ?? string.Empty, SelectedCategory.Id, true);
        // await _techStackService.CreateStackAsync(stack);
        
        await _dialogService.ShowInformationAsync("Not Implemented", "Tech stack creation needs proper factory method in Domain layer.");
        await LoadTechStacksForCategoryAsync();
    }

    private async Task EditTechStackAsync()
    {
        if (SelectedTechStack == null)
            return;

        var name = await _dialogService.ShowInputDialogAsync(
            "Edit Tech Stack",
            "Enter new name:",
            SelectedTechStack.Name);

        if (string.IsNullOrWhiteSpace(name))
            return;

        var description = await _dialogService.ShowInputDialogAsync(
            "Edit Tech Stack",
            "Enter new description (optional):",
            SelectedTechStack.Description);

        // TODO: Domain entities should have Update methods or be mutable
        // SelectedTechStack.UpdateDetails(name, description ?? string.Empty);
        // await _techStackService.UpdateStackAsync(SelectedTechStack);
        
        await _dialogService.ShowInformationAsync("Not Implemented", "Tech stack editing needs proper update method in Domain layer.");
        await LoadTechStacksForCategoryAsync();
    }

    private async Task DeleteTechStackAsync()
    {
        if (SelectedTechStack == null)
            return;

        var confirm = await _dialogService.ShowConfirmationAsync(
            "Delete Tech Stack",
            $"Are you sure you want to delete tech stack '{SelectedTechStack.Name}'?");

        if (!confirm)
            return;

        await _techStackService.DeleteStackAsync(SelectedTechStack.Id);
        await LoadTechStacksForCategoryAsync();
        await _dialogService.ShowInformationAsync("Success", "Tech stack deleted successfully.");
    }

    private async Task AIRefreshAsync()
    {
        var confirm = await _dialogService.ShowConfirmationAsync(
            "AI Refresh",
            "This will use AI to research and populate widely-used technology categories and stacks. This may take a few minutes and consume AI tokens. Continue?");

        if (!confirm)
            return;

        IsRefreshing = true;
        try
        {
            await _aiGenerationService.RefreshCategoriesAndStacksAsync();
            await LoadDataAsync();
            await _dialogService.ShowInformationAsync("Success", "AI refresh completed successfully. Categories and tech stacks have been updated.");
        }
        catch (System.Exception ex)
        {
            await _dialogService.ShowErrorAsync("AI Refresh Failed", $"An error occurred during AI refresh: {ex.Message}");
        }
        finally
        {
            IsRefreshing = false;
        }
    }
}
