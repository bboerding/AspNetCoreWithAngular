using AspNetCoreWithAngular.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreWithAngular.Data
{
    //Um eine neue Datenbank anzulegen, sind folgende Befehle per Kommandozeile durchzuführen
    //- dotnet ef database update => Damit wird eine leere Datenbank ohne Tabellen angelegt
    //- dotnet ef migrations add InitialDb => Damit wird ein Migrations-Folder mit allen Datenbank-Tabellen und Feldern angelegt
    //- dotnet ef database update => Jetzt werden die Tabellen angelegt


    //Der Datenbank Kontext erbt von IdentityDbContext
    //  was erweiterte Funktionalität auch über die UserIdentity enthält
    //Das User-Objekt ist eine eigene Instanz von IdentityUser...
    public class DatabaseContext : IdentityDbContext<User>
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options): base(options)
        {

        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        //public DbSet<OrderItem> OrderItems { get; set; }
    }
}
