using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using DevExpress.Xpo;
using web.Persistent;
using web.Utils;

namespace web.Controllers
{
    [Route("Orders")]
    public class OrdersController : Controller
    {
        readonly UnitOfWork uow;

        public OrdersController(UnitOfWork unitOfWork)
        {
            this.uow = unitOfWork;
        }

        [HttpGet]
        public IEnumerable<Order> GetAll()
        {
            return uow.Query<Order>();
        }

        [HttpPost]
        public IActionResult Post(string values)
        {
            var order = new Order(uow);
            JsonConvert.PopulateObject(values, order);
            uow.CommitChanges();
            return Ok();
        }

        [HttpPut]
        public IActionResult Put(int key, string values)
        {
            var order = uow.GetObjectByKey<Order>(key);
            JsonConvert.PopulateObject(values, order);
            uow.CommitChanges();
            return Ok();
        }

        [HttpDelete]
        public void Delete(int key)
        {
            uow.Delete(uow.GetObjectByKey<Order>(key));
            uow.CommitChanges();
        }
    }
}