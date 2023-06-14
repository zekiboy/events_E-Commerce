using eventsWeb.entity;

namespace eventsWeb.business.Abstract
{
    public interface ICartService
    {
        void  InitializeCard(string userId);
        Cart GetCartByUserId(string userId);       
        void  AddToCart(string userId, int productId, int quantity);
        void DeleteFromCart(string userId, int productId);
    }
}