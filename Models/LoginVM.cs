namespace TicketAPI.Models
{
    public class LoginVM
    {
        public string Signer { get; set; } // Ethereum account that claim the signature
        public string Signature { get; set; } // The signature
        public string Message { get; set; } // The plain message
        public string Hash { get; set; } // The prefixed and sha3 hashed message 

        public LoginVM() { }

        public LoginVM(string signer, string signature, string message, string hash)
        {
            Signer = signer;
            Signature = signature;
            Message = message;
            Hash = hash;
        }
    }
}
