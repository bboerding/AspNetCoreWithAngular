using AspNetCoreWithAngular.Data.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
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
        private UserManager<User> _userManager;

        public DatabaseSeeder(DatabaseContext context, IHostingEnvironment hosting, UserManager<User> userManager)
        {
            _context = context;
            _hosting = hosting;
            _userManager = userManager;
        }

        public async Task Seed()
        {
            _context.Database.EnsureCreated();

            var user = await _userManager.FindByEmailAsync("bboerding@web.de");

            if (user == null)
            {
                user = new User
                {
                    UserName = "bboerding@web.de",
                    FirstName = "Bernhard",
                    LastName = "Börding",
                    Email = "bboerding@web.de"
                };

                var result = await _userManager.CreateAsync(user, "P@ssw0rd!");
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Failed to create a default user");
                }
            }

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
                    User = user,
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
