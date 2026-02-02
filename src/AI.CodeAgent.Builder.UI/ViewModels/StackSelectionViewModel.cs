using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AI.CodeAgent.Builder.UI.Services;
using AI.CodeAgent.Builder.Domain.Entities;

namespace AI.CodeAgent.Builder.UI.ViewModels;

/// <summary>
/// Represents a selectable tech stack in the tree.
/// </summary>
public class SelectableTechStack : ViewModelBase
{
    private bool _isSelected;

    public SelectableTechStack(TechStack stack)
    {
        Stack = stack;
    }

    public TechStack Stack { get; }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}

/// <summary>
/// Represents a category with selectable tech stacks.
/// </summary>
public class SelectableCategory : ViewModelBase
{
    public SelectableCategory(Category category)
    {
        Category = category;
        TechStacks = new ObservableCollection<SelectableTechStack>();
    }

    public Category Category { get; }
    public ObservableCollection<SelectableTechStack> TechStacks { get; }
}

/// <summary>
/// Step 2: Tech Stack Selection (Category Tree with Multi-Select)
/// </summary>
public sealed class StackSelectionViewModel : WizardStepViewModel
{
    private readonly ICategoryService _categoryService;
    private readonly ITechStackService _techStackService;
    private bool _isLoading;

    public StackSelectionViewModel(
        ICategoryService categoryService,
        ITechStackService techStackService)
    {
        _categoryService = categoryService;
        _techStackService = techStackService;

        Categories = new ObservableCollection<SelectableCategory>();

        _ = LoadDataAsync();
    }

    public override string StepTitle => "Select Technology Stacks";
    public override string StepDescription => "Choose the technology stacks for your project";

    public ObservableCollection<SelectableCategory> Categories { get; }

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
            var categories = await _categoryService.GetAllCategoriesAsync();
            Categories.Clear();

            foreach (var category in categories.Where(c => c.IsActive))
            {
                var selectableCategory = new SelectableCategory(category);
                var stacks = await _techStackService.GetStacksByCategoryAsync(category.Id);

                foreach (var stack in stacks.Where(s => s.IsActive))
                {
                    selectableCategory.TechStacks.Add(new SelectableTechStack(stack));
                }

                Categories.Add(selectableCategory);
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    public override Task<bool> ValidateAsync()
    {
        // At least one tech stack must be selected
        var hasSelection = Categories
            .SelectMany(c => c.TechStacks)
            .Any(s => s.IsSelected);

        return Task.FromResult(hasSelection);
    }

    public TechStack[] GetSelectedStacks()
    {
        return Categories
            .SelectMany(c => c.TechStacks)
            .Where(s => s.IsSelected)
            .Select(s => s.Stack)
            .ToArray();
    }
}
