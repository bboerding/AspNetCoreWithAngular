using System.Collections.Generic;
using AspNetCoreWithAngular.Data.Entities;

namespace AspNetCoreWithAngular.Data
{
    public interface IDatabaseRepository
    {
        IEnumerable<Product> GetAllProducts();
        IEnumerable<Product> GetProductsByCategory(string category);
        IEnumerable<Order> GetAllOrders(bool includeItems);
        IEnumerable<Order> GetAllOrdersByUser(string username, bool includeItems);
        Order GetOrder(string userName, int id, bool includeItems);
        bool SaveChanges();
        void AddEntity(object model);
    }
}