namespace TicketAPI.Models
{
    public class CreateUser
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }

        public CreateUser() { }

        public CreateUser(string username, string password, string email)
        {
            UserName = username;
            Password = password;
            Email = email;
        }
    }
}
