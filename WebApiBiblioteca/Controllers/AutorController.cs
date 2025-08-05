using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApiBiblioteca.DTO.Autor;
using WebApiBiblioteca.Models;
using WebApiBiblioteca.Service.Autores;

namespace WebApiBiblioteca.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutorController : ControllerBase
    {

        private readonly IAutorService _serviceAutor;
        public AutorController(IAutorService autorService)
        {
            _serviceAutor = autorService;
        }

        [HttpGet("ListarAutores")]
        public async Task<ActionResult<ResponseModel<List<AutorModel>>>> ListaAutores() 
        {
            var autores = await _serviceAutor.ListarAutor();
            return Ok(autores);        
        }

        [HttpGet("BuscarAutoPorId/{idAutor}")]
        public async Task<ActionResult<ResponseModel<AutorModel>>> BuscarAutoPorId(int idAutor)
        {
            var autores = await _serviceAutor.BuscarAutorPorId(idAutor);
            return Ok(autores);
        }


        [HttpGet("BuscarAutoPorIdLivro/{idLivro}")]
        public async Task<ActionResult<ResponseModel<AutorModel>>> BuscarAutorPorIdLivro(int idLivro)
        {
            var autores = await _serviceAutor.BuscarAutorPorIdLivro(idLivro);
            return Ok(autores);
        }


        [HttpPost("CriarAutor")]
        public async Task<ActionResult> CriarAutor(AutorCriacaoDTO AutorCriacaoDTO)
        {
            await _serviceAutor.CadastrarAutor(AutorCriacaoDTO);

            return Ok();
           
        }

        [HttpPut("EditarAutor")]
        public async Task<ActionResult<ResponseModel<AutorModel>>> EditarAutor(AutorEditarDTO autorEditarDTO)
        {
           var resultado = await _serviceAutor.EditarAutor(autorEditarDTO);

            return Ok(resultado);

        }

        [HttpDelete("ExcluirAutor")]
        public async Task<ActionResult> ExcluirAutor(int autorId)  
        {
            var resultado =  await _serviceAutor.ExcluirAutor(autorId);

            return Ok(resultado);

        }
    }
}
