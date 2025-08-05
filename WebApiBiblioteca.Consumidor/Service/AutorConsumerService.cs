using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using WebApiBiblioteca.Consumidor.DTO;
using WebApiBiblioteca.Consumidor.Models;
using WebApiBiblioteca.Consumidor.Service;

namespace WebApiBiblioteca.Consumidor.Service;

public class AutorConsumerService : BackgroundService
{
    private readonly IServiceProvider _provider;
    private readonly RabbitMQSettings _settings;

    public AutorConsumerService(IServiceProvider provider, IOptions<RabbitMQSettings> options)
    {
        _provider = provider;
        _settings = options.Value;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory()
        {
            HostName = _settings.HostName,
            UserName = _settings.UserName,
            Password = _settings.Password
        };

        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        channel.QueueDeclare(queue: _settings.Fila, durable: false, exclusive: false, autoDelete: false);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);

            try
            {
                var autor = JsonSerializer.Deserialize<AutorMensagemDTO>(json);

                using var scope = _provider.CreateScope();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                await emailService.EnviarEmailAsync(autor!);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar mensagem: {ex.Message}");
            }
        };

        channel.BasicConsume(queue: _settings.Fila, autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }
}
