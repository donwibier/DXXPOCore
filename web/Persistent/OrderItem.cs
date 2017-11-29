using System;
using DevExpress.Xpo;

namespace web.Persistent
{
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
}