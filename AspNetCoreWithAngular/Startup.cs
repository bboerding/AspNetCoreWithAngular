using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreWithAngular.Data;
using AspNetCoreWithAngular.Services;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace AspNetCoreWithAngular
{
    public class Startup
    {
        public Startup(IConfiguration config)
        {
            _config = config;
        }

        public IConfiguration _config { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Einbinden des Datebase-Contextes
            services.AddDbContext<DatabaseContext>(cfg =>
            {
                cfg.UseSqlServer(_config.GetConnectionString("DatabaseConnectionString"));
            });

            //Hinzufügen des Automappers
            //Dafür müssen folgende NuGet-Pakete installiert werde:
            //Automapper und Automapper.Extensions.Microsoft.DependencyInjection
            services.AddAutoMapper();

            //AddTransient bedeutet, dass dieser Service unabhängig von der business-Lösung bzw. von sich verändernden Daten ist
            services.AddTransient<IMailService, NullMailService>();
            services.AddTransient<DatabaseSeeder>();

            //AddScoped muss typischerweise für Datenbank-Repositories angegeben werden
            //  weil man mit lebenden Daten arbeitet
            services.AddScoped<IDatabaseRepository, DatabaseRepository>();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc()
                //Mit dem LoopingHandlig wird gesteuert, wie sich EntityFramework verhalten soll, wenn Datenbank-Loopings vorkommen
                //Ein DatabaseLooping ist z.B. zwischen Order und OrderItems vorhanden
                //- Order hat eine beliebige Menge an OrderItems
                //- Jedes OrderItem hat eine Referenz auf Order
                .AddJsonOptions(opt => opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore)
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            if (env.IsDevelopment())
            {
                //Seed the Database
                using (var scope = app.ApplicationServices.CreateScope())
                {
                    var seeder = scope.ServiceProvider.GetService<DatabaseSeeder>();
                    seeder.Seed();
                }
            }
        }
    }
}
