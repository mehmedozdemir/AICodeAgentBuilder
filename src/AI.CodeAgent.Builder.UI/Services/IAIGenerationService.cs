using System.Collections.Generic;
using System.Threading.Tasks;
using AI.CodeAgent.Builder.Domain.Entities;

namespace AI.CodeAgent.Builder.UI.Services;

public interface IAIGenerationService
{
    Task RefreshCategoriesAndStacksAsync();
    Task<IEnumerable<AIResponse>> GetAIHistoryAsync();
}
