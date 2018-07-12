using AspNetCoreWithAngular.Data.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreWithAngular.Data
{
    public class DatabaseRepository : IDatabaseRepository
    {
        private DatabaseContext _context;
        private ILogger<DatabaseRepository> _logger;

        public DatabaseRepository(DatabaseContext context, ILogger<DatabaseRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IEnumerable<Product> GetAllProducts()
        {
            try
            {
                _logger.LogInformation("GetAllProducts was called");
                return _context.Products
                               .OrderBy(p => p.Title)
                               .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get all products {ex}");
                return null;
            }
        }

        public IEnumerable<Product> GetProductsByCategory(string category)
        {
            try
            {
                _logger.LogInformation("GetProductsByCategory was called");
                return _context.Products
                               .Where(p => p.Category == category)
                               .OrderBy(p => p.Title)
                               .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get all products by Category {ex}");
                return null;
            }
        }

        public IEnumerable<Order> GetAllOrders(bool includeItems)
        {
            if (includeItems)
            {
                //Alle Orders incl. deren OrderItems und deren Produktbeschreibung
                return _context.Orders
                               //Zunächst die Items hinzufügen
                               .Include(o => o.Items)
                               //Dann innerhalb der Items das Produkt des Items hinzufügen
                               .ThenInclude(o => o.Product)
                               .ToList();
            }
            else
            {
                //Alle Orders ohne die Items
                return _context.Orders.ToList();
            }
        }

        public Order GetOrder(int id, bool includeItems)
        {
            if (includeItems)
            {
                //Holt sich eine bestimmte Order incl. deren OrderItems und deren Produkt
                return _context.Orders
                               //Zunächst die Items hinzufügen
                               .Include(o => o.Items)
                               //Dann innerhalb der Items das Produkt des Items hinzufügen
                               .ThenInclude(o => o.Product)
                               .Where(o => o.Id == id)
                               .FirstOrDefault();
            }
            else
            {
                return _context.Orders
                               .Where(o => o.Id == id)
                               .FirstOrDefault();
            }
        }

        public bool SaveChanges()
        {
            _logger.LogInformation("SaveChanges was called");
            return _context.SaveChanges() > 0;
        }

        //Fügt einen Datensatz in die Datenbank ein
        //Dabei werden über das model die zugehörigen Tabellen (z.B. Orders und OrderItems) ermittelt
        //und die entsprechenden Datensätze eingefügt
        public void AddEntity(object model)
        {
            _context.Add(model);
        }
    }
}
