using System.ComponentModel.DataAnnotations;

namespace DooblesApi.Models;

public class Review
{
    public int Id { get; set; }
    [Range(1, 5)]
    public required int Stars { get; set; }
    public required string ReviewText { get; set; }
    public required string Reviewer { get; set; }
}
