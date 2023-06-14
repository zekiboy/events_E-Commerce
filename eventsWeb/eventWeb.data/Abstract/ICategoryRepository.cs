using eventsWeb.entity;

namespace eventWeb.data.Abstract
{
    public interface ICategoryRepository:IRepository<Category>
    {
        Category GetByIdwithProducts(int categoryId);
        void  DeleteProductFromCategory(int productId, int categoryId);
    }
}