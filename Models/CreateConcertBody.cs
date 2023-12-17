namespace TicketAPI.Models
{
    public class CreateConcertBody
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime PerformanceDate { get; set; }
        public IFormFile Image { get; set; }
        public DateTime SellDate {get; set;}

        public CreateConcertBody() { }

        public CreateConcertBody(string Id, string Name, IFormFile Image, DateTime SellDate, DateTime PerfomanceDate)
        {
            this.Id = Id;
            this.Name = Name;
            this.Image = Image;
            this.SellDate = SellDate;
            this.PerformanceDate = PerfomanceDate;
        }
    }
}
