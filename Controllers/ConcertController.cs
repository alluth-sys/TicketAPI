using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Linq;
using TicketAPI.CloudStorage;
using TicketAPI.Models;
using static System.Net.Mime.MediaTypeNames;

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


        [HttpGet]
        [Route("/read-root")]
        public async Task<bool> ReadRoot()
        { 
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while connecting to the database: {ex.Message}");
                return false;
            }
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
                string sellDate = payload.SellDate.AddDays(1).ToShortDateString();
                string performanceDate = payload.PerformanceDate.AddDays(1).ToShortDateString();
                string region = payload.Region;

                using (SqlConnection cn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    string insertQuery = "INSERT INTO ConcertList (Id, Name, Image, SellDate, PerformanceDate, Region) VALUES (@Id, @Name, @Image, @SellDate, @PerformanceDate, @Region)";

                    using (SqlCommand cmd = new SqlCommand(insertQuery, cn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.Parameters.AddWithValue("@Name", name);
                        cmd.Parameters.AddWithValue("@Image", imgUrl);
                        cmd.Parameters.AddWithValue("@SellDate", sellDate);
                        cmd.Parameters.AddWithValue("@PerformanceDate", performanceDate);
                        cmd.Parameters.AddWithValue("@Region", region);

                        cn.Open();
                        cmd.ExecuteNonQuery();
                        cn.Close();
                    }
                }

                var createdConcert = new { id, name, imgUrl, sellDate, performanceDate, message="Created Concert Successfully" };
                return Ok(createdConcert);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("/list-concerts")]
        public async Task<IActionResult> ListConcerts([FromBody] ConcertQueryParams param)
        {
            List<ConcertDetailResponse> result = new List<ConcertDetailResponse>();
            try
            {
                using (SqlConnection cn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await cn.OpenAsync();
                    if(param.Region == "All")
                    {
                        string listQuery = "SELECT * FROM ConcertList";

                        using (SqlCommand cmd = new SqlCommand(listQuery, cn))
                        {
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    result.Add(new ConcertDetailResponse
                                    {
                                        Id = reader.GetString(0),
                                        Name = reader.GetString(1),
                                        Image = reader.GetString(2),
                                        SellDate = reader.GetString(3),
                                        PerformanceDate = reader.GetString(4),
                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        string listQuery = "SELECT * FROM ConcertList WHERE Region = @Region";

                        using (SqlCommand cmd = new SqlCommand(listQuery, cn))
                        {
                            cmd.Parameters.AddWithValue("@Region", param.Region);
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    result.Add(new ConcertDetailResponse
                                    {
                                        Id = reader.GetString(0),
                                        Name = reader.GetString(1),
                                        Image = reader.GetString(2),
                                        SellDate = reader.GetString(3),
                                        PerformanceDate = reader.GetString(4),
                                    });
                                }
                            }
                        }
                    }
                    await cn.CloseAsync();
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving concerts");
            }
        }

        [HttpGet]
        [Route("/list-banners")]
        public async Task<IActionResult> GetBanners()
        {
            List<string> result = new List<string>();


            try
            {
                using (SqlConnection cn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await cn.OpenAsync();
                    string listQuery = "SELECT Image FROM ConcertList";

                    using (SqlCommand cmd = new SqlCommand(listQuery, cn))
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                result.Add(reader.GetString(0));
                            }
                        }
                    }
                    await cn.CloseAsync();
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving concerts");
            }
        }

        [HttpGet]
        [Route("/list-concerts/{id}")]
        public async Task<IActionResult> GetConcertDetails(string id)
        {
            ConcertDetailResponse result = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await cn.OpenAsync();
                    string query = "SELECT * FROM ConcertList WHERE Id = @Id";

                    using (SqlCommand cmd = new SqlCommand(query, cn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                result = new ConcertDetailResponse
                                {
                                    Id = reader.GetString(0),
                                    Name = reader.GetString(1),
                                    Image = reader.GetString(2),
                                    SellDate = reader.GetString(3),
                                    PerformanceDate = reader.GetString(4),
                                    Price = reader.GetInt32(6)
                                };
                            }
                        }
                    }

                    await cn.CloseAsync();
                }
                if (result == null)
                {
                    return NotFound("Concert not found");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
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
