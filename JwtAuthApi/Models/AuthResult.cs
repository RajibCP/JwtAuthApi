namespace JwtAuthApi.Models
{
    public class AuthResult
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public bool Result { get; set; }
        public List<string>? Errors { get; set; }
    }
}