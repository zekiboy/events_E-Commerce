using eventsWeb.entity;

namespace eventWeb.data.Abstract
{
    public interface ICartRepository : IRepository<Cart>
    {
        void DeleteFromCart(int cartId, int productId);
        Cart GetByUserId(string userId);   

    }
}