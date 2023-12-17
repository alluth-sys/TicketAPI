using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Linq;
using TicketAPI.CloudStorage;
using TicketAPI.Models;

namespace TicketAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConcertController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ICloudStorage _cloudStorage;

        public ConcertController(IConfiguration configuration, ICloudStorage cloudStorage)
        {
            _configuration = configuration;
            _cloudStorage = cloudStorage;
        }

        /*[HttpGet]
        [Route("/list-concerts")]
        public async Task<IActionResult> ListConcerts()
        {
            DataTable dt = new DataTable();
            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            SqlCommand cmd = new SqlCommand("SELECT * FROM Concerts", con);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);

            List<Concert> concerts = new List<Concert>();
            foreach (DataRow item in dt.Rows)
            {
                concerts.Add(new Concert
                {
                    Id = item["Id"].ToString(),
                    Name = item["Name"].ToString(),
                    Description = item["Description"].ToString(),
                    PlayingDate = (DateTime)item["PlayingDate"],
                    PaymentMethods = item["PaymentMethods"].ToString(),
                    Notice = item["Notice"].ToString(),
                    Channel = item["Channel"].ToString()
                });
            }
            return Ok(concerts);
        }*/


        [HttpGet]
        [Route("/read-root")]
        public IActionResult ReadRoot()
        {
            return Ok("Ticket System");
        }

        [HttpPost]
        [Route("/create-concert")]
        public async Task<IActionResult> CreateConcert([FromForm] CreateConcertBody payload)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                string imgUrl = await UploadFile(payload);
                string id = payload.Id;
                string name = payload.Name;
                string sellDate = payload.SellDate.ToShortDateString();
                string performanceDate = payload.PerformanceDate.ToShortDateString();

                using (SqlConnection cn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    string insertQuery = "INSERT INTO ConcertList (Id, Name, Image, SellDate, PerformanceDate) VALUES (@Id, @Name, @Image, @SellDate, @PerformanceDate)";

                    using (SqlCommand cmd = new SqlCommand(insertQuery, cn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.Parameters.AddWithValue("@Name", name);
                        cmd.Parameters.AddWithValue("@Image", imgUrl);
                        cmd.Parameters.AddWithValue("@SellDate", sellDate);
                        cmd.Parameters.AddWithValue("@PerformanceDate", performanceDate);

                        cn.Open();
                        cmd.ExecuteNonQuery();
                        cn.Close();
                    }
                }

                // Return a 201 Created response with concert details
                var createdConcert = new { id, name, imgUrl, sellDate, performanceDate };
                return CreatedAtAction(nameof(CreateConcert), createdConcert);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the concert");
            }
        }


        private async Task<string> UploadFile(CreateConcertBody concert)
        {
            string fileNameForStorage = concert.Id.ToString() + ".jpg";
            string imageUrl = await _cloudStorage.UploadFileAsync(concert.Image, fileNameForStorage);

            return imageUrl;
        }


    }
}
