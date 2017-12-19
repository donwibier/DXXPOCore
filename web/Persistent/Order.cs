using System;
using DevExpress.Xpo;

namespace web.Persistent
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
}