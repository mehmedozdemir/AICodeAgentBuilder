# AI.CodeAgent.Builder.UI

Production-grade Avalonia UI layer for AI Code Agent Builder.

## Overview

This UI layer provides a desktop application interface for generating AI code agent configurations. It follows **strict MVVM** and **Clean Architecture** principles, with zero business logic in the UI layer.

## Architecture

### Layer Dependencies
```
UI → Application → Domain
```

### Key Principles
- ✅ **MVVM Pattern**: ViewModels coordinate, Views display, Models are in Domain
- ✅ **Clean Architecture**: UI depends only on Application services
- ✅ **Dependency Injection**: All ViewModels and services resolved via DI
- ✅ **No Business Logic**: All logic delegated to Application layer
- ✅ **Thread-Safe**: Navigation and Dialogs use proper synchronization

## Project Structure

```
AI.CodeAgent.Builder.UI/
├── Commands/
│   ├── RelayCommand.cs              # Sync command implementation
│   └── AsyncRelayCommand.cs         # Async command with IsExecuting
├── Converters/
│   ├── BoolToColorConverter.cs      # Boolean → Brush
│   ├── BoolToStatusConverter.cs     # Boolean → String
│   └── StatusToColorConverter.cs    # AIResponseStatus → Brush
├── Services/
│   ├── INavigationService.cs        # Navigation interface
│   ├── NavigationService.cs         # Stack-based navigation
│   ├── IDialogService.cs            # Dialog interface
│   └── DialogService.cs             # Avalonia dialogs
├── ViewModels/
│   ├── ViewModelBase.cs             # Base with INotifyPropertyChanged
│   ├── MainViewModel.cs             # Main shell coordinator
│   ├── HomeViewModel.cs             # Welcome screen
│   ├── CategoryManagementViewModel.cs  # Category/Stack CRUD
│   ├── HistoryViewModel.cs          # AI audit trail
│   ├── ProjectBuilderViewModel.cs   # Wizard coordinator
│   ├── ProjectMetadataViewModel.cs  # Wizard Step 1
│   ├── StackSelectionViewModel.cs   # Wizard Step 2
│   ├── StackParameterViewModel.cs   # Wizard Step 3
│   ├── ArchitectureRulesViewModel.cs # Wizard Step 4
│   └── PreviewGenerateViewModel.cs  # Wizard Step 5
├── Views/
│   ├── MainWindow.axaml             # Application shell
│   ├── HomeView.axaml               # Welcome screen
│   ├── CategoryManagementView.axaml # Category management
│   ├── ProjectBuilderView.axaml     # 5-step wizard
│   └── HistoryView.axaml            # AI history viewer
├── App.axaml                        # Application resources
└── App.axaml.cs                     # DI configuration
```

## Screen Flows

### 1. Home Screen
- Welcome message and quick actions
- Navigate to: Project Builder, Category Management, History

### 2. Category Management
- **Left Panel**: List of categories
- **Right Panel**: Tech stacks for selected category
- **Actions**: Add/Edit/Delete, AI Refresh

### 3. Project Builder Wizard (5 Steps)
1. **Project Metadata**: Name, Description, Target Platform
2. **Stack Selection**: Multi-select from categorized tech stacks
3. **Stack Parameters**: Dynamic form for stack-specific config
4. **Architecture & Rules**: Select pattern + engineering rules
5. **Preview & Generate**: Review + trigger artifact generation

### 4. History Viewer
- **Left Panel**: List of AI responses
- **Right Panel**: Full prompt, response, and metadata
- **Filter**: By status (All, Pending, Validated, Rejected)

## Services

### NavigationService
- **Pattern**: Stack-based history for wizard support
- **Features**: GoBack(), NavigateTo<T>(), NavigateToRoot<T>()
- **Thread-Safe**: Lock-based synchronization
- **DI Integration**: Resolves ViewModels from IServiceProvider

### DialogService
- **Types**: Information, Error, Warning, Confirmation, Input
- **Thread-Safe**: Uses Avalonia Dispatcher.UIThread
- **Async-First**: All methods return Task/Task<T>

## Commands

### RelayCommand
- Synchronous command execution
- Optional CanExecute predicate
- Generic and non-generic versions

### AsyncRelayCommand
- Async command execution
- **IsExecuting** property for loading states
- **Prevents concurrent execution** automatically
- Auto-raises CanExecuteChanged

## MVVM Patterns Used

### Property Change Notification
```csharp
private string _name = string.Empty;
public string Name
{
    get => _name;
    set => SetProperty(ref _name, value);
}
```

### Command Binding
```csharp
public AsyncRelayCommand SaveCommand { get; }

SaveCommand = new AsyncRelayCommand(SaveAsync, () => !IsSaving);
```

### Navigation
```csharp
_navigationService.NavigateTo<CategoryManagementViewModel>();
_navigationService.GoBack();
```

