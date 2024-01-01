namespace TicketAPI.Models
{
    public class ConcertQueryParams
    {
        public string Region { get; set; }
        public ConcertQueryParams() { }
        public ConcertQueryParams(string region) {
            Region = region;
        }
    }
}
