using Application.Gateways;
using Application.Interfaces.DataSources;
using Application.UseCases.Ingredients;
using Application.UseCases.Ingredients.Command;
using Domain.Entities;
using FluentAssertions;
using Moq;
using Shared.DTO.Ingrendient.Input;
using Xunit;

namespace Ingredients.UnitTests.Application.UseCases
{
    public class UpdateIngredientTests
    {
        private readonly Mock<IIngredientDataSource> _dataSourceMock;
        private readonly IngredientGateway _gateway;
        private readonly UpdateIngredientUseCase _useCase;

        public UpdateIngredientTests()
        {
            _dataSourceMock = new Mock<IIngredientDataSource>();
            _gateway = IngredientGateway.Create(_dataSourceMock.Object);
            _useCase = UpdateIngredientUseCase.Create(_gateway);
        }

        [Fact]
        public async Task Should_Update_Successfully_When_Valid()
        {
            var id = Guid.NewGuid();
            var command = new IngredientCommand { Id = id, Name = "Nome Novo", Price = 99.9m };

            var existing = new IngredientInputDto(id, DateTime.Now,"Nome Antigo", 10m);

            _dataSourceMock.Setup(x => x.GetById(id)).ReturnsAsync(existing);

            var result = await _useCase.Run(command);

            result.Name.Should().Be("Nome Novo");
            result.Price.Should().Be(99.9m);

            _dataSourceMock.Verify(x => x.Update(It.IsAny<IngredientInputDto>()), Times.Once);
        }

        [Fact]
        public async Task Should_Throw_Exception_When_Not_Found()
        {
            var command = new IngredientCommand { Id = Guid.NewGuid(), Name = "X", Price = 1 };

            _dataSourceMock.Setup(x => x.GetById(command.Id)).ReturnsAsync((IngredientInputDto?)null);

            var ex = await Assert.ThrowsAsync<Exception>(() => _useCase.Run(command));
            ex.Message.Should().Contain("Ingredient not find by Id");

            _dataSourceMock.Verify(x => x.Update(It.IsAny<IngredientInputDto>()), Times.Never);
        }

        [Fact]
        public async Task Should_Rethrow_ArgumentException_On_Domain_Validation()
        {
            var id = Guid.NewGuid();

            var command = new IngredientCommand { Id = id, Name = "", Price = 10 };

            var existing = new IngredientInputDto(id, DateTime.Now, "Valido", 10);
            _dataSourceMock.Setup(x => x.GetById(id)).ReturnsAsync(existing);

            await Assert.ThrowsAsync<Exception>(() => _useCase.Run(command));
        }

        [Fact]
        public async Task Should_Wrap_Generic_Exception()
        {
            var command = new IngredientCommand { Id = Guid.NewGuid(), Name = "Ok", Price = 1 };
            _dataSourceMock.Setup(x => x.GetById(It.IsAny<Guid>()))
                           .ThrowsAsync(new Exception("Banco caiu"));

            var ex = await Assert.ThrowsAsync<Exception>(() => _useCase.Run(command));
            ex.Message.Should().StartWith("Error:Banco caiu");
        }
    }
}
