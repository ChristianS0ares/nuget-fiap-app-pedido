@bdd
Funcionalidade: Gerenciamento de Pedidos de Lanchonete
    Como um usuário da API
    Eu quero gerenciar pedidos de uma lanchonete

@bdd
Cenário: Obter todos os pedidos
    Dado que eu criei um pedido com o cliente "John Doe"
    Quando eu solicito a lista de pedidos
    Então eu devo receber uma lista contendo o pedido do cliente "John Doe"

@bdd
Cenário: Adicionar um novo pedido
    Dado que eu tenho itens disponíveis para pedido
    Quando eu adiciono um pedido com os itens "1" e "3"
    Então o pedido deve ser adicionado com sucesso e contendo os itens "1" e "3"

@bdd
Cenário: Obter pedido por ID
    Dado que eu criei um pedido com o cliente "Alice Bob"
    Quando eu solicito o pedido pelo seu ID
    Então eu devo receber o pedido do cliente "Alice Bob"

@bdd
Cenário: Atualizar um pedido existente
    Dado que eu criei um pedido com o cliente "Carlos Silva"
    Quando eu atualizo o pedido para incluir o item "7" e excluir o item "1"
    E eu solicito o pedido pelo seu ID
    Então eu devo receber o pedido atualizado com o item "7"

@bdd
Cenário: Excluir um pedido
    Dado que eu criei um pedido com o cliente "Elena Ruiz"
    Quando eu excluo o pedido do cliente "Elena Ruiz"
    Então o pedido do cliente "Elena Ruiz" não deve mais existir

@bdd
Cenário: Tentativa de atualizar um pedido inexistente
    Quando eu tento atualizar um pedido com o ID inexistente "12345"
    Então eu devo receber uma mensagem de erro informando que o pedido não existe

@bdd
Cenário: Tentativa de excluir um pedido inexistente
    Quando eu tento excluir um pedido com o ID inexistente "54321"
    Então eu devo receber uma mensagem de erro informando que o pedido não existe