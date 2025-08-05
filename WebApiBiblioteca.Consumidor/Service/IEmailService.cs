using WebApiBiblioteca.Consumidor.DTO;

namespace WebApiBiblioteca.Consumidor.Service
{

    public interface IEmailService
    {
        Task EnviarEmailAsync(AutorMensagemDTO autor);
    }
}
