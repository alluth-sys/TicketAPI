namespace TicketAPI.Models
{
    public class GetOrder
    {
        public string EventName { get; set; }

        public string PurchaseDate { get; set; }
        public string Seat { get; set; }

        public string PaymentMethod { get; set; }

        public string Amount { get; set; }

        public string OrderId { get; set; }

        public GetOrder() { }

        public GetOrder(string eventName, string purchaseDate, string seat, string paymentMethod, string amount, string orderId)
        {

            EventName = eventName;
            PurchaseDate = purchaseDate;
            Seat = seat;
            PaymentMethod = paymentMethod;
            Amount = amount;
            OrderId = orderId;
        }
    }
}
