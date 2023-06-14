using eventsWeb.entity;

namespace events.Models
{
    public class ProductDetailModel
    {
        public Product Product{get; set;}
        public List<Category>? Categories { get; set; }
    }
}