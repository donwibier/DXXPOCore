using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo;
using web.Persistent;

namespace web.Utils
{
    public static class XpoHelper
    {
        static readonly Type[] entityTypes = new Type[] {
          typeof(XPObjectType),
          typeof(Order),
          typeof(OrderItem)
        };

        public static void InitXpo(string connectionString)
        {
            XpoDefault.DataLayer = CreatePooledDataLayer(connectionString);
            XpoDefault.Session = null;
            CreateDemoData(() => new UnitOfWork());
        }
        public static ThreadSafeDataLayer CreatePooledDataLayer(string connectionString)
        {
            var dictionary = PrepareDictionary();

            using (var updateDataLayer = XpoDefault.GetDataLayer(connectionString, dictionary, AutoCreateOption.DatabaseAndSchema))
            {
                updateDataLayer.UpdateSchema(false, dictionary.CollectClassInfos(entityTypes));
            }

            string pooledConnectionString = XpoDefault.GetConnectionPoolString(connectionString);
            var dataStore = XpoDefault.GetConnectionProvider(pooledConnectionString, AutoCreateOption.SchemaAlreadyExists);
            var dataLayer = new ThreadSafeDataLayer(dictionary, dataStore); ;
            return dataLayer;
        }
        static XPDictionary PrepareDictionary()
        {
            var dict = new ReflectionDictionary();
            dict.GetDataStoreSchema(entityTypes);
            return dict;
        }
        public static UnitOfWork CreateUnitOfWork()
        {
            return new UnitOfWork();
        }

        public static void CreateDemoData(Func<UnitOfWork> createUnitOfWork)
        {
            using (var uow = createUnitOfWork())
            {
                if (!uow.Query<Order>().Any())
                {
                    var order1 = new Order(uow)
                    {
                        OrderNo = 1,
                        OrderDate = DateTime.Now.AddDays(-5),
                        Client = "Customer A"
                    };
                    order1.OrderItems.Add(new OrderItem(uow)
                    {
                        Qty = 1,
                        Description = "Product A",
                        UnitPrice = 10
                    });
                    order1.OrderItems.Add(new OrderItem(uow)
                    {
                        Qty = 2,
                        Description = "Product B",
                        UnitPrice = 15
                    });
                    new OrderItem(uow)
                    {
                        Qty = 2,
                        Description = "Product C",
                        UnitPrice = 20,
                        Order = order1
                    };

                    var order2 = new Order(uow)
                    {
                        OrderNo = 2,
                        OrderDate = DateTime.Now.AddDays(-40),
                        Client = "Customer B"
                    };
                    order2.OrderItems.Add(new OrderItem(uow)
                    {
                        Qty = 1,
                        Description = "Product A",
                        UnitPrice = 10
                    });
                    order2.OrderItems.Add(new OrderItem(uow)
                    {
                        Qty = 2,
                        Description = "Product D",
                        UnitPrice = 15
                    });
                    uow.CommitChanges();
                }
            }
        }
    }

}