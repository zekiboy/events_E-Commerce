using System.ComponentModel.DataAnnotations;
using eventsWeb.entity;

namespace events.Models
{
    public class CategoryModel
    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage="Kategori adı zorunludur.")]
        [StringLength(100,MinimumLength=5,ErrorMessage="Kategori için 5-100 arasında değer giriniz.")]     
        public string? Name { get; set; }      
        public List<Product>? Products { get; set; } 
    }
}