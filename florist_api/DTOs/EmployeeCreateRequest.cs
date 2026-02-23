namespace florist_api.DTOs
{
    public class EmployeeCreateRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public DateTime HireDate { get; set; } = DateTime.Now;
    }
}