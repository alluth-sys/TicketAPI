using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TicketAPI.Models;
using Nethereum.Signer;
using Nethereum.Util;

namespace TicketAPI.Controllers
{
    [Route("api/[controller]")]
    public class TokenController : Controller
    {
        private IConfiguration _config;
        public TokenController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost]
        [Route("/api/authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] UserVM userDto)
        {
            if (IsValidSignature(userDto.Account, userDto.Signature))
            {
                var token = GenerateJwtToken(userDto.Account);
                return Ok(new { token = token });
            }
            return Unauthorized();
        }

        private bool IsValidSignature(string account, string signature)
        {
            // The original message you asked the user to sign
            var originalMessage = $"I hereby declare the authenticity of my existence";

            // Recover the account from the signature
            var recoveredAccount = new EthereumMessageSigner().EncodeUTF8AndEcRecover(originalMessage, signature);

            return account.Equals(recoveredAccount, StringComparison.InvariantCultureIgnoreCase);
        }

        private string GenerateJwtToken(string account)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]); // Use a secure key from configuration
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", account) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
