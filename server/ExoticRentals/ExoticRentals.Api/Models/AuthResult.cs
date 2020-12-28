namespace ExoticRentals.Api.Models
{
    public class AuthResult
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public bool IsAuthenticated { get { return !string.IsNullOrWhiteSpace(Token); } }
    }
}
