using eventsWeb.business.Abstract;
using eventsWeb.entity;
using eventWeb.data.Abstract;

namespace eventsWeb.business.Concrete
{
    public class ProductManager : IProductService
    {

        private IProductRepository _productRepository;
        public ProductManager(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public void Create(Product entity)
        {
            _productRepository.Create(entity);
        }


        public void Delete(Product entity)
        {
            _productRepository.Delete(entity);
        }

        public List<Product> GetAll()
        {
            return _productRepository.GetAll();
        }

        public Product GetById(int id)
        {
            return _productRepository.GetById(id);
        }

        public Product GetByIdWithCategories(int id)
        {
            return _productRepository.GetByIdWithCategories(id);
        }

        public List<Product> GetProductByCategoryId(int id)
        {
            return _productRepository.GetProductByCategoryId(id);
        }

        public Product GetProductDetails(int id)
        {
            return _productRepository.GetProductDetails(id);
        }

        public List<Product> GetProductsByQSearch(string search)
        {
            return _productRepository.GetProductsByQSearch(search);
        }

        public List<Product> GetProductsWithCategories()
        {
            return _productRepository.GetProductsWithCategories();
        }

        public void Update(Product entity)
        {
            _productRepository.Update(entity);
        }

        public void Update(Product entity, int[] categoryIds)
        {
            _productRepository.Update(entity, categoryIds);
        }

        // public void UpdatePCategory(ProductCategory entity)
        // {
        //     _productRepository.UpdatePCategory(entity);
        // }
    }
}