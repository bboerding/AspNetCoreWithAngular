using AspNetCoreWithAngular.Data;
using AspNetCoreWithAngular.Data.Entities;
using AspNetCoreWithAngular.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreWithAngular.Controllers
{
    //- hier wird eine api-Prefix gesetzt
    //- [Controller] wird dynamisch auf "Orders" gesetzt
    //Alternativ kann man auch [Route("api/Orders")] bzw. [Route(Orders)] angeben...
    [Route("api/[Controller]")]
    public class OrdersController : Controller
    {
        private IDatabaseRepository _repository;
        private ILogger _logger;
        private IMapper _mapper;

        public OrdersController(IDatabaseRepository databaseRepository, 
            ILogger<ProductsController> logger,
            IMapper mapper)
        {
            _repository = databaseRepository;
            _logger = logger;
            _mapper = mapper;
        }

        //Alle Orders
        //mit dem optionalen Parameter includeItems wird gesteuert, 
        //  ob die items ausgegeben werden oder nicht. Default ist true
        //Aufruf: http://localhost:8888/api/orders wenn auf die Items zurükgegeben werden
        //Aufruf: http://localhost:8888/api/orders?includeItems=false wenn keine Items zurükgegeben werden
        [HttpGet]
        public IActionResult Get(bool includeItems = true)
        {
            try
            {

                //Die Database-Orders in ViewModel-Orders mappen
                var ordersViewModel = _mapper.Map<IEnumerable<Order>, IEnumerable<OrderViewModel>>(_repository.GetAllOrders(includeItems));
                //Ok ist eine Http-200 Rückmeldung
                return Ok(ordersViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get orders: {ex}");
                return BadRequest("Failed to get orders");
            }
        }

        //Die Id wird als Query-Parameter übergeben
        //Es wird grundsätzlich ein OrderViewModel zurückgeliefert
        [HttpGet("{id:int}")]
        public IActionResult Get(int id, bool includeItems = true)
        {
            try
            {
                //Die order aus der Datenbank holen
                var order = _repository.GetOrder(id, includeItems);
                if (order != null)
                {
                    //Übertragen des Order-Datensatzes in ein ViewModel
                    var orderView = _mapper.Map<Order, OrderViewModel>(order);
                    return Ok(orderView); //Http-200
                }
                else return NotFound(); //Http-404
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get the Order with the Id {id}: {ex}");
                return BadRequest("Failed to get order"); //Http-400
            }
        }

        //Fügt einen neuen Datensatz ein
        //[FromBody] bedeutet, dass die Daten sich im Body des Http-Requests befinden
        //Wenn [FromBody] nicht gesetzt ist, müssen die Daten über die Querey übergeben werden
        //Zum Testen kann man innerhalb von Postman die passenden Werte im Body eintragen 
        //Siehe Pluralsight-Kurs "Building a Web App with ASP.NET Core...", Kapitel 8, Implementing Post
        [HttpPost]
        public IActionResult Post([FromBody]OrderViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Mappen vom ViewModel zum Database-Model 
                    var newOrder = _mapper.Map<OrderViewModel, Order>(model);
                    if (newOrder.OrderDate == DateTime.MaxValue)
                        newOrder.OrderDate = DateTime.Now;
                    //Einfügen des neuen Datensatzes in die Datenbank
                    _repository.AddEntity(newOrder);

                    if (_repository.SaveChanges())
                    {
                        //Wenn der Datensatz erfplgreich gespeichert wurde
                        //Wird dieser Datensatz als OrderViewModel zurückgegeben
                        var newOrderView = _mapper.Map<Order, OrderViewModel>(newOrder);
                        return Created($"/api/orders/{newOrderView.OrderId}", newOrderView);
                    }
                    else
                    {
                        _logger.LogError($"Failed to save a new order");
                        return BadRequest("Failed to save a new order");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to save a new order {ex}");
            }

            return BadRequest("Failed to save a new order");
        }
    }
}
