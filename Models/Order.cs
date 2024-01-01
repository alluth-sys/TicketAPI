namespace TicketAPI.Models
{
    public class Order
    {

        public string UserId { get; set; }
        public string EventName { get; set; }

        public DateTime PurchaseDate { get; set; }
        public string Seat { get; set; }

        public string PaymentMethod { get; set; }

        public string Amount { get; set; }

        public Order() { }

        public Order(string userId, string eventName, DateTime purchaseDate, string seat, string paymentMethod)
        {
            UserId = userId;
            EventName = eventName;
            PurchaseDate = purchaseDate;
            Seat = seat;
            PaymentMethod = paymentMethod;
        }
    }
}
