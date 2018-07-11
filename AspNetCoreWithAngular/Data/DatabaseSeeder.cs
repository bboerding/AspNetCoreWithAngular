using AspNetCoreWithAngular.Data.Entities;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreWithAngular.Data
{
    public class DatabaseSeeder
    {
        private DatabaseContext _context;
        private IHostingEnvironment _hosting;

        public DatabaseSeeder(DatabaseContext context, IHostingEnvironment hosting)
        {
            _context = context;
            _hosting = hosting;
        }

        public void Seed()
        {
            _context.Database.EnsureCreated();

            if (!_context.Products.Any())
            {
                //Beispieldaten setzen
                //Die Datei "art.json beinhaltet alle Beispieldaten (Bilder)
                var file = Path.Combine(_hosting.ContentRootPath, "Data/art.json");
                var json = File.ReadAllText(file);

                var products = JsonConvert.DeserializeObject<IEnumerable<Product>>(json);
                _context.Products.AddRange(products);

                var order = new Order
                {
                    OrderDate = DateTime.Now,
                    OrderNumber = "12345",
                    Items = new List<OrderItem>
                    {
                        new OrderItem
                        {
                            Product = products.FirstOrDefault(),
                            Quantity = 5,
                            UnitPrice = products.FirstOrDefault().Price
                        }
                    }
                };

                _context.Add(order);
                _context.SaveChanges();
            }
        }
    }
}
