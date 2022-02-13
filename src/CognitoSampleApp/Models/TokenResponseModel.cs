using System;

namespace CognitoSampleApp.Models
{
    public class TokenResponseModel
    {
        public string RefreshToken { get; set; }
        
        public string Token { get; set; }

        public DateTimeOffset Expiry { get; set; }
    }
}