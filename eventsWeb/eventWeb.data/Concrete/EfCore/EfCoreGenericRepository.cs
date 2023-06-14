using eventWeb.data.Abstract;
using Microsoft.EntityFrameworkCore;

namespace eventWeb.data.Concrete
{
    public class EfCoreGenericRepository<TEntity, TContext> : IRepository<TEntity>
    where TEntity : class
    where TContext : DbContext, new()
    {
        public void Create(TEntity entity)
        {
            using (var context = new TContext())
            {

                context.Set<TEntity>().Add(entity);
                context.SaveChanges();
            }
        }

        public void Delete(TEntity entity)
        {
            using (var context = new TContext())
            {
                context.Set<TEntity>().Remove(entity);
                context.SaveChanges();
            }
        }

        public List<TEntity> GetAll()
        {
            using (var context = new TContext())
            {
                return context.Set<TEntity>().ToList();

            }
        }

        public TEntity GetById(int id)
        {
            using (var context = new TContext())
            {
                return context.Set<TEntity>().Find(id);

                //GetProductDetails metodunu aşağıdaki gibi çektim onu sil, burayı revize ederek kullan
                // var rtn = context.Products
                //        .Where(i => i.ProductId == id)
                //        .Include(i => i.ProductCategories)
                //        .ThenInclude(i => i.Category)
                //        .FirstOrDefault();

                // return rtn;

            }
        }

        public virtual void Update(TEntity entity)
        {
            using (var context = new TContext())
            {
                context.Entry(entity).State = EntityState.Modified;
                context.SaveChanges();
            }
        }
    }
}
