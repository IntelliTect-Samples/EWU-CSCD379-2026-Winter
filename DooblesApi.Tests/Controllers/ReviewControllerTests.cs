using DooblesApi.Controllers;
using DooblesApi.Models;
using DooblesApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace DooblesApi.Tests.Controllers;

public class ReviewControllerTests : TestBase
{
	private readonly ReviewController _controller;
	private readonly IReviewService _service;

	public ReviewControllerTests()
	{
		_service = new ReviewService(_context);
		_controller = new ReviewController(_service);
	}

	[Fact]
	public async Task PostReview_ReturnsCreatedReview_WhenValid()
	{
		// Arrange
		var review = new Review
		{
			Stars = 5,
			ReviewText = "Great game!",
			Reviewer = "TestUser"
		};

		// Act
		var result = await _controller.PostReview(review);

		// Assert
		var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
		var createdReview = Assert.IsType<Review>(createdResult.Value);
		Assert.Equal(5, createdReview.Stars);
		Assert.Equal("Great game!", createdReview.ReviewText);
		Assert.Equal("TestUser", createdReview.Reviewer);
		Assert.True(createdReview.Id > 0);
	}

	[Fact]
	public async Task PostReview_ReturnsBadRequest_WhenStarsTooLow()
	{
		// Arrange
		var review = new Review
		{
			Stars = 0,
			ReviewText = "Bad review",
			Reviewer = "TestUser"
		};

		// Act
		var result = await _controller.PostReview(review);

		// Assert
		Assert.IsType<BadRequestObjectResult>(result.Result);
	}

	[Fact]
	public async Task PostReview_ReturnsBadRequest_WhenStarsTooHigh()
	{
		// Arrange
		var review = new Review
		{
			Stars = 6,
			ReviewText = "Too many stars",
			Reviewer = "TestUser"
		};

		// Act
		var result = await _controller.PostReview(review);

		// Assert
		Assert.IsType<BadRequestObjectResult>(result.Result);
	}

	[Fact]
	public async Task GetReview_ReturnsReview_WhenExists()
	{
		// Arrange
		var review = new Review
		{
			Stars = 4,
			ReviewText = "Good game",
			Reviewer = "TestUser"
		};
		_context.Reviews.Add(review);
		await _context.SaveChangesAsync();

		// Act
		var result = await _controller.GetReview(review.Id);

		// Assert
		var returnedReview = Assert.IsType<Review>(result.Value);
		Assert.Equal(4, returnedReview.Stars);
		Assert.Equal("Good game", returnedReview.ReviewText);
	}

	[Fact]
	public async Task GetReview_ReturnsNotFound_WhenDoesNotExist()
	{
		// Act
		var result = await _controller.GetReview(999);

		// Assert
		Assert.IsType<NotFoundResult>(result.Result);
	}

	[Fact]
	public async Task GetAllReviews_ReturnsAllReviews()
	{
		// Arrange
		_context.Reviews.AddRange(
			new Review { Stars = 5, ReviewText = "Excellent!", Reviewer = "User1" },
			new Review { Stars = 3, ReviewText = "Okay", Reviewer = "User2" },
			new Review { Stars = 1, ReviewText = "Bad", Reviewer = "User3" }
		);
		await _context.SaveChangesAsync();

		// Act
		var result = await _controller.GetAllReviews();

		// Assert
		var okResult = Assert.IsType<OkObjectResult>(result.Result);
		var reviews = Assert.IsType<List<Review>>(okResult.Value);
		Assert.Equal(3, reviews.Count);
	}

	[Theory]
	[InlineData(1)]
	[InlineData(2)]
	[InlineData(3)]
	[InlineData(4)]
	[InlineData(5)]
	public async Task PostReview_AcceptsValidStarRatings(int stars)
	{
		// Arrange
		var review = new Review
		{
			Stars = stars,
			ReviewText = $"Review with {stars} stars",
			Reviewer = "TestUser"
		};

		// Act
		var result = await _controller.PostReview(review);

		// Assert
		var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
		var createdReview = Assert.IsType<Review>(createdResult.Value);
		Assert.Equal(stars, createdReview.Stars);
	}
}
