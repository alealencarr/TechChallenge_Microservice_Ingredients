using Application.Gateways;
using Application.Interfaces.DataSources;
using Application.UseCases.Ingredients;
using Domain.Entities;
using FluentAssertions;
using Moq;
using Shared.DTO.Ingrendient.Input;
using Xunit;

namespace Ingredients.UnitTests.Application.UseCases
{
    public class GetIngredientByIdTests
    {
        private readonly Mock<IIngredientDataSource> _dataSourceMock;
        private readonly IngredientGateway _gateway;
        private readonly GetIngredientByIdUseCase _useCase;

        public GetIngredientByIdTests()
        {
            _dataSourceMock = new Mock<IIngredientDataSource>();
            _gateway = IngredientGateway.Create(_dataSourceMock.Object);
            _useCase = GetIngredientByIdUseCase.Create(_gateway);
        }

        [Fact]
        public async Task Should_Return_Ingredient_When_Found()
        {
            var id = Guid.NewGuid();
            var expected = new IngredientInputDto(id, DateTime.Now,"Tomate", 2.0m);

            _dataSourceMock.Setup(x => x.GetById(id))
                           .ReturnsAsync(expected);

            var result = await _useCase.Run(id);

            result.Should().NotBeNull();
            result!.Name.Should().Be("Tomate");
            _dataSourceMock.Verify(x => x.GetById(id), Times.Once);
        }

        [Fact]
        public async Task Should_Return_Null_When_Not_Found()
        {
            var id = Guid.NewGuid();
            _dataSourceMock.Setup(x => x.GetById(id))
                           .ReturnsAsync((IngredientInputDto?)null);

            var result = await _useCase.Run(id);

            result.Should().BeNull();
        }

        [Fact]
        public async Task Should_Wrap_Exception()
        {
            var id = Guid.NewGuid();
            _dataSourceMock.Setup(x => x.GetById(id))
                           .ThrowsAsync(new Exception("Falha grave"));

            var ex = await Assert.ThrowsAsync<Exception>(() => _useCase.Run(id));
            ex.Message.Should().StartWith("Error:Falha grave");
        }
    }
}