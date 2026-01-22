using Application.Gateways;
using Application.Interfaces.DataSources;
using Application.Presenter.Ingredients;
using Application.UseCases.Ingredients;
using Application.UseCases.Ingredients.Command;
using Shared.DTO.Ingredient.Output;
using Shared.DTO.Ingredient.Request;
using Shared.Result;

namespace Application.Controllers.Ingredients
{
    public class IngredientController
    {
        private IIngredientDataSource _dataSource;
        public IngredientController(IIngredientDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public async Task<ICommandResult<IngredientOutputDto>> CreateIngredient(IngredientRequestDto ingredientRequestDto)
        {
            IngredientPresenter ingredientPresenter = new("Ingrediente cadastrado!");

            try
            {
                var command = new IngredientCommand( ingredientRequestDto.Name, ingredientRequestDto.Price);

                var ingredientGateway = IngredientGateway.Create(_dataSource);
                var useCaseCreate = CreateIngredientUseCase.Create(ingredientGateway);
                var ingredientEntity = await useCaseCreate.Run(command);

                var dtoRetorno = ingredientPresenter.TransformObject(ingredientEntity);

                return dtoRetorno;
            }
            catch (Exception ex)
            {
                return ingredientPresenter.Error<IngredientOutputDto>(ex.Message);
            }
        }

        public async Task<ICommandResult<IngredientOutputDto>> UpdateIngredient(IngredientRequestDto ingredientRequestDto, Guid id)
        {
            IngredientPresenter ingredientPresenter = new("Ingrediente alterado!");

            try
            {
                var command = new IngredientCommand(id, ingredientRequestDto.Name, ingredientRequestDto.Price);

                var ingredientGateway = IngredientGateway.Create(_dataSource);
                var useCaseCreate = UpdateIngredientUseCase.Create(ingredientGateway);
                var ingredientEntity = await useCaseCreate.Run(command);

                var dtoRetorno = ingredientPresenter.TransformObject(ingredientEntity);

                return dtoRetorno;
            }
            catch (Exception ex)
            {
                return ingredientPresenter.Error<IngredientOutputDto>(ex.Message);
            }
        }

        public async Task<ICommandResult<IngredientOutputDto?>> GetIngredientById(Guid id)
        {
            IngredientPresenter ingredientPresenter = new("Ingrediente encontrado!");

            try
            {
                var ingredientGateway = IngredientGateway.Create(_dataSource);
                var useCase = GetIngredientByIdUseCase.Create(ingredientGateway);
                var ingredient = await useCase.Run(id);

                return ingredient is null ? ingredientPresenter.Error<IngredientOutputDto?>("Ingredient not found.") : ingredientPresenter.TransformObject(ingredient);
            }
            catch (Exception ex)
            {
                return ingredientPresenter.Error<IngredientOutputDto?>(ex.Message);
            }
        }

        public async Task<ICommandResult<List<IngredientOutputDto>>> GetAllIngredientsAsync()
        {
            IngredientPresenter ingredientPresenter = new("Ingredientes encontrados!");

            try
            {
                var ingredientGateway = IngredientGateway.Create(_dataSource);
                var useCase = GetAllIngredientsUseCase.Create(ingredientGateway);
                var ingredients = await useCase.Run();

                return ingredients is null ? ingredientPresenter.Error<List<IngredientOutputDto>>("Ingredients not found.") : ingredientPresenter.TransformList(ingredients);
            }
            catch (Exception ex)
            {
                return ingredientPresenter.Error<List<IngredientOutputDto>>(ex.Message);
            }
        }

 

        public async Task<ICommandResult<List<IngredientOutputDto>?>> GetIngredientByIds(List<Guid> ids)
        {
            IngredientPresenter ingredientPresenter = new("Ingredientes encontrado!");

            try
            {
                var ingredientGateway = IngredientGateway.Create(_dataSource);
                var useCase = GetIngredientByIdsUseCase.Create(ingredientGateway);
                var ingredient = await useCase.Run(ids);

                return ingredient is null ? ingredientPresenter.Error<List<IngredientOutputDto>?>("Ingredients not found.") : ingredientPresenter.TransformList(ingredient);
            }
            catch (Exception ex)
            {
                return ingredientPresenter.Error<List<IngredientOutputDto>?>(ex.Message);
            }
        }
    }
}
