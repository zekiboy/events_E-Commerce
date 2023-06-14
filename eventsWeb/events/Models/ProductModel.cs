using System.ComponentModel.DataAnnotations;
using eventsWeb.entity;

namespace events.Models
{
    public class ProductModel
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage ="Name alanı zorunlu")]
        [StringLength(60,MinimumLength =3,ErrorMessage ="Name alanı için min 3 karakter girilmeli")]
        public string? Name { get; set; }

        // public List<double> Price{ get; set; }   en son fiyatı liste haline getir döngü ile al

        [Required(ErrorMessage="Price zorunlu bir alan.")]  
        [Range(1,10000,ErrorMessage="Price için 1-10000 arasında değer girmelisiniz.")]
        public double Price{ get; set; }  

        [Required(ErrorMessage="Description zorunlu bir alan.")]
        [StringLength(100,MinimumLength=5,ErrorMessage="Description 5-100 karakter aralığında olmalıdır.")]
        public string? Description {get; set;}

     
        public string? ImgUrl {get; set;}

        public bool? IsApproved {get; set;}
        // public int CategoryId {get; set;}        
        // public List<ProductCategory>? ProductCategories {get; set;}   
        public List<Category>? SelectedCategories {get; set;}

        [Required(ErrorMessage="Location zorunlu bir alan.")]  
        public string? Location { get; set; }
        public string? sittingUrl { get; set; }
        public string? Rules { get; set; }

        [Required(ErrorMessage="Event Date zorunlu bir alan.")]  
        public DateTime eventDate { get; set; } 
    }
}