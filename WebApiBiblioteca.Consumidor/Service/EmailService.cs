using MailKit.Net.Smtp;
using MimeKit;
using WebApiBiblioteca.Consumidor.DTO;
using WebApiBiblioteca.Consumidor.Models;
using WebApiBiblioteca.Consumidor.Service;

namespace WebApiBiblioteca.Consumidor.Service;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task EnviarEmailAsync(AutorMensagemDTO autor)
    {
        var mensagem = new MimeMessage();
        mensagem.From.Add(MailboxAddress.Parse(_config["Smtp:User"]));
        mensagem.To.Add(MailboxAddress.Parse(_config["EmailDestino"]));
        mensagem.Subject = $"Novo Autor Cadastrado: {autor.Nome}";

        mensagem.Body = new TextPart("plain")
        {
            Text = $"📚 Autor: {autor.Nome} {autor.SobreNome}\n🆔 ID: {autor.Id}\n📌 Evento: {autor.Evento}"
        };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_config["Smtp:Host"], int.Parse(_config["Smtp:Port"]), false);
        await smtp.AuthenticateAsync(_config["Smtp:User"], _config["Smtp:Password"]);
        await smtp.SendAsync(mensagem);
        await smtp.DisconnectAsync(true);
    }
}
