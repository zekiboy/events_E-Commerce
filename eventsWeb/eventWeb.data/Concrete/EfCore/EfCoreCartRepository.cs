using eventsWeb.entity;
using eventWeb.data.Abstract;
using Microsoft.EntityFrameworkCore;

namespace eventWeb.data.Concrete.EfCore
{
    public class EfCoreCartRepository : EfCoreGenericRepository<Cart, eventsWebContext>, ICartRepository
    {
        public void DeleteFromCart(int cartId, int productId)
        {
            using(var context = new eventsWebContext())
            {
                var cmd = @"delete from CartItems where CartId=@p0 and ProductId=@p1";
                context.Database.ExecuteSqlRaw(cmd,cartId,productId);
            }
        }

        public Cart GetByUserId(string userId)
        {
            using(var context=new eventsWebContext())
            {
                return context.Carts
                            .Include(i=>i.CartItems)
                            .ThenInclude(i=>i.Product)
                            .FirstOrDefault(i=>i.UserId==userId);
            }
        }

        public override void Update (Cart entity)
        {
            using (var context = new eventsWebContext())
            {
                context.Carts.Update(entity);
                context.SaveChanges();
            }
        }

    }



}