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
internal sealed class Create : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/ingredients",
           async (AppDbContext appDbContext, [FromBody] IngredientRequestDto ingredientDto) =>
           {
               if (!MiniValidator.TryValidate(ingredientDto, out var errors))
                   return Results.ValidationProblem(errors);

               IIngredientDataSource dataSource = new IngredientDataSource(appDbContext);
               IngredientController _ingredientController = new IngredientController(dataSource);
               var ingredient = await _ingredientController.CreateIngredient(ingredientDto);

               return  ingredient.Succeeded ? Results.Created($"/{ingredient.Data?.Id}",ingredient) : Results.BadRequest(ingredient);

           })
           .WithTags("Ingredients")
           .Produces<ICommandResult<IngredientOutputDto?>>()
           .WithName("Ingredient.Create").RequireAuthorization(); //.RequireAuthorization(new AuthorizeAttribute { Roles = "Admin,Master" }); 
    }
}
