using Application.Controllers.Ingredients;
using Application.Interfaces.DataSources;
using Infrastructure.DataSources;
using Infrastructure.DbContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniValidation;
using Shared.DTO.Ingredient.Output;
using Shared.DTO.Ingredient.Request;
using Shared.Result;
using System.Diagnostics.CodeAnalysis;

namespace API.Endpoints.Ingredients;
[ExcludeFromCodeCoverage]

internal sealed class Update : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("api/ingredients/{id}",
           async (AppDbContext appDbContext, [FromRoute] Guid id, [FromBody] IngredientRequestDto ingredientDto) =>
           {
               if (!MiniValidator.TryValidate(ingredientDto, out var errors))
                   return Results.ValidationProblem(errors);

               IIngredientDataSource dataSource = new IngredientDataSource(appDbContext);
               IngredientController _ingredientController = new IngredientController(dataSource);
               var ingredient = await _ingredientController.UpdateIngredient(ingredientDto, id);

               return ingredient.Succeeded ? Results.Ok(ingredient) : Results.BadRequest(ingredient);

           })
           .WithTags("Ingredients")
           .Produces<ICommandResult<IngredientOutputDto?>>()
           .WithName("Ingredient.Update").RequireAuthorization(); //.RequireAuthorization(new AuthorizeAttribute { Roles = "Admin,Master" }); 
    }
}
