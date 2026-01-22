using System.Diagnostics.CodeAnalysis;

namespace Shared.DTO.Ingredient.Output
{
    [ExcludeFromCodeCoverage]

    public record IngredientOutputDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public decimal Price { get; set; }
    }


}
