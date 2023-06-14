using System.Linq;
using eventsWeb.entity;
using eventWeb.data.Abstract;
using Microsoft.EntityFrameworkCore;

namespace eventWeb.data.Concrete.EfCore
{
    public class EFCoreCategoryRepository:EfCoreGenericRepository<Category,eventsWebContext>, ICategoryRepository
    {
        public void DeleteProductFromCategory(int productId, int categoryId)
        {
            using(var context = new eventsWebContext())
            {
                var cmd = "delete from productcategory where ProductId=@p0 and CategoryId=@p1;";
                context.Database.ExecuteSqlRaw(cmd,productId,categoryId);

            }
        }

        public Category GetByIdwithProducts(int categoryId)
        {
            using(var context = new eventsWebContext())
            {
                var rtn  = context.Categories
                                .Where(i=>i.CategoryId==categoryId)
                                .Include(i=>i.ProductCategories)
                                .ThenInclude(i=>i.Product)
                                .FirstOrDefault();

                return rtn;
            }
        }

    }
}