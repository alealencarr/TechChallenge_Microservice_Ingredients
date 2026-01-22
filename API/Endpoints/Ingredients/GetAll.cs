using Application.Controllers.Ingredients;
using Application.Interfaces.DataSources;
using Infrastructure.DataSources;
using Infrastructure.DbContexts;
using Shared.DTO.Ingredient.Output;
using Shared.Result;
using System.Diagnostics.CodeAnalysis;

namespace API.Endpoints.Ingredients;
[ExcludeFromCodeCoverage]

internal sealed class GetAll : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/ingredients",
           async (AppDbContext appDbContext, HttpContext httpContext) =>
           {

               IIngredientDataSource dataSource = new IngredientDataSource(appDbContext);
               IngredientController _ingredientController = new IngredientController(dataSource);
               var ingredients = await _ingredientController.GetAllIngredientsAsync();

               return ingredients.Succeeded ? Results.Ok(ingredients) : Results.BadRequest(ingredients);

           })
           .WithTags("Ingredients")
           .Produces<ICommandResult<List<IngredientOutputDto>>>()
           .WithName("Ingredient.GetAll").RequireAuthorization();
    }
}


