using DooblesApi.Data;
using DooblesApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DooblesApi.Services;

public class ReviewService : IReviewService
{
    private readonly DooblesDbContext _context;

    public ReviewService(DooblesDbContext context)
    {
   _context = context;
 }

    public async Task<Review> CreateReviewAsync(Review review)
    {
        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();
        return review;
    }

    public async Task<Review?> GetReviewByIdAsync(int id)
    {
        return await _context.Reviews.FindAsync(id);
    }

    public async Task<List<Review>> GetAllReviewsAsync()
    {
        return await _context.Reviews.ToListAsync();
    }
}
