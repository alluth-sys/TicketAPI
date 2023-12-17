namespace TicketAPI.Models
{
    public class Concert
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime PlayingDate {get; set;}
        public string[] PaymentMethods { get; set;}
        public string Notice { get; set;}
        public string[] Channel {  get; set; }




    }
}
