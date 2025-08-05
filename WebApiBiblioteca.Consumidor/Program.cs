using WebApiBiblioteca.Consumidor.Models;
using WebApiBiblioteca.Consumidor.Service;


var builder = WebApplication.CreateBuilder(args);

// Configurações do RabbitMQ
builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));

// Serviços da aplicação
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddHostedService<AutorConsumerService>();

// Swagger (opcional, já que não há controllers, mas se quiser expor status etc.)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Ativa Swagger somente em desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Apenas se você tiver controllers, como algum endpoint de status
app.MapControllers();

app.Run();
