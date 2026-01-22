using Application.Controllers.Ingredients;
using Application.Interfaces.DataSources;
using Infrastructure.DataSources;
using Infrastructure.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO.Ingredient.Output;
using Shared.Result;
using System.Diagnostics.CodeAnalysis;

namespace API.Endpoints.Ingredients;
[ExcludeFromCodeCoverage]

internal sealed class GetByIds : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/ingredients/listIngredients",
           async (AppDbContext appDbContext, [FromBody] List<Guid> ids) =>
           {
               IIngredientDataSource dataSource = new IngredientDataSource(appDbContext);
               IngredientController _ingredientController = new IngredientController(dataSource);
               var ingredient = await _ingredientController.GetIngredientByIds(ids);

               return ingredient.Succeeded ? Results.Ok(ingredient) : Results.NotFound(ingredient);

           })
           .WithTags("Ingredients")
           .Produces<ICommandResult<IngredientOutputDto?>>()
           .WithName("Ingredient.GetByIds").RequireAuthorization();//.RequireAuthorization();
    }
}
