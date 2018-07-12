using AspNetCoreWithAngular.Data;
using AspNetCoreWithAngular.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreWithAngular.Controllers
{
    //Für WebApis muss die Route angegeben werden
    //- hier wird eine api-Prefix gesetzt
    //- [Controller] wird dynamisch auf "Products" gesetzt
    //Alternativ kann man auch [Route("api/Products")] bzw. [Route(Products)] angeben...
    //Aufruf: http://localhost:8888/spi/Products
    [Route("api/[Controller]")]
    public class ProductsController : Controller
    {
        private IDatabaseRepository _repository;
        private ILogger _logger;

        public ProductsController(IDatabaseRepository databaseRepository, ILogger<ProductsController> logger)
        {
            _repository = databaseRepository;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                //Ok ist eine Http-200 Rückmeldung
                return Ok(_repository.GetAllProducts());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get products: {ex}");
                //BadRequest ist eine Http-400 Rückmeldung
                return BadRequest("Failed to get products");
            }
        }
    }
}
