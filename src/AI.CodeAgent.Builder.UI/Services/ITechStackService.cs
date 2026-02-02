using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AI.CodeAgent.Builder.Domain.Entities;

namespace AI.CodeAgent.Builder.UI.Services;

public interface ITechStackService
{
    Task<IEnumerable<TechStack>> GetStacksByCategoryAsync(Guid categoryId);
    Task<TechStack?> GetStackByIdAsync(Guid stackId);
    Task<TechStack> CreateStackAsync(TechStack stack);
    Task UpdateStackAsync(TechStack stack);
    Task DeleteStackAsync(Guid stackId);
}
