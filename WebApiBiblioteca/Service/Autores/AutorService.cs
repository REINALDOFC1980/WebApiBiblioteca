using Microsoft.EntityFrameworkCore;
using WebApiBiblioteca.Data;
using WebApiBiblioteca.DTO.Autor;
using WebApiBiblioteca.Infra;
using WebApiBiblioteca.Models;
using WebApiBiblioteca.Service.RabbitMQ;


namespace WebApiBiblioteca.Service.Autores
{
    public class AutorService : IAutorService
    {
        private readonly AppDbContext _context;
        private readonly IRabbitMQService _rabbit;


        public AutorService(AppDbContext context, IRabbitMQService rabbit)
        {
            _context = context;
            _rabbit = rabbit;
        }



        public async Task<ResponseModel<AutorModel>> BuscarAutorPorId(int IdAutor)
        {
            ResponseModel<AutorModel> resposta = new ResponseModel<AutorModel>();

            try
            {
                var autor = await _context.Autores.FirstOrDefaultAsync(linha => linha.Id == IdAutor);

                if(autor == null)
                {
                    resposta.Mensagem = "Nenhum registro localizar";
                    resposta.Status = false;
                    return resposta;

                }

                resposta.Dados = autor;
                resposta.Mensagem = "Autor Localizado";
               

                return resposta;

            }
            catch (Exception ex)
            {

                resposta.Mensagem = ex.Message;
                resposta.Status = false;

                return resposta;
            }
        }

        public async Task<ResponseModel<AutorModel>> BuscarAutorPorIdLivro(int IdLivro)
        {
            ResponseModel<AutorModel> resposta = new ResponseModel<AutorModel>();

            try
            {
                var livro = await _context.Livros.Include(a => a.Autor).FirstOrDefaultAsync(livro => livro.Id == IdLivro);
                if (livro == null)
                {
                    resposta.Mensagem = "Nenhum registro localizar";
                    resposta.Status = false;
                    return resposta;
                }

                resposta.Dados = livro.Autor;
                resposta.Mensagem = "Autor Localizado";

                return resposta;
            }
            catch (Exception ex)
            {

                resposta.Mensagem = ex.Message;
                resposta.Status = false;

                return resposta;
            }
        }

        public async Task CadastrarAutor(AutorCriacaoDTO autorCriacaoDTO)
        {
            try
            {
                var autor = new AutorModel()
                {
                    Nome = autorCriacaoDTO.Nome,
                    SobreNome = autorCriacaoDTO.SobreNome
                };

                _context.Add(autor);
                await _context.SaveChangesAsync();

                _rabbit.PublicarMensagem(new
                {
                    Evento = "AutorCadastrado",
                    autor.Id,
                    autor.Nome,
                    autor.SobreNome
                }, "fila.inserir");

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<ResponseModel<AutorModel>> EditarAutor(AutorEditarDTO autorEditarDTO)
        {
            ResponseModel<AutorModel> resposta = new ResponseModel<AutorModel>();
          
            try
            {
                var autor = await _context.Autores.FirstOrDefaultAsync(linha => linha.Id == autorEditarDTO.Id);
                if (autor == null)
                {
                    resposta.Mensagem = "Nenhum registro localizar";
                    resposta.Status = false;
                    return resposta;
                }

                autor.Nome = autorEditarDTO.Nome;
                autor.SobreNome = autorEditarDTO.SobreNome;

                _context.Update(autor);
                await _context.SaveChangesAsync();

            

                _rabbit.PublicarMensagem(new
                {
                    Evento = "EditarCadastro",
                    autor.Id,
                    autor.Nome,
                    autor.SobreNome
                }, "fila.editar");

                resposta.Mensagem = "Autor editado com sucesso!";
                return resposta;
            }
            catch (Exception ex)
            {

                resposta.Mensagem = ex.Message;
                resposta.Status = false;

                return resposta;
            }
        }

        public async Task<ResponseModel<AutorModel>> ExcluirAutor(int IdAutor)
        {
            ResponseModel<AutorModel> resposta = new ResponseModel<AutorModel>();
            try
            {
                var autor = await _context.Autores.FirstOrDefaultAsync(linha => linha.Id == IdAutor);

                if (autor == null)
                {
                    resposta.Mensagem = "Nenhum registro localizar";
                    resposta.Status = false;
                    return resposta;
                }

                _context.Remove(autor);
                await _context.SaveChangesAsync();

                resposta.Mensagem = "Autor removido com sucesso!";
                return resposta;

            }
            catch (Exception ex)
            {

                resposta.Mensagem = ex.Message;
                resposta.Status = false;

                return resposta;
            }
        }

        public async Task<ResponseModel<List<AutorModel>>> ListarAutor()
        {
            ResponseModel<List<AutorModel>> resposta = new ResponseModel<List<AutorModel>>();

            try
            {
                var autores = await _context.Autores.ToListAsync();
                if (autores == null)
                {
                    resposta.Mensagem = "Nenhum registro localizar";
                    resposta.Status = false;
                    return resposta;
                }

                resposta.Dados = autores;

                return resposta;

            }
            catch (Exception ex)
            {
                resposta.Mensagem = ex.Message;
                resposta.Status = false;

                return resposta;


            }
        }
    }
}
