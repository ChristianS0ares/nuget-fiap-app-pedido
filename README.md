# NuGet FIAP - App Pedido

Este repositório contém a implementação do sistema de gestão de pedidos, parte do projeto da fase IV do curso de pós-graduação em arquitetura de software da FIAP - 3SOAT. O sistema é estruturado em várias camadas, incluindo acesso a dados, lógica de negócios, e uma API para acesso via aplicativos ou serviços externos.


## Membros:
- [Camila Lemos de Melo - RM 352359]()
- [Christian Soares Santos - RM 351509](https://www.linkedin.com/in/christian-soares-93250a170/)
- [Fábio Guimarães Martins - RM 351825](https://www.linkedin.com/in/fabio-martins-2021)
- [Josué Batista Cruz Júnior - RM 352045](https://www.linkedin.com/in/josuejuniorjf/)


## Tecnologias Utilizadas

- ASP.NET Core: Utilizado para construir a API no servidor, gerenciando as requisições e respostas HTTP.
- Swagger/OpenAPI: Utilizado para documentar a API, permitindo uma visualização interativa e design da API através do Swagger UI.
- MongoDB.Driver: Driver permite que aplicativos escritos em linguagens baseadas em .NET acessem e manipulem dados armazenados em instâncias do MongoDB de maneira eficaz e eficiente.
- MongoDB: Banco de dados baseado em documentos.
- Docker: Contêinerização para facilitar o desenvolvimento, teste e produção.
- SpecFlow: Framework para desenvolvimento dirigido por comportamento (BDD), utilizando a linguagem Gherkin para definição de testes.
- XUnit: Framework para testes unitários em .NET.

## Estrutura do Repositório
### Camadas
- nuget-fiap-app-pedido-common: Contém modelos e interfaces utilizados pelos serviços e repositórios.
- nuget-fiap-app-pedido-repository: Implementação do acesso aos dados utilizando o MongoDB.Driver.
- nuget-fiap-app-pedido-server: API que expõe endpoints para interação com o sistema de pedidos.
- nuget-fiap-app-pedido-services: Lógica de negócios, chamando os repositórios para acesso a dados.
### Testes
- nuget-fiap-app-pedido-test: Testes unitários e de integração.
- nuget-fiap-app-pedido-test-bdd: Testes de comportamento usando SpecFlow.

## Docker
O sistema pode ser executado em contêineres Docker, facilitando a configuração e implantação. O arquivo docker-compose.yml raiz configura o ambiente necessário, incluindo a aplicação e o banco de dados.
```bash
docker-compose up -d --build
```
## Testes
Testes são uma parte crucial do projeto, garantindo que todos os componentes funcionem como esperado antes da implantação. O projeto inclui:

- Testes Unitários: Testam componentes individuais para assegurar que executem suas responsabilidades isoladamente.
- Testes de Integração: Verificam a integração entre diferentes módulos e serviços.
- Testes BDD (Behavior-Driven Development): Utilizam cenários escritos em linguagem natural para garantir que o sistema atende aos requisitos de negócios.

### Execução dos Testes
#### Testes Unitários
Para executar apenas os testes unitários na camada server, utilize o seguinte comando:
```bash
dotnet test --filter Category=Unit
```
Este comando seleciona e executa todos os testes marcados com a categoria Unit.
#### Testes de Integração
Para executar os testes de integração:
```bash
dotnet test --filter Category=Integration
```
Antes de rodar os testes de integração, assegure-se de que a base de dados MongoDB esteja configurada corretamente com as variáveis de ambiente DB_PASSWORD, DB_USER, e DB_HOST. Outro requisito é disponibilidade a API de Produtos, como a API de Pedidos realiza a busca dos Produtos nessa API é necessário definir o host.
```bash
set ProdutoApi__BaseUrl=https://api-de-produtos-prod/endereco
```
#### Testes BDD
Para os testes de BDD, utilize:
```bash
dotnet test --filter Category=bdd
```
Da mesma forma que os testes de integração, é necessário configurar a base de dados e as variáveis de ambiente mencionadas anteriormente para garantir que os testes de BDD funcionem corretamente.
### Configuração de Ambiente
É importante que a base de dados esteja acessível e configurada corretamente para os testes de integração e BDD. Configure as seguintes variáveis de ambiente antes de executar os testes:

- DB_PASSWORD: Senha do usuário da base de dados.
- DB_USER: Nome do usuário da base de dados.
- DB_HOST: Host onde a base de dados está rodando.

Essas configurações garantem que os testes possam interagir corretamente com a base de dados, permitindo uma avaliação eficaz da integração e comportamento dos componentes do sistema.

Como já mencionado anteriormente a definição do Host da API de Produtos é essencial:
```bash
set ProdutoApi__BaseUrl=https://api-de-produtos-prod/endereco
```


### Relatório de Cobertura de Testes

Para visualizar o relatório de cobertura de testes, navegue até o diretório `nuget-fiap-app-pedido-test/coveragereport` e abra o arquivo `index.html`. Este relatório fornece uma visão detalhada da cobertura de testes alcançada pelos testes unitários e de integração, ajudando a identificar áreas do código que podem necessitar de mais atenção em termos de testes.


![](nuget-fiap-app-pedido-test/coverage.PNG)

### Geração de Relatório de Cobertura de Testes
Para gerar um relatório detalhado de cobertura de testes, que permite avaliar quais partes do código foram efetivamente testadas, siga os passos abaixo:
#### Executar Testes com Cobertura
Primeiro, execute os testes com a coleta de dados de cobertura ativada usando o seguinte comando:
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
```
Este comando habilita a coleta de cobertura durante a execução dos testes e gera um arquivo de cobertura no formato cobertura, um padrão XML para relatórios de cobertura.
#### Gerar Relatório de Cobertura
Após a coleta dos dados de cobertura, utilize o ReportGenerator para converter o arquivo de cobertura em um relatório HTML amigável, facilitando a análise visual dos resultados. Execute o comando abaixo:
```bash
reportgenerator -reports:"coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
```
Este comando gera um relatório HTML dentro do diretório coveragereport, utilizando os dados do arquivo coverage.cobertura.xml.
#### Visualizar o Relatório
Para visualizar o relatório de cobertura, navegue até o diretório coveragereport e abra o arquivo index.html em um navegador de sua escolha. Este arquivo apresenta uma visão detalhada da cobertura de teste, incluindo quais linhas de código foram executadas durante os testes.
#### Considerações
Certifique-se de ter o pacote coverlet.collector no seu projeto de teste para a coleta de cobertura funcionar corretamente.
O ReportGenerator deve estar instalado globalmente ou disponível como uma ferramenta em seu ambiente de desenvolvimento para que o comando funcione.


## Consumo da API

Para facilitar o consumo e teste da API, disponibilizamos uma [coleção](api-pedidos.postman_collection.json) do Postman que contém modelos pré-configurados de requisições. Abaixo estão detalhadas as rotas disponíveis, juntamente com exemplos de uso para cada operação.


### Pedidos

#### Listar Todos os Pedidos
- Método: GET
- URL: http://localhost:8082/Pedido
- Descrição: Retorna todos os pedidos cadastrados no sistema.

#### Criar Pedido
- Método: POST
- URL: http://localhost:8082/Pedido
- Body:
```bash
{
  "cliente": {
    "cpf": "123456789-10",
    "nome": "Cristiano Ronaldo",
    "email": "cr7@jogador.com"
  },
  "itens": [
    {
      "id": 1,
      "quantidade": 2
    },
    {
      "id": 2,
      "quantidade": 2
    }
  ]
}
```
- Descrição: Adiciona um novo pedido ao sistema.

#### Obter Pedido por ID
- Método: GET
- URL: http://localhost:8082/Pedido/ff9405ae-6945-4f6b-92d1-1f273c7ce9f8
- Descrição: Retorna o pedido conforme ID passado.

#### Atualizar Pedido
- Método: PUT
- URL: http://localhost:8082/Pedido/ff9405ae-6945-4f6b-92d1-1f273c7ce9f8
- Body:
```bash
{
  "cliente": {
    "cpf": "123456789-10",
    "nome": "Cristiano Ronaldo",
    "email": "cr7@jogador.com"
  },
  "itens": [
    {
      "id": 1,
      "quantidade": 2
    },
    {
      "id": 14,
      "quantidade": 2
    }
  ]
}
```
- Descrição: Atualiza os dados de um pedido existente.

#### Excluir Pedido
- Método: DELETE
- URL: http://localhost:8082/Pedido/ff9405ae-6945-4f6b-92d1-1f273c7ce9f8
- Descrição: Remove um pedido do sistema.