using Microsoft.AspNetCore.Identity;

namespace florist_api.Models
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime HireDate { get; set; } = DateTime.Now;
        // add other custom props later
    }
}