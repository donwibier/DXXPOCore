using System;
using System.Linq;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Data.Filtering;

namespace console
{
    public class Order : XPObject
    {
        public Order(Session session) : base(session) { }

        private DateTime _OrderDate;
        public DateTime OrderDate
        {
            get { return _OrderDate; }
            set { SetPropertyValue("OrderDate", ref _OrderDate, value); }
        }
        private int _OrderNo;
        public int OrderNo
        {
            get { return _OrderNo; }
            set { SetPropertyValue("OrderNo", ref _OrderNo, value); }
        }
        private string _Client;
        public string Client
        {
            get { return _Client; }
            set { SetPropertyValue("Client", ref _Client, value); }
        }

        [Association("Order-OrderItems"), Aggregated]
        public XPCollection<OrderItem> OrderItems
        {
            get { return GetCollection<OrderItem>("OrderItems"); }
        }
        [PersistentAlias("OrderItems.Sum(Price)")]
        public decimal TotalAmount
        {
            get { return Convert.ToDecimal(EvaluateAlias("TotalAmount")); }
        }
    }

    public class OrderItem : XPObject
    {
        public OrderItem(Session session) : base(session) { }

        private Order _Order;
        [Association("Order-OrderItems")]
        public Order Order
        {
            get { return _Order; }
            set { SetPropertyValue("Order", ref _Order, value); }
        }


        private int _Qty;
        public int Qty
        {
            get { return _Qty; }
            set { SetPropertyValue("Qty", ref _Qty, value); }
        }
        private string _Description;
        public string Description
        {
            get { return _Description; }
            set { SetPropertyValue("Description", ref _Description, value); }
        }
        private decimal _UnitPrice;
        public decimal UnitPrice
        {
            get { return _UnitPrice; }
            set { SetPropertyValue("UnitPrice", ref _UnitPrice, value); }
        }
        [PersistentAlias("Qty * UnitPrice")]
        public decimal Price
        {
            get { return Convert.ToDecimal(EvaluateAlias("Price")); }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("XPO Console Demo");

            XpoDefault.DataLayer = XpoDefault.GetDataLayer(
                SQLiteConnectionProvider.GetConnectionString("console.db"),
                AutoCreateOption.DatabaseAndSchema);

            using (var uow = new UnitOfWork())
            {
                uow.ClearDatabase();
            }

            using (var uow = new UnitOfWork())
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

            using (var uow = new UnitOfWork())
            {
                var orders = from o in uow.Query<Order>()
                             where o.OrderDate < DateTime.Now.AddDays(-10)
                             orderby o.OrderDate
                             select o;
                foreach (var o in orders){
                    Console.WriteLine($"Order #{o.OrderNo} / {o.OrderDate}, client {o.Client}, Total Amount { o.TotalAmount }");
                    foreach(var i in o.OrderItems){
                        Console.WriteLine($"   {i.Qty} x {i.Description} ({i.UnitPrice}) = {i.Price}");
                    }
                }
            }
        }
    }
}
