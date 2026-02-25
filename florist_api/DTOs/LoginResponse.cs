namespace florist_api.DTOs
{
    public class LoginResponse
    {
        public required string Token { get; set; }
        public required string Username { get; set; } 
        public required string Role { get; set; }
    }
}