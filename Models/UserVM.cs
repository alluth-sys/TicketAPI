namespace TicketAPI.Models
{
    public class UserVM
    {
        public string Account { get; set; } // Unique account name (the Ethereum account)
        public string Signature { get; set; } // The user name

        public UserVM() { }

        public UserVM(string account, string signature)
        {
            Account = account;
            Signature = signature;
        }
    }
}
