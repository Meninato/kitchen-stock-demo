using KitchenStock.Application.Modules.Recipe.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Recipe.MediatR.Queries;

public record GetCostAnalysisQuery(Guid RecipeId, Guid UserId) : IRequest<RecipeCostAnalysisResult>;