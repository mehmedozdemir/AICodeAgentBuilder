using System.Threading.Tasks;
using AI.CodeAgent.Builder.Application.DTOs;

namespace AI.CodeAgent.Builder.UI.Services;

public interface IArtifactGenerationService
{
    Task<GenerationResult> GenerateArtifactsAsync(ProjectProfileDto projectProfile);
}
