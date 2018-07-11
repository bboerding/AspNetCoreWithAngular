using System.Collections.Generic;
using AspNetCoreWithAngular.Data.Entities;

namespace AspNetCoreWithAngular.Data
{
    public interface IDatabaseRepository
    {
        IEnumerable<Product> GetAllProducts();
        IEnumerable<Product> GetProductsByCategory(string category);
        bool SaveChanges();
    }
}