### Dialogs
```csharp
var result = await _dialogService.ShowConfirmationAsync("Delete?", "Are you sure?");
if (result)
{
    // Delete logic
}
```

## Dependency Injection

### Service Registration (App.axaml.cs)
```csharp
// UI Services
services.AddSingleton<INavigationService, NavigationService>();
services.AddSingleton<IDialogService, DialogService>();

// Shell
services.AddSingleton<MainViewModel>();

// Screen ViewModels (Transient)
services.AddTransient<HomeViewModel>();
services.AddTransient<CategoryManagementViewModel>();
services.AddTransient<ProjectBuilderViewModel>();
services.AddTransient<HistoryViewModel>();

// Wizard Steps
services.AddTransient<ProjectMetadataViewModel>();
services.AddTransient<StackSelectionViewModel>();
services.AddTransient<StackParameterViewModel>();
services.AddTransient<ArchitectureRulesViewModel>();
services.AddTransient<PreviewGenerateViewModel>();
```

## Application Service Dependencies

The UI layer requires these Application services (see `docs/APPLICATION-SERVICES-REQUIRED.cs`):

- `ICategoryService` - Category CRUD operations
- `ITechStackService` - Tech stack CRUD operations
- `IAIGenerationService` - AI-driven data generation + history
- `IArchitectureService` - Architecture pattern queries
- `IEngineeringRuleService` - Engineering rule queries
- `IArtifactGenerationService` - Artifact generation from project profile

## Building

```bash
# Restore dependencies
dotnet restore

# Build
dotnet build

# Run
dotnet run --project src/AI.CodeAgent.Builder.UI
```

## Testing

### Manual Testing Checklist
- [ ] Navigate between all screens via sidebar
- [ ] Complete wizard end-to-end
- [ ] Test validation on each wizard step
- [ ] Test AI Refresh with loading states
- [ ] Test category/stack CRUD operations
- [ ] Test confirmation dialogs
- [ ] Test input dialogs
- [ ] Test error dialogs
- [ ] View AI history and filter
- [ ] Test back button in wizard

### Unit Testing
Create unit tests for ViewModels:
- Test command execution
- Test validation logic
- Test property changes
- Mock Application services

## Styling

### Color Scheme
- **Primary**: `#2563eb` (Blue)
- **Sidebar**: `#1e293b` (Dark Slate)
- **Content**: `#f8fafc` (Light Gray)
- **Text**: `#1e293b` (Dark)
- **Success**: `Green`
- **Error**: `Red`
- **Warning**: `Orange`

### Button Styles
- `.action-button` - Primary action buttons
- `.cancel-button` - Cancel/secondary buttons
- `.danger-button` - Destructive actions
- `.wizard-button` - Wizard navigation
- `.nav-button` - Sidebar navigation

## Known Limitations

1. **No Validation Errors Display**: Currently shows dialog on validation failure, could show inline errors
2. **No Undo/Redo**: Wizard doesn't support undo/redo
3. **No Auto-Save**: Wizard state is lost if closed
4. **No Search**: Category and History views don't have search
5. **No Pagination**: Large lists may have performance issues

## Future Enhancements

- [ ] Dark mode support
- [ ] Keyboard shortcuts (Ctrl+N for new project, etc.)
- [ ] Drag-and-drop for reordering categories
- [ ] Export/Import project configurations
- [ ] Recent projects list
- [ ] Settings screen
- [ ] Localization support
- [ ] Custom themes
- [ ] Wizard state persistence
- [ ] Inline validation errors
- [ ] Search and filter everywhere
- [ ] Pagination for large lists

## Troubleshooting

### Build Errors

**Problem**: Application service interfaces not found
**Solution**: Implement the required interfaces in the Application layer (see `docs/APPLICATION-SERVICES-REQUIRED.cs`)

**Problem**: StackParameter missing properties
**Solution**: Update Domain entity to include `ParameterName`, `DisplayName`, `Description`, `IsRequired`

### Runtime Errors

**Problem**: ViewModels not resolving
**Solution**: Ensure all ViewModels are registered in DI (App.axaml.cs ConfigureServices)

**Problem**: Navigation not working
**Solution**: Ensure NavigationService is registered as singleton and MainViewModel subscribes to CurrentViewModelChanged

**Problem**: Dialogs not showing
**Solution**: Ensure DialogService uses Dispatcher.UIThread and GetMainWindow() returns valid window

## Contributing

Follow these principles when extending the UI:

1. **No Business Logic in UI**: Delegate to Application services
2. **Use MVVM Pattern**: ViewModels coordinate, Views display
3. **Async Commands**: Use AsyncRelayCommand for long operations
4. **Thread-Safe**: Use NavigationService and DialogService properly
5. **Null-Safety**: Use nullable reference types
6. **Validation**: Implement ValidateAsync() for wizard steps
7. **Loading States**: Show IsLoading/IsExecuting indicators
8. **Error Handling**: Use try-catch and show error dialogs

## License

See root LICENSE file.

## Authors

AI Code Agent Builder Team
