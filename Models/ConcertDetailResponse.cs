namespace TicketAPI.Models
{
    public class ConcertDetailResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string PerformanceDate { get; set; }
        public string Image { get; set; }
        public string SellDate { get; set; }
        public int Price { get; set; }

        public ConcertDetailResponse() {}

        public ConcertDetailResponse(string Id, string Name, string Image, string SellDate, string PerfomanceDate, int price)
        {
            this.Id = Id;
            this.Name = Name;
            this.Image = Image;
            this.SellDate = SellDate;
            this.PerformanceDate = PerfomanceDate;
            Price = price;
        }
    }
}
