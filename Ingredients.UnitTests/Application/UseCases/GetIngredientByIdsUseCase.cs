using Application.Gateways;
using Application.Interfaces.DataSources;
using Application.UseCases.Ingredients;
using FluentAssertions;
using Moq;
using Shared.DTO.Ingrendient.Input;

namespace Ingredients.UnitTests.Application.UseCases
{
    public class GetIngredientByIdsTests
    {
        private readonly Mock<IIngredientDataSource> _dataSourceMock;
        private readonly IngredientGateway _gateway;
        private readonly GetIngredientByIdsUseCase _useCase;

        public GetIngredientByIdsTests()
        {
            _dataSourceMock = new Mock<IIngredientDataSource>();
            _gateway = IngredientGateway.Create(_dataSourceMock.Object);
            _useCase = GetIngredientByIdsUseCase.Create(_gateway);
        }

        [Fact]
        public async Task Should_Return_List_When_Found()
        {
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            var ids = new List<Guid> { id1, id2 };

            var ingredients = new List<IngredientInputDto>
            {
                new IngredientInputDto(id1, DateTime.Now,"Bacon", 10),
                new IngredientInputDto(id2, DateTime.Now,"Ovo", 5)
            };

            _dataSourceMock.Setup(x => x.GetByIds(ids))
                           .ReturnsAsync(ingredients);

            var result = await _useCase.Run(ids);

            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            _dataSourceMock.Verify(x => x.GetByIds(ids), Times.Once);
        }

        [Fact]
        public async Task Should_Return_Empty_When_None_Found()
        {
            var ids = new List<Guid> { Guid.NewGuid() };
            _dataSourceMock.Setup(x => x.GetByIds(ids))
                           .ReturnsAsync(new List<IngredientInputDto>());

            var result = await _useCase.Run(ids);

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Should_Wrap_Exception()
        {
            _dataSourceMock.Setup(x => x.GetByIds(It.IsAny<List<Guid>>()))
                           .ThrowsAsync(new Exception("Erro no banco"));

            var ex = await Assert.ThrowsAsync<Exception>(() => _useCase.Run(new List<Guid>()));
            ex.Message.Should().StartWith("Error:Erro no banco");
        }
    }
}