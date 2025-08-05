using Xunit;
using System.Threading.Tasks;
using WebApiBiblioteca.Models;
using WebApiBiblioteca.Service.Autores;
using Microsoft.EntityFrameworkCore;
using WebApiBiblioteca.Data;
using System.Collections.Generic;
using WebApiBiblioteca.DTO.Autor;
using Moq;
using WebApiBiblioteca.Infra;
using WebApiBiblioteca.Service.RabbitMQ;

namespace WebApiBiblioteca.Tests.Services
{
    public class AutorServiceTests
    {
        // Cria um contexto de banco em memória com autores de teste
        private async Task<AppDbContext> GetDbContextComAutor()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "DbAutorTest")
                .Options;

            var context = new AppDbContext(options);

            if (!await context.Autores.AnyAsync())
            {
                context.Autores.Add(new AutorModel { Id = 1, Nome = "João", SobreNome = "Silva" });
                context.Autores.Add(new AutorModel { Id = 2, Nome = "Maria", SobreNome = "Souza" });
                await context.SaveChangesAsync();
            }

            return context;
        }

        [Fact]
        public async Task BuscarAutorPorId_DeveRetornarAutor_SeExistir()
        {
            // Arrange
            var context = await GetDbContextComAutor();
            var mockRabbit = new Mock<IRabbitMQService>();

            var service = new AutorService(context, mockRabbit.Object);

            // Act
            var resultado = await service.BuscarAutorPorId(1);

            // Assert
            Assert.NotNull(resultado.Dados);
            Assert.Equal("João", resultado.Dados?.Nome);
            Assert.True(resultado.Status);
        }


        [Fact]
        public async Task BuscarAutorPorId_DeveRetornarMensagemSeNaoEncontrado()
        {
            // Arrange
            var context = await GetDbContextComAutor();
            var mockRabbit = new Mock<IRabbitMQService>();

            var service = new AutorService(context, mockRabbit.Object);

            // Act
            var resultado = await service.BuscarAutorPorId(99);

            // Assert
            Assert.Null(resultado.Dados);
            Assert.Equal("Nenhum registro localizar", resultado.Mensagem);
            Assert.False(resultado.Status);
        }


        [Fact]
        public async Task SalvarAutor_DeveSalvarNoBanco()
        {
            // Arrange: Cria banco em memória novo e serviço
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "DbSalvarAutorTest")
                .Options;

            using var context = new AppDbContext(options);
            var mockRabbit = new Mock<IRabbitMQService>();

            var service = new AutorService(context, mockRabbit.Object);

            var novoAutor = new AutorCriacaoDTO
            {
                Nome = "Carlos",
                SobreNome = "Andrade"
            };

            // Act: Chama o método que salva o autor
            await service.CadastrarAutor(novoAutor);

            // Assert: Verifica se o autor está no banco
            var autorSalvo = await context.Autores.FirstOrDefaultAsync(a => a.Nome == "Carlos" && a.SobreNome == "Andrade");

            Assert.NotNull(autorSalvo); // o autor foi salvo
            Assert.Equal("Carlos", autorSalvo?.Nome);
            Assert.Equal("Andrade", autorSalvo?.SobreNome);
        }

        [Fact]
        public async Task EditarAutor_DeveSalvarNoBanco()
        {
            // Arrange: Cria banco em memória novo e serviço
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "DbEditarAutorTest")
                .Options;

            using var context = new AppDbContext(options);
            context.Autores.Add(new AutorModel { Id = 1, Nome = "Carlos", SobreNome = "Lima" });
           
            await context.SaveChangesAsync();
            var mockRabbit = new Mock<IRabbitMQService>();

            var service = new AutorService(context, mockRabbit.Object);

            // Act: Edita o autor (muda SobreNome para "Ferreira")
            var resultado = await service.EditarAutor(new AutorEditarDTO
            {
                Id = 1,
                Nome = "Carlos",
                SobreNome = "Ferreira"
            });

            // Assert: Verifica se foi salvo corretamente
            var autorSalvo = await context.Autores.FirstOrDefaultAsync(a => a.Id == 1);

            Assert.NotNull(autorSalvo);
            Assert.Equal("Carlos", autorSalvo?.Nome);
            Assert.Equal("Ferreira", autorSalvo?.SobreNome); // ❗ Aqui estava "Andrade", mas no teste editou para
        }
    }
}
