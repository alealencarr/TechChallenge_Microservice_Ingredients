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
    public class GetAllIngredientsTests
    {
        private readonly Mock<IIngredientDataSource> _dataSourceMock;
        private readonly IngredientGateway _gateway;
        private readonly GetAllIngredientsUseCase _useCase;

        public GetAllIngredientsTests()
        {
            _dataSourceMock = new Mock<IIngredientDataSource>();

            _gateway = IngredientGateway.Create(_dataSourceMock.Object);

            _useCase = GetAllIngredientsUseCase.Create(_gateway);
        }

        [Fact]
        public async Task Should_Return_All_Ingredients_Successfully()
        {
            var ingredientsList = new List<IngredientInputDto>
            {
                new IngredientInputDto(Guid.NewGuid(),DateTime.Now,"Bacon", 5.0m),
                new IngredientInputDto(Guid.NewGuid(),DateTime.Now,"Queijo", 3.5m)
            };
            _dataSourceMock.Setup(x => x.GetAll())
                           .ReturnsAsync(ingredientsList);

            var result = await _useCase.Run();

            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result[0].Name.Should().Be("Bacon");

            _dataSourceMock.Verify(x => x.GetAll(), Times.Once);
        }

        [Fact]
        public async Task Should_Return_Empty_List_When_No_Ingredients_Found()
        {
            _dataSourceMock.Setup(x => x.GetAll())
                           .ReturnsAsync(new List<IngredientInputDto>());

            var result = await _useCase.Run();

            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Should_Throw_Exception_When_Gateway_Fails()
        {
            _dataSourceMock.Setup(x => x.GetAll())
                           .ThrowsAsync(new Exception("Falha de conexão"));

            var exception = await Assert.ThrowsAsync<Exception>(() => _useCase.Run());

            exception.Message.Should().StartWith("Error:Falha de conexão");
        }
    }
}