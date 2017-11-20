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
        [HttpGet]
        public IEnumerable<Order> GetAll()
        {
            var uow = XpoHelper.CreateUnitOfWork();
            // We can't dispose the UnitOfWork here, because the standard
            // serialization process only starts when the request finishes.
            // To make sure that it is eventually disposed of, register
            // it with the response.
            Response.RegisterForDispose(uow);
            return uow.Query<Order>();
        }

        [HttpPost]
        public IActionResult Post(string values)
        {
            using (var uow = XpoHelper.CreateUnitOfWork())
            {
                var artist = new Order(uow);
                JsonConvert.PopulateObject(values, artist);
                uow.CommitChanges();
                return Ok();
            }
        }

        [HttpPut]
        public IActionResult Put(int key, string values)
        {
            using (var uow = XpoHelper.CreateUnitOfWork())
            {
                var artist = uow.GetObjectByKey<Order>(key);
                JsonConvert.PopulateObject(values, artist);
                uow.CommitChanges();
                return Ok();
            }
        }

        [HttpDelete]
        public void Delete(int key)
        {
            using (var uow = XpoHelper.CreateUnitOfWork())
            {
                uow.Delete(uow.GetObjectByKey<Order>(key));
                uow.CommitChanges();
            }
        }
    }
}