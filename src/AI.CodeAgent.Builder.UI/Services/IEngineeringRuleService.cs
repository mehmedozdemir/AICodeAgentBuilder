using System.Collections.Generic;
using System.Threading.Tasks;
using AI.CodeAgent.Builder.Domain.Entities;

namespace AI.CodeAgent.Builder.UI.Services;

public interface IEngineeringRuleService
{
    Task<IEnumerable<EngineeringRule>> GetAllRulesAsync();
}
