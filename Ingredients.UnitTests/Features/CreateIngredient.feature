# language: pt-BR
Funcionalidade: Criar Ingrediente
    Como um gestor do cardapio
    Eu quero cadastrar novos ingredientes
    Para compor meus produtos

Cenario: Criar um ingrediente valido com sucesso
    Dado que eu tenho um ingrediente valido com nome "Bacon" e preco 5.50
    Quando eu solicito a criacao deste ingrediente
    Entao o ingrediente deve ser salvo no banco
    E o sistema deve retornar o ingrediente com ID gerado

Cenario: Tentar criar ingrediente invalido
    Dado que eu tenho um ingrediente invalido sem nome
    Quando eu solicito a criacao deste ingrediente
    Entao o sistema deve retornar um erro de argumento invalido