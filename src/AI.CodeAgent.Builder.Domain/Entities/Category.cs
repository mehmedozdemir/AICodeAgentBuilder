using AI.CodeAgent.Builder.Domain.Common;

namespace AI.CodeAgent.Builder.Domain.Entities;

/// <summary>
/// Represents a high-level categorization of technology stacks.
/// Examples: Backend, Frontend, Database, DevOps, Security, Architecture.
/// 
/// Categories serve as organizational units and can be dynamically managed
/// by users or AI-generated content. This entity is an aggregate root.
/// </summary>
public sealed class Category : BaseEntity
{
    // Private fields to enforce encapsulation
    private string _name = string.Empty;
    private string _description = string.Empty;

    // Private constructor for EF Core
    private Category()
    {
    }

    private Category(string name, string description)
    {
        SetName(name);
        SetDescription(description);
        IsActive = true;
    }

    /// <summary>
    /// The name of the category.
    /// Must be unique within the system.
    /// </summary>
    public string Name
    {
        get => _name;
        private set => _name = value;
    }

    /// <summary>
    /// A detailed description explaining the purpose and scope of this category.
    /// </summary>
    public string Description
    {
        get => _description;
        private set => _description = value;
    }

    /// <summary>
    /// Display order for UI presentation.
    /// Lower values appear first.
    /// </summary>
    public int DisplayOrder { get; private set; }

    /// <summary>
    /// Indicates whether this category is currently active.
    /// Inactive categories are hidden from selection but preserved for historical data.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Indicates whether this category was created by AI.
    /// Used for tracking and auditing AI-generated content.
    /// </summary>
    public bool IsAIGenerated { get; private set; }

    /// <summary>
    /// Factory method to create a new category with validation.
    /// </summary>
    public static Category Create(string name, string description, bool isAIGenerated = false)
    {
        var category = new Category(name, description)
        {
            IsAIGenerated = isAIGenerated,
            DisplayOrder = 0
        };

        return category;
    }

    /// <summary>
    /// Updates the category name with validation.
    /// </summary>
    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name cannot be empty.", nameof(name));

        if (name.Length > 100)
            throw new ArgumentException("Category name cannot exceed 100 characters.", nameof(name));

        _name = name.Trim();
        SetUpdatedAt();
    }

    /// <summary>
    /// Updates the category description.
    /// </summary>
    public void SetDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Category description cannot be empty.", nameof(description));

        if (description.Length > 500)
            throw new ArgumentException("Category description cannot exceed 500 characters.", nameof(description));

        _description = description.Trim();
        SetUpdatedAt();
    }

    /// <summary>
    /// Sets the display order for UI sorting.
    /// </summary>
    public void SetDisplayOrder(int order)
    {
        if (order < 0)
            throw new ArgumentException("Display order cannot be negative.", nameof(order));

        DisplayOrder = order;
        SetUpdatedAt();
    }

    /// <summary>
    /// Activates the category, making it available for selection.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        SetUpdatedAt();
    }

    /// <summary>
    /// Deactivates the category, hiding it from selection.
    /// Historical references remain intact.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        SetUpdatedAt();
    }
}
