using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreWithAngular.Data;
using AspNetCoreWithAngular.Data.Entities;
using AspNetCoreWithAngular.Services;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace AspNetCoreWithAngular
{
    public class Startup
    {
        private readonly IConfiguration _config;
        private readonly IHostingEnvironment _environment;

        public Startup(IConfiguration config, IHostingEnvironment environment)
        {
            _config = config;
            _environment = environment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Einbinden der Identity und Setzen der Passwort-Regeln
            //Achtung: Die Passwort-Regenl sollten in der appsettings.json gesetzt werden
            //Zusätzlich werden die Identities in derselben Datenbank gehalten wie das Projekt
            //Will man eine eigenständige Datenbank für Identity müsste man die an einen anderen DBContext einbinden
            //Siehe dazu https://app.pluralsight.com/player?course=aspnetcore-mvc-efcore-bootstrap-angular-web Kapitel 9
            services.AddIdentity<User, IdentityRole>(cfg =>
            {
                cfg.User.RequireUniqueEmail = true;
                cfg.Password.RequiredUniqueChars = 1;
            }).AddEntityFrameworkStores<DatabaseContext>();

            //Authentication-Methoden
            //Siehe dazu https://app.pluralsight.com/player?course=aspnetcore-mvc-efcore-bootstrap-angular-web Kapitel 9
            services.AddAuthentication()
                //Cookie Authentication ist der default; 
                .AddCookie()
                //JwtBearer arbeitet mit tokens, die sich aus den Credentials des Users ergeben
                .AddJwtBearer(cfg =>
                {
                    var validIssuer = _config["Tokens:Issuer"];
                    var validAudience = _config["Tokens:Audience"];
                    var key = _config["Tokens:Key"];
                    cfg.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                    {
                        ValidIssuer = validIssuer,
                        ValidAudience = validAudience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                    };
                });

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


            services.AddMvc(opt =>
            {
                if (_environment.IsProduction())
                {
                    //Für die Production wird https eingeschaltet...
                    opt.Filters.Add(new RequireHttpsAttribute());
                }
            })
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
            //app.UseCookiePolicy();

            app.UseAuthentication();

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
                    seeder.Seed().Wait();
                }
            }
        }
    }
}
