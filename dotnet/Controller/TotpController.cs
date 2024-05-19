using Microsoft.AspNetCore.Mvc;
using OtpNet;
using QRCoder;
using TOTPExample.Model;
using Microsoft.AspNetCore.Mvc;
using OtpNet;
using QRCoder;
using System.Collections.Generic;
using System.IO;

namespace TOTPExample.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private static readonly Dictionary<string, User> Users = new Dictionary<string, User>();

        [HttpPost("register")]
        public IActionResult Register([FromBody] string username)
        {
            var key = KeyGeneration.GenerateRandomKey(20);
            var base32Secret = Base32Encoding.ToString(key);

            var user = new User { UserName = username, TOTPSecret = base32Secret };
            Users[username] = user;

            // in user device will show: Hajimohammadi:YourAppName:{username}
            var otpAuthUrl = $"otpauth://totp/YourAppName:{username}?secret={base32Secret}&issuer=Hajimohammadi";

            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrCodeData = qrGenerator.CreateQrCode(otpAuthUrl, QRCodeGenerator.ECCLevel.Q);
                using (var qrCode = new PngByteQRCode(qrCodeData))
                {
                    var qrCodeImage = qrCode.GetGraphic(20);
                    var base64QrCode = Convert.ToBase64String(qrCodeImage);
                    return Ok(new { qrCodeImage = base64QrCode });
                }
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            if (Users.TryGetValue(loginRequest.Username, out var user))
            {
                var isValid = VerifyCode(loginRequest.Code, user.TOTPSecret);
                if (isValid)
                {
                    return Ok("Login successful");
                }
                return Unauthorized("Invalid TOTP code");
            }
            return NotFound("User not found");
        }

        private bool VerifyCode(string userCode, string base32Secret)
        {
            var key = Base32Encoding.ToBytes(base32Secret);
            var correction = new TimeCorrection(DateTime.UtcNow);
            var totp = new Totp(key, timeCorrection: correction);
            return totp.VerifyTotp(userCode, out long timeStepMatched);
        }

    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Code { get; set; }
    }
}
