# Serviço Backend Simples

Um projeto backend feito com o intuito de exibir um exemplo simples de como que é feito um serviço API CRUD feito em ASP.NET Core com controllers, possuindo todas as funcionalidades básicas.

A função geral do serviço é o gerenciamento e armazenamento de usuários (Users) em um banco de dados. No serviço você pode criar, remover, modificar e listar (podendo até escolher a função) os usuários presentes no banco de dados. Além disso, você também poderá fazer Login, assim podendo acessar Actions restritas (a maioria).

## Como buildar
Primeiramente você precisará do Docker devidamente instaldo e configurado em sua máquina. Após isso, baixe a <a href="#">imagem</a> disponível na página release, e rode o seguinte comando: 
<br>
> `docker run --rm -p 8080:5050 container`

## Como Utilizar o Serviço
Primeiro você cria um User novo em "user/create", logo em seguida você faz login em "user/login" para poder receber a sua Jwt Token. Com o acesso à Token, você poderá usufruir das funcionalidades oferecidas pelas Actions que estão restritas ao Admin.

---

### Funcionalidades Presentes:
 - Criar User;
 - Adicionar User ao banco de dados;
 - Modificar User;
 - Deletar User;
 - Fazer Login;
 - Listar todos os usuários registrados, ou especificados: Admin, Customer.

### Libraries e Frameworks utilizadas:
 - EF Core (Entity Framework Core);
 - SQLite;
 - Jwt Bearer;
 - Swagger.

### Ferramentas Utilizadas:
 - Git;
 - Docker;
 - .NET 9.0;
 - ASP.NET Core (com controllers);
 - VSCode.
