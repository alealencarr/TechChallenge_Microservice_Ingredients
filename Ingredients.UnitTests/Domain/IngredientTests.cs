using Domain.Entities;
using FluentAssertions;

namespace Domain.UnitTests.Entities
{
    public class IngredientTests
    {

        [Fact]
        public void Deve_Criar_Ingrediente_Valido()
        {
            var nome = "Queijo Muçarela";
            var preco = 15.50m;

            var ingrediente = new Ingredient(nome, preco);

            ingrediente.Should().NotBeNull();
            ingrediente.Id.Should().NotBeEmpty(); 
            ingrediente.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1)); 
            ingrediente.Name.Should().Be(nome);
            ingrediente.Price.Should().Be(preco);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void Deve_Lancar_ArgumentNullException_Quando_Nome_Invalido(string nomeInvalido)
        {
            Action act = () => new Ingredient(nomeInvalido, 10.0m);

            act.Should().Throw<ArgumentNullException>()
               .WithMessage("Value cannot be null. (Parameter 'Para criar um ingrediente é necessário informar o nome.')");
        }

        [Fact]
        public void Deve_Lancar_ArgumentNullException_Quando_Preco_For_Zero()
        {
            var precoZero = 0m;

            Action act = () => new Ingredient("Bacon", precoZero);

            act.Should().Throw<ArgumentNullException>()
               .WithMessage("Value cannot be null. (Parameter 'Para criar um ingrediente o preço deve ser maior que zero.')");
        }


        [Fact]
        public void Deve_Reconstituir_Ingrediente_Completo()
        {
            var id = Guid.NewGuid();
            var dataCriacao = DateTime.Now.AddDays(-30); 
            var nome = "Farinha de Trigo";
            var preco = 5.90m;

            var ingrediente = new Ingredient(id, dataCriacao, nome, preco);

            ingrediente.Id.Should().Be(id);
            ingrediente.CreatedAt.Should().Be(dataCriacao);
            ingrediente.Name.Should().Be(nome);
            ingrediente.Price.Should().Be(preco);
        }

        [Fact]
        public void Deve_Instanciar_Construtor_Vazio()
        {
            var ingrediente = new Ingredient();

            ingrediente.Should().NotBeNull();
            ingrediente.Id.Should().BeEmpty();
        }
    }
}