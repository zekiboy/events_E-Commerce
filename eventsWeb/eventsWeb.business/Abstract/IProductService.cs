using eventsWeb.entity;

namespace eventsWeb.business.Abstract
{
    public interface IProductService
    {
        public List<Product> GetAll();
        public Product GetById(int id);
        Product GetByIdWithCategories(int id);
        public void Create(Product entity);
        public void Update(Product entity);
        public void Delete(Product entity);
        Product GetProductDetails(int id);
        List<Product> GetProductByCategoryId(int id);  
        public List<Product> GetProductsByQSearch(string search);
        void Update(Product entity, int[] categoryIds);
        public List<Product> GetProductsWithCategories();

        
        // public void UpdatePCategory(ProductCategory entity);

    }
}