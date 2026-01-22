using Application.Gateways;
using Application.UseCases.Ingredients;
using Application.UseCases.Ingredients.Command;
using Application.Interfaces.DataSources; 
using Domain.Entities;
using FluentAssertions;
using Moq;
using Reqnroll;
using Shared.DTO.Ingrendient.Input;

namespace Ingredients.UnitTests.StepDefinitions
{
    [Binding]
    public class CriarIngredienteStepDefinitions
    {
        private readonly Mock<IIngredientDataSource> _dataSourceMock;
        private readonly IngredientGateway _gateway;
        private readonly CreateIngredientUseCase _useCase;

        private IngredientCommand _commandInput;
        private Ingredient _resultadoSucesso;
        private Exception _resultadoErro;

        public CriarIngredienteStepDefinitions()
        {
            _dataSourceMock = new Mock<IIngredientDataSource>();
            _gateway = IngredientGateway.Create(_dataSourceMock.Object);
            _useCase = CreateIngredientUseCase.Create(_gateway);
        }


        [Given("que eu tenho um ingrediente valido com nome {string} e preco {decimal}")]
        public void GivenIngredienteValido(string nome, decimal preco)
        {
            _commandInput = new IngredientCommand { Name = nome, Price = preco };
        }

        [Given("que eu tenho um ingrediente invalido sem nome")]
        public void GivenIngredienteInvalido()
        {
            _commandInput = new IngredientCommand { Name = "", Price = 10 };
        }


        [When("eu solicito a criacao deste ingrediente")]
        public async Task WhenSolicitoCriacao()
        {
            try
            {
                _resultadoSucesso = await _useCase.Run(_commandInput);
            }
            catch (Exception ex)
            {
                _resultadoErro = ex;
            }
        }


        [Then("o ingrediente deve ser salvo no banco")]
        public void ThenDeveSalvarNoBanco()
        {
            _dataSourceMock.Verify(x => x.Create(It.IsAny<IngredientInputDto>()), Times.Once);
        }

        [Then("o sistema deve retornar o ingrediente com ID gerado")]
        public void ThenDeveRetornarComId()
        {
            _resultadoSucesso.Should().NotBeNull();
            _resultadoSucesso.Id.Should().NotBeEmpty();
            _resultadoSucesso.Name.Should().Be(_commandInput.Name);
        }

        [Then("o sistema deve retornar um erro de argumento invalido")]
        public void ThenDeveRetornarErroArgumento()
        {
            _resultadoErro.Should().NotBeNull();
            _resultadoErro.Should().BeOfType<ArgumentNullException>();
        }
    }
}