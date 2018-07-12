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
    //Mittels der Route wird festgelegt, dass man die Items nur im Kontext einer order ermitteln kann
    //Die OrderId muss also explizit angegeben werden
    //Aufruf: http://localhost:8888/api/orders/1/items
    [Route("api/orders/{orderid}/items")]
    public class OrderItemsController : Controller
    {
        private IDatabaseRepository _repository;
        private ILogger<OrderItemsController> _logger;
        private IMapper _mapper;

        public OrderItemsController(IDatabaseRepository repository, 
            ILogger<OrderItemsController> logger,
            IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        //Holt sich alle OrderItems einer bestimmten Order
        //Aufruf: http://localhost:8888/api/orders/1/items
        [HttpGet]
        public IActionResult Get(int orderId)
        {
            //Die order incl items aus der Datenbank holen
            var order = _repository.GetOrder(orderId, true);
            if (order != null)
            {
                //Alle Items der Order nach OrderItemViewModel mappen
                var orderItemViewModel = _mapper.Map<IEnumerable<OrderItem>, IEnumerable<OrderItemViewModel>>(order.Items);
                return Ok(orderItemViewModel);
            }
            else
            {
                return NotFound();
            }
        }

        //Holt sich ein bestimmtes OrderItems einer bestimmten Order
        //Aufruf: http://localhost:8888/api/orders/1/items/1
        [HttpGet("{id}")]
        public IActionResult Get(int orderId, int id)
        {
            //Die order aus der Datenbank holen
            var order = _repository.GetOrder(orderId, true);
            if (order != null)
            {
                //Das OrderItem holen
                var item = order.Items.Where(i => i.Id == id).FirstOrDefault();
                if (item != null)
                {
                    var orderItemViewModel = _mapper.Map<OrderItem, OrderItemViewModel>(item);
                    return Ok(orderItemViewModel);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return NotFound();
            }
        }
    }
}
