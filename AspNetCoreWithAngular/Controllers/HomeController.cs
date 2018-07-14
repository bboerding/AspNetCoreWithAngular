using AspNetCoreWithAngular.Data;
using AspNetCoreWithAngular.Models;
using AspNetCoreWithAngular.Services;
using AspNetCoreWithAngular.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;

namespace AspNetCoreWithAngular.Controllers
{
    public class HomeController : Controller
    {
        private IMailService _mailService;
        private IDatabaseRepository _repository;

        public HomeController(IMailService mailService, IDatabaseRepository repository)
        {
            _mailService = mailService;
            _repository = repository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("ÜberUns")]
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        [HttpGet("Kontakte")]
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        [HttpPost("Kontakte")]
        public IActionResult Contact(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                _mailService.SendMessage("bboerding@web.de", model.Subject, $"From: {model.Name} - {model.Email}, Message: {model.Message}");
                ViewBag.UserMessage = "Mail verschickt";
                ModelState.Clear();
            }

            return View();
        }

        //Die Shop-Funktionalität darf nur für angemeldete User durchgeführt werden
        [Authorize]
        public IActionResult Shop()
        {
            var results = _repository.GetAllProducts();
            return View(results);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
