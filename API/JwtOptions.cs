internal class JwtOptions
{
    public class JwtConfig
    {
        public string Role { get; set; } // "admin" ou "guest"
        public string UserId { get; set; }
        public DateTime Expiration { get; set; }
    }

}