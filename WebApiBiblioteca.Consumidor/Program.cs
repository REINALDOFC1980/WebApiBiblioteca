using WebApiBiblioteca.Consumidor.Models;
using WebApiBiblioteca.Consumidor.Service;


var builder = WebApplication.CreateBuilder(args);

// Configura��es do RabbitMQ
builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));

// Servi�os da aplica��o
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddHostedService<AutorConsumerService>();

// Swagger (opcional, j� que n�o h� controllers, mas se quiser expor status etc.)
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

// Apenas se voc� tiver controllers, como algum endpoint de status
app.MapControllers();

app.Run();
