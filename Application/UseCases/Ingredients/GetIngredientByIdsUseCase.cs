using Application.Gateways;
using Domain.Entities;

namespace Application.UseCases.Ingredients
{
    public class GetIngredientByIdsUseCase
    {
        IngredientGateway _gateway = null;
        public static GetIngredientByIdsUseCase Create(IngredientGateway gateway)
        {
            return new GetIngredientByIdsUseCase(gateway);
        }

        private GetIngredientByIdsUseCase(IngredientGateway gateway)
        {
            _gateway = gateway;
        }

        public async Task<List<Ingredient>?> Run(List<Guid> ids)
        {
            try
            {
                var ingredient   = await _gateway.GetByIds(ids);

                return ingredient;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error:{ex.Message}");
            }
        }
    }
}
