using eventsWeb.entity;

namespace eventWeb.data.Abstract
{
    public interface IProductRepository:IRepository<Product>
    {
        List<Product> GetPopularProducts();
        Product GetProductDetails(int id);
        List<Product> GetProductByCategoryId(int id);
        List<Product> GetProductsByQSearch(string search);
        Product GetByIdWithCategories(int id);
        void Update(Product entity, int[] categoryIds);
        List<Product> GetProductsWithCategories();



    }
}