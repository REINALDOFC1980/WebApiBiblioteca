using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json.Serialization;

namespace WebApiBiblioteca.Models
{
    public class AutorModel
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? SobreNome { get; set; }

        [JsonIgnore]//evita que esse código seja ignorado no swagger
        public ICollection<LivroModel>? Livros { get; set; }
    }
}
