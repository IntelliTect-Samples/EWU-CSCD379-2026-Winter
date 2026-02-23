using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace KalesGalleryApi.Models;

public class ApplicationUser : IdentityUser
{
    [JsonIgnore]
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    [JsonIgnore]
    public ICollection<Commission> Commissions { get; set; } = new List<Commission>();
}
