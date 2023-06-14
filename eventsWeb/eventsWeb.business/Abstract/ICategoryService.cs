using eventsWeb.entity;

namespace eventsWeb.business.Abstract
{
    public interface ICategoryService
    {
        public List<Category> GetAll();
        public Category GetById(int id);
        public void Create(Category entity);
        public void Update(Category entity);
        public void Delete(Category entity);
        Category GetByIdwithProducts(int categoryId);
        void DeleteProductFromCategory(int productId, int categoryId);
        
    }
}