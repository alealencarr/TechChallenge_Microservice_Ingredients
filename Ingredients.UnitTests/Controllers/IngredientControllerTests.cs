using Application.Controllers.Ingredients;
using Application.Interfaces.DataSources;
using Domain.Entities;
using FluentAssertions;
using Moq;
using Shared.DTO.Ingredient.Request;
using Shared.DTO.Ingrendient.Input;
using Xunit;

namespace Ingredients.UnitTests.Application.Controllers
{
    public class IngredientControllerTests
    {
        private readonly Mock<IIngredientDataSource> _dataSourceMock;
        private readonly IngredientController _controller;

        public IngredientControllerTests()
        {
            _dataSourceMock = new Mock<IIngredientDataSource>();
            _controller = new IngredientController(_dataSourceMock.Object);
        }


        [Fact]
        public async Task CreateIngredient_Should_Return_Success_When_Valid()
        {
            var request = new IngredientRequestDto { Name = "Bacon", Price = 5.50m };

            var result = await _controller.CreateIngredient(request);

            result.Succeeded.Should().BeTrue();
            result.Messages.Should().Contain("Ingrediente cadastrado!");
            result.Data.Name.Should().Be("Bacon");
            result.Data.Price.Should().Be(5.50m);

            _dataSourceMock.Verify(x => x.Create(It.IsAny<IngredientInputDto>()), Times.Once);
        }

        [Fact]
        public async Task CreateIngredient_Should_Handle_Exception()
        {
            var request = new IngredientRequestDto { Name = "Erro", Price = 0 };
            _dataSourceMock.Setup(x => x.Create(It.IsAny<IngredientInputDto>()))
                           .ThrowsAsync(new Exception("Para criar um ingrediente o preço deve ser maior que zero."));

            var result = await _controller.CreateIngredient(request);

            result.Succeeded.Should().BeFalse();
            result.Messages.Should().Contain("Value cannot be null. (Parameter 'Para criar um ingrediente o preço deve ser maior que zero.')");
        }


        [Fact]
        public async Task UpdateIngredient_Should_Return_Success_When_Found()
        {
            var id = Guid.NewGuid();
            var request = new IngredientRequestDto { Name = "Bacon Editado", Price = 6.00m };

            var existing = new IngredientInputDto(id, DateTime.Now,"Bacon Antigo", 5.0m);
 
            _dataSourceMock.Setup(x => x.GetById(id)).ReturnsAsync(existing);

            var result = await _controller.UpdateIngredient(request, id);

            result.Succeeded.Should().BeTrue();
            result.Messages.Should().Contain("Ingrediente alterado!");
            result.Data.Name.Should().Be("Bacon Editado");

            _dataSourceMock.Verify(x => x.Update(It.IsAny<IngredientInputDto>()), Times.Once);
        }

        [Fact]
        public async Task UpdateIngredient_Should_Return_Error_When_Not_Found()
        {
            var id = Guid.NewGuid();
            var request = new IngredientRequestDto { Name = "X", Price = 1 };

            _dataSourceMock.Setup(x => x.GetById(id)).ReturnsAsync((IngredientInputDto?)null);

            var result = await _controller.UpdateIngredient(request, id);

            result.Succeeded.Should().BeFalse();

            result.Messages.Should().Contain(m => m.Contains("Ingredient not find") || m.Contains("Error"));
        }

 
        [Fact]
        public async Task GetIngredientById_Should_Return_Success_When_Found()
        {
            var id = Guid.NewGuid();
            var entity = new IngredientInputDto(id, DateTime.Now, "Alface", 1.0m);

            _dataSourceMock.Setup(x => x.GetById(id)).ReturnsAsync(entity);

            var result = await _controller.GetIngredientById(id);

            result.Succeeded.Should().BeTrue();
            result.Data.Name.Should().Be("Alface");
        }

        [Fact]
        public async Task GetIngredientById_Should_Return_Error_When_Not_Found()
        {
            var id = Guid.NewGuid();
            _dataSourceMock.Setup(x => x.GetById(id)).ReturnsAsync((IngredientInputDto?)null);

            var result = await _controller.GetIngredientById(id);

            result.Succeeded.Should().BeFalse();
            result.Messages.Should().Contain("Ingredient not found.");
        }


        [Fact]
        public async Task GetAllIngredients_Should_Return_List()
        {
            var list = new List<IngredientInputDto> { new IngredientInputDto(Guid.NewGuid(),DateTime.Now,"A", 1), new IngredientInputDto(Guid.NewGuid(), DateTime.Now, "B", 2) };

            _dataSourceMock.Setup(x => x.GetAll()).ReturnsAsync(list);

            var result = await _controller.GetAllIngredientsAsync();

            result.Succeeded.Should().BeTrue();
            result.Data.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllIngredients_Should_Handle_Exception()
        {
            _dataSourceMock.Setup(x => x.GetAll()).ThrowsAsync(new Exception("Falha"));

            var result = await _controller.GetAllIngredientsAsync();

            result.Succeeded.Should().BeFalse();
            result.Messages.Should().Contain("Error:Falha");
        }

 
        [Fact]
        public async Task GetIngredientByIds_Should_Return_List()
        {
            var ids = new List<Guid> { Guid.NewGuid() };
            var list = new List<IngredientInputDto> { new IngredientInputDto(Guid.NewGuid(), DateTime.Now, "A", 1) };

            _dataSourceMock.Setup(x => x.GetByIds(ids)).ReturnsAsync(list);

            var result = await _controller.GetIngredientByIds(ids);

            result.Succeeded.Should().BeTrue();
            result.Data.Should().HaveCount(1);
        }

 
    }
}