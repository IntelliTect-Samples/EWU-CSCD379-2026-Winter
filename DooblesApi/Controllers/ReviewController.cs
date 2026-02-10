using Microsoft.AspNetCore.Mvc;
using DooblesApi.Models;
using DooblesApi.Services;

namespace DooblesApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpPost]
    public async Task<ActionResult<Review>> PostReview([FromBody] Review review)
    {
        // Validate stars are between 1 and 5
        if (review.Stars < 1 || review.Stars > 5)
        {
            return BadRequest("Stars must be between 1 and 5");
        }

        var createdReview = await _reviewService.CreateReviewAsync(review);
        return CreatedAtAction(nameof(GetReview), new { id = createdReview.Id }, createdReview);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Review>> GetReview(int id)
    {
        var review = await _reviewService.GetReviewByIdAsync(id);

        if (review == null)
        {
            return NotFound();
        }

        return review;
    }

    [HttpGet]
    public async Task<ActionResult<List<Review>>> GetAllReviews()
    {
        var reviews = await _reviewService.GetAllReviewsAsync();
        return Ok(reviews);
    }
}
