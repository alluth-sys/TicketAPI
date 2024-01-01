namespace TicketAPI.Models
{
    public class GetUser
    {
        public string Password { get; set; }
        public string Email { get; set; }

        public GetUser() { }

        public GetUser(string password, string email)
        {
            Password = password;
            Email = email;
        }
    }
}


