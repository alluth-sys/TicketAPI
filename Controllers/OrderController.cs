using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using TicketAPI.CloudStorage;
using TicketAPI.Models;

namespace TicketAPI.Controllers
{
    [ApiController]
    public class OrderController : Controller
    {
        private readonly IConfiguration _configuration;

        public OrderController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("/order/create-purchase")]
        public async Task<IActionResult> PurchaseTicket([FromBody] Order payload)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                Guid guid = Guid.NewGuid();
                string orderId = guid.ToString();
                string uid = payload.UserId;
                string eventName = payload.EventName;
                string purchaseDate = payload.PurchaseDate.ToShortDateString();
                string seat = payload.Seat;
                string payment = payload.PaymentMethod;
                string amount = payload.Amount;

                using (SqlConnection cn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    string insertQuery = "INSERT INTO Orders (OrderId, UserId, EventName, PurchaseDate, Seat, Payment, Amount) VALUES (@OrderId, @UserId, @EventName, @PurchaseDate, @Seat, @Payment, @Amount)";

                    using (SqlCommand cmd = new SqlCommand(insertQuery, cn))
                    {
                        cmd.Parameters.AddWithValue("@OrderId", orderId);
                        cmd.Parameters.AddWithValue("@UserId", uid);
                        cmd.Parameters.AddWithValue("@EventName", eventName);
                        cmd.Parameters.AddWithValue("@PurchaseDate", purchaseDate);
                        cmd.Parameters.AddWithValue("@Seat", seat);
                        cmd.Parameters.AddWithValue("@Payment", payment);
                        cmd.Parameters.AddWithValue("@Amount", amount);

                        await cn.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                        await cn.CloseAsync();
                    }
                }

                var createdOrder = new { uid, eventName, message = "Created Order Successfully" };
                return Ok(createdOrder);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("/order/list-purchase/{userid}")]
        public async Task<IActionResult> ListOrders(string userid)
        {
            List<GetOrder> result = new List<GetOrder>();
            try
            {
                using (SqlConnection cn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await cn.OpenAsync();
                    string query = "SELECT * FROM Orders WHERE UserId = @Id";

                    using (SqlCommand cmd = new SqlCommand(query, cn))
                    {
                        cmd.Parameters.AddWithValue("@Id", userid);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                result.Add(new GetOrder
                                {
                                    OrderId = reader.GetString(0),
                                    EventName = reader.GetString(1),
                                    PurchaseDate = reader.GetString(2),
                                    PaymentMethod = reader.GetString(3),
                                    Seat = reader.GetString(4),
                                    Amount = reader.GetString(5)
                                });
                            }
                        }
                    }

                    await cn.CloseAsync();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
