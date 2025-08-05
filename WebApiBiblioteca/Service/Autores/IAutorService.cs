using WebApiBiblioteca.DTO.Autor;
using WebApiBiblioteca.Models;

namespace WebApiBiblioteca.Service.Autores
{
    public interface IAutorService
    {
        Task<ResponseModel<List<AutorModel>>> ListarAutor();
        Task<ResponseModel<AutorModel>> BuscarAutorPorId(int IdAutor);
        Task<ResponseModel<AutorModel>> BuscarAutorPorIdLivro(int IdLivro);
        Task CadastrarAutor(AutorCriacaoDTO autorCriacaoDTO);

        Task <ResponseModel<AutorModel>> EditarAutor(AutorEditarDTO autorEditarDTO);

        Task <ResponseModel<AutorModel>>  ExcluirAutor(int IdAutor);

    }
}
