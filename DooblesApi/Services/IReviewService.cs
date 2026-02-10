using DooblesApi.Models;

namespace DooblesApi.Services;

public interface IReviewService
{
    Task<Review> CreateReviewAsync(Review review);
    Task<Review?> GetReviewByIdAsync(int id);
    Task<List<Review>> GetAllReviewsAsync();
}
