using Application.Controllers.Ingredients;
using Application.Interfaces.DataSources;
using Infrastructure.DataSources;
using Infrastructure.DbContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO.Ingredient.Output;
using Shared.Result;
using System.Diagnostics.CodeAnalysis;

namespace API.Endpoints.Ingredients;
[ExcludeFromCodeCoverage]

internal sealed class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/ingredients/{id}",
           async (AppDbContext appDbContext, [FromRoute] Guid id) =>
           {
               IIngredientDataSource dataSource = new IngredientDataSource(appDbContext);
               IngredientController _ingredientController = new IngredientController(dataSource);
               var ingredient = await _ingredientController.GetIngredientById(id);

               return ingredient.Succeeded ? Results.Ok(ingredient) : Results.NotFound(ingredient);

           })
           .WithTags("Ingredients")
           .Produces<ICommandResult<IngredientOutputDto?>>()
           .WithName("Ingredient.GetById").RequireAuthorization();//.RequireAuthorization();
    }
}
