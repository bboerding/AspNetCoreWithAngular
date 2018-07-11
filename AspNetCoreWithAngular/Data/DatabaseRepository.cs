using AspNetCoreWithAngular.Data.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public bool SaveChanges()
        {
            _logger.LogInformation("SaveChanges was called");
            return _context.SaveChanges() > 0;
        }
    }
}
