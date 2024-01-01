using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using TicketAPI.CloudStorage;
using TicketAPI.Models;

namespace TicketAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserAuthController : ControllerBase
    {

        private readonly IConfiguration _configuration;

        public UserAuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("/user/signup")]
        public async Task<IActionResult> SignUp([FromBody] CreateUser payload)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                Guid guid = Guid.NewGuid();
                string id = guid.ToString();
                string username = payload.UserName;
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(payload.Password);
                string email = payload.Email;

                using (SqlConnection cn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    string insertQuery = "INSERT INTO USERS (Id, UserName, Password, Email) VALUES (@Id, @UserName, @Password, @Email)";

                    using (SqlCommand cmd = new SqlCommand(insertQuery, cn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.Parameters.AddWithValue("@UserName", username);
                        cmd.Parameters.AddWithValue("@Password", hashedPassword);
                        cmd.Parameters.AddWithValue("@Email", email);

                        await cn.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                        await cn.CloseAsync();
                    }
                }

                var createdUser = new { id, username, message = "Created User Successfully" };
                return Ok(createdUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("/user/login")]
        public async Task<IActionResult> Login([FromBody] GetUser payload)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                string email = payload.Email;
                string password = payload.Password;

                using (SqlConnection cn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    string query = "SELECT Id, Username, Password, Email FROM USERS WHERE Email = @Email";

                    using (SqlCommand cmd = new SqlCommand(query, cn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);

                        await cn.OpenAsync();
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                string storedHashedPassword = reader["Password"].ToString();

                                // Verify the hashed password
                                if (BCrypt.Net.BCrypt.Verify(password, storedHashedPassword))
                                {
                                    var user = new { id = reader["Id"], username = reader["Username"], email = reader["Email"], message="Login Successful"};
                                    return Ok(user);
                                }
                                else
                                {
                                    return Unauthorized("Incorrect password.");
                                }
                            }
                            else
                            {
                                return NotFound("User not found.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
