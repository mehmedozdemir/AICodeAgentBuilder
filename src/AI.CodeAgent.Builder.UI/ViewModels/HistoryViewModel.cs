using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AI.CodeAgent.Builder.UI.Services;
using AI.CodeAgent.Builder.Domain.Entities;
using AI.CodeAgent.Builder.Domain.Enums;
using AI.CodeAgent.Builder.UI.Commands;

namespace AI.CodeAgent.Builder.UI.ViewModels;

/// <summary>
/// ViewModel for AI History/Audit view.
/// Displays AI generation history with filtering and details.
/// </summary>
public sealed class HistoryViewModel : ViewModelBase
{
    private readonly IAIGenerationService _aiGenerationService;
    private AIResponse? _selectedResponse;
    private AIResponseStatus? _statusFilter = null;
    private bool _isLoading;

    public HistoryViewModel(IAIGenerationService aiGenerationService)
    {
        _aiGenerationService = aiGenerationService;

        Responses = new ObservableCollection<AIResponse>();

        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
        ApplyFilterCommand = new AsyncRelayCommand(ApplyFilterAsync);

        _ = LoadDataAsync();
    }

    public ObservableCollection<AIResponse> Responses { get; }

    public AIResponse? SelectedResponse
    {
        get => _selectedResponse;
        set => SetProperty(ref _selectedResponse, value);
    }

    public AIResponseStatus? StatusFilter
    {
        get => _statusFilter;
        set
        {
            if (SetProperty(ref _statusFilter, value))
            {
                _ = ApplyFilterAsync();
            }
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        private set => SetProperty(ref _isLoading, value);
    }

    public AsyncRelayCommand LoadDataCommand { get; }
    public AsyncRelayCommand ApplyFilterCommand { get; }

    private async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            var responses = await _aiGenerationService.GetAIHistoryAsync();
            Responses.Clear();
            foreach (var response in responses.OrderByDescending(r => r.CreatedAt))
            {
                Responses.Add(response);
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task ApplyFilterAsync()
    {
        IsLoading = true;
        try
        {
            var responses = await _aiGenerationService.GetAIHistoryAsync();
            
            if (StatusFilter.HasValue)
            {
                responses = responses.Where(r => r.Status == StatusFilter.Value).ToList();
            }

            Responses.Clear();
            foreach (var response in responses.OrderByDescending(r => r.CreatedAt))
            {
                Responses.Add(response);
            }
        }
        finally
        {
            IsLoading = false;
        }
    }
}
