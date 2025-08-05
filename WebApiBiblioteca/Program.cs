using Microsoft.EntityFrameworkCore;
using WebApiBiblioteca.Data;
using WebApiBiblioteca.Infra;
using WebApiBiblioteca.Service.Autores;
using WebApiBiblioteca.Service.RabbitMQ;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
object value = builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAutorService, AutorService>();
builder.Services.AddScoped<IRabbitMQService, RabbitMQService>();

//Conexao com o banco via EF
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

});
builder.Services.AddSingleton<RabbitMQService>();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.EnsureCreated();
    }
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
