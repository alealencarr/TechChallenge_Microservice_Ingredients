using System.Diagnostics.CodeAnalysis;

namespace Shared.DTO.Ingrendient.Input;
[ExcludeFromCodeCoverage]

public record IngredientInputDto(Guid Id, DateTime CreatedAt, string Name, decimal Price);

 