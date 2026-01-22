using Application.Interfaces.DataSources;
using Infrastructure.DbContexts;
using Infrastructure.DbModels;
using Microsoft.EntityFrameworkCore;
using Shared.DTO.Ingrendient.Input;
using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.DataSources
{
    [ExcludeFromCodeCoverage]

    public class IngredientDataSource : IIngredientDataSource
    {
        private readonly AppDbContext _appDbContext;

        public IngredientDataSource(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task Update(IngredientInputDto ingredient)
        {
            var ingredientDb = await _appDbContext.Ingredient.Where(x => x.Id == ingredient.Id).FirstOrDefaultAsync() ?? throw new Exception("Ingredient not find by Id.");
            ingredientDb.Name = ingredient.Name;
            ingredientDb.Price = ingredient.Price;

            _appDbContext.Update(ingredientDb);
            await _appDbContext.SaveChangesAsync();
        }
        public async Task Create(IngredientInputDto ingredient)
        {
            var ingredientDbModel = new IngredientDbModel(ingredient.Id, ingredient.Name, ingredient.Price, ingredient.CreatedAt);

            await _appDbContext.AddAsync(ingredientDbModel);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task<IngredientInputDto?> GetById(Guid id)
        {

            var ingredient = await _appDbContext.Ingredient.AsNoTracking().Where(x => x.Id == id).FirstOrDefaultAsync();

            return ingredient is not null ? new IngredientInputDto(ingredient.Id, ingredient.CreatedAt, ingredient.Name, ingredient.Price) : null;
        }

        public async Task<List<IngredientInputDto>> GetAll()
        {
            var ingredients = await _appDbContext.Ingredient.AsNoTracking().ToListAsync();

            return ingredients.Select(x => new IngredientInputDto(x.Id, x.CreatedAt, x.Name, x.Price)).ToList();
        }

        public async Task<List<IngredientInputDto>> GetByIds(List<Guid> ids)
        {
            var ingredients = await _appDbContext.Set<IngredientDbModel>()
               .AsNoTracking()
               .Where(p => ids.Contains(p.Id))
               .ToListAsync();

            return ingredients.Select(x => new IngredientInputDto(x.Id, x.CreatedAt, x.Name, x.Price)).ToList();
        }
    }
}