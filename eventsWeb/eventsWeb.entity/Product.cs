namespace eventsWeb.entity
{
    public class Product
    {
        public int ProductId { get; set; }
        public string? Name { get; set; }
        // public List<double> Price{ get; set; }   
        public double Price{ get; set; }  
        public string? Description {get; set;}
        public string? ImgUrl {get; set;}
        public bool IsApproved {get; set;}
        // public int CategoryId {get; set;}        
        public List<ProductCategory>? ProductCategories {get; set;}   

        public string? Location { get; set; }
        public string? sittingUrl { get; set; }
        public string? Rules { get; set; }

        public DateTime eventDate { get; set; }
    }
}