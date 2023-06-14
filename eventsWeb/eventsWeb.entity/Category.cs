namespace eventsWeb.entity
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string? Name { get; set; }
        // public string? Description { get; set; }
        // public string? imgLink {get; set;}
        public List<ProductCategory>? ProductCategories {get; set;}          
    }
}