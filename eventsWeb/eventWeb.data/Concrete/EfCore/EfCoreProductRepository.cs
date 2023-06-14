using eventsWeb.entity;
using eventWeb.data.Abstract;
using Microsoft.EntityFrameworkCore;

namespace eventWeb.data.Concrete.EfCore
{
    public class EfCoreProductRepository : EfCoreGenericRepository<Product, eventsWebContext>, IProductRepository
    {
        public Product GetByIdWithCategories(int id)
        {
            //admin ürün detay sayfasında kat listelemek için
            // normal detail sayfasında da aynı metodu kullanmıştık onu çekebilirsin

            using(var context = new eventsWebContext())
            {
                var cmd = context.Products
                                    .Where(i=>i.ProductId==id)
                                    .Include(i=>i.ProductCategories)
                                    .ThenInclude(i=>i.Category)
                                    .FirstOrDefault();
                return cmd;
            }
        }

        // public void Create(Product entity)
        // {
        //     throw new NotImplementedException();
        // }

        // public void Delete(Product entity)
        // {
        //     throw new NotImplementedException();
        // }

        // public void Update(Product entity)
        // {
        //     throw new NotImplementedException();
        // }

        // List<Product> IRepository<Product>.GetAll()
        // {
        //     throw new NotImplementedException();
        // }

        // Product IRepository<Product>.GetById(int id)
        // {
        //     throw new NotImplementedException();
        // }

        public List<Product> GetPopularProducts()
        {
            using(var context = new eventsWebContext())
            {
                return context.Products.ToList();
            }
        }

        public List<Product> GetProductByCategory(string name)
        {
            throw new NotImplementedException();
        }

        public Product GetProductDetails(int id)
        {
            using(var context = new eventsWebContext())
            {
                //her ürünün kategori bilgisini almamız içni önce productstan productcategory'e oradan da categorye gideceğiz 
                // ilk tabloyu bağlarken ınclude bi sonrakini bağlarken thenınclude kullanıyoruz
                var rtn = context.Products
                                    .Where(i=>i.ProductId==id)
                                    .Include(i=>i.ProductCategories)
                                    .ThenInclude(i=>i.Category)
                                    .FirstOrDefault();

                return rtn;
            }
        }

        public List<Product> GetProductsByQSearch(string search)
        {
            using(var context = new eventsWebContext())
            {
                var products = context.Products.AsQueryable();
               
                products=products
                            .Include(i=>i.ProductCategories)
                            .ThenInclude(i=>i.Category)
                            .Where(i=>i.ProductCategories.Any(a=>
                                a.Product.Name.ToLower().Contains(search.ToLower()) 
                                    || a.Product.Description.ToLower().Contains(search.ToLower())
                                    || a.Product.Location.ToLower().Contains(search.ToLower())
                                    || a.Category.Name.ToLower().Contains(search.ToLower())
                                ));
                return products.ToList();
                
            }
        }

        public List<Product> GetProductsWithCategories()
        {
            //anasayfaya farklı kategori başlıkları çekebilmek için yazdım
            using(var context = new eventsWebContext())
            {
                var products = context.Products.AsQueryable();
   
                    products = products
                                    .Include(i=>i.ProductCategories)
                                    .ThenInclude(i=>i.Category);
                return products.ToList();
            }
        }

        public void Update(Product entity, int[] categoryIds)
        {
            using(var context = new eventsWebContext())
            {
                var product = context.Products
                        .Include(i=>i.ProductCategories)
                        .FirstOrDefault(i=>i.ProductId==entity.ProductId);

                if(product!=null)
                {
                    product.ProductId=entity.ProductId;
                    product.Name=entity.Name;
                    product.Price=entity.Price;
                    product.Description=entity.Description;
                    product.ImgUrl=entity.ImgUrl;
                    product.Location=entity.Location;
                    product.sittingUrl=entity.sittingUrl;
                    product.Rules=entity.Rules;
                    product.eventDate=entity.eventDate;

                    product.ProductCategories = categoryIds.Select(catId=>new ProductCategory()
                    {
                        ProductId=entity.ProductId,
                        CategoryId= catId
                    }).ToList();

                    context.SaveChanges();
                }        
            }
        }


        List<Product> IProductRepository.GetProductByCategoryId(int id)
        {
            using(var context = new eventsWebContext())
            {
                var products = context.Products.AsQueryable();
                //AsQueryable biz sorguyu yazıyoruz ama veritabanına göndermeden ben üzerine bir linq sorgusu eklemek istiyorum demek
                
                if(id!=null)
                {
                    products = products
                                    .Include(i=>i.ProductCategories)
                                    .ThenInclude(i=>i.Category)
                                    .Where(i=>i.ProductCategories.Any(a=>a.Category.CategoryId== id));                    
                }
                return products.ToList();
            }
        }

        // public void UpdatePCategory(ProductCategory entity)
        // {
        //     using (var context = new eventsWebContext())
        //     {
        //         context.Entry(entity).State = EntityState.Modified;
        //         context.SaveChanges();
        //     }
        // }

    }
}