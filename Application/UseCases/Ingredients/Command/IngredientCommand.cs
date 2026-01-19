namespace Application.UseCases.Ingredients.Command;

public class IngredientCommand
{
    public IngredientCommand(Guid id, string name, decimal price)
    {
        Name = name;
        Price = price;
        Id = id;
    }

    public IngredientCommand(string name, decimal price)
    {
        Name = name;
        Price = price;
    }
    public IngredientCommand() { }

    public string Name { get; set; }
    public decimal Price { get; set; }
    public Guid Id { get; set; }
}