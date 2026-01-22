using Application.Gateways;
using Application.UseCases.Ingredients.Command;
using Domain.Entities;

namespace Application.UseCases.Ingredients
{

    public class UpdateIngredientUseCase
    {
        IngredientGateway _gateway = null;
        public static UpdateIngredientUseCase Create(IngredientGateway gateway)
        {
            return new UpdateIngredientUseCase(gateway);
        }

        private UpdateIngredientUseCase(IngredientGateway gateway)
        {
            _gateway = gateway;
        }

        public async Task<Ingredient> Run(IngredientCommand ingredient)
        {
            try
            {
                var ingredientExists = await _gateway.GetById(ingredient.Id);

                if (ingredientExists is null)
                    throw new Exception($"Error: Ingredient not find by Id.");

                ingredientExists.Price = ingredient.Price;

                if(string.IsNullOrEmpty(ingredient.Name))
                    throw new Exception($"Error: Favor informar o Name.");

                ingredientExists.Name = ingredient.Name;

                await _gateway.UpdateIngredient(ingredientExists);

                return (ingredientExists);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error:{ex.Message}");
            }
        }
    }
}
