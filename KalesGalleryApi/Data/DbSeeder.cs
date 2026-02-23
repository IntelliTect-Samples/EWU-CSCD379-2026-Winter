using KalesGalleryApi.Models;
using KalesGalleryApi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KalesGalleryApi.Data;

public static class DbSeeder
{
    public const string AdminRole = "Admin";

    public const string AdminEmail = "admin@kalesgallery.com";
    public const string AdminPassword = "Admin123";

    // Seed image folder path
    private const string ImageFolder = @"C:\Users\billm\School\CSharpWebApp\Images";

    public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Create Admin role
        if (!await roleManager.RoleExistsAsync(AdminRole))
        {
            await roleManager.CreateAsync(new IdentityRole(AdminRole));
        }

        // Create admin user
        var adminUser = await userManager.FindByEmailAsync(AdminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = AdminEmail,
                Email = AdminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, AdminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, AdminRole);
            }
        }

        // Seed test users, art pieces, and commissions
        var context = serviceProvider.GetRequiredService<GalleryDbContext>();
        var blobService = serviceProvider.GetRequiredService<IBlobStorageService>();

        await SeedUsersAsync(userManager);
        await SeedArtPiecesAsync(context, blobService);
        await SeedCommissionsAsync(context, userManager);
    }

    private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
    {
        var testUsers = new[]
        {
            ("alice@test.com", "Alice123"),
            ("bob@test.com", "Bob12345"),
            ("carol@test.com", "Carol123"),
        };

        foreach (var (email, password) in testUsers)
        {
            if (await userManager.FindByEmailAsync(email) == null)
            {
                var user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(user, password);
            }
        }
    }

    private static async Task SeedArtPiecesAsync(GalleryDbContext context, IBlobStorageService blobService)
    {
        if (await context.ArtPieces.AnyAsync()) return;

        var artData = new[]
        {
            ("Sunset Over the Valley", "A vibrant oil painting depicting a golden sunset over rolling hills.", 250.00m),
            ("White Ink Blossom", "Delicate white ink flowers on a dark canvas, blending modern and traditional styles.", 180.00m),
            ("Abstract Dreamscape", "Bold colors and sweeping strokes create an immersive abstract landscape.", 320.00m),
            ("Quiet Morning", "A serene photograph capturing early morning light on a still lake.", 95.00m),
            ("Coastal Breeze", "Soft watercolor tones evoke the feeling of a warm day at the shore.", 150.00m),
            ("Urban Mosaic", "A vivid mixed-media piece inspired by city life and street art.", 210.00m),
        };

        var imageFiles = Directory.GetFiles(ImageFolder)
            .Where(f => f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                      || f.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
                      || f.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            .OrderBy(f => f)
            .ToArray();

        for (int i = 0; i < Math.Min(artData.Length, imageFiles.Length); i++)
        {
            var (name, description, price) = artData[i];
            var filePath = imageFiles[i];
            var ext = Path.GetExtension(filePath);
            var blobName = $"{Guid.NewGuid()}{ext}";
            var contentType = ext.ToLower() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream"
            };

            await using var stream = File.OpenRead(filePath);
            var imageUrl = await blobService.UploadAsync(stream, blobName, contentType);

            context.ArtPieces.Add(new ArtPiece
            {
                Name = name,
                Description = description,
                Price = price,
                IsAvailable = true,
                ImageUrl = imageUrl
            });
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedCommissionsAsync(GalleryDbContext context, UserManager<ApplicationUser> userManager)
    {
        if (await context.Commissions.AnyAsync()) return;

        var alice = await userManager.FindByEmailAsync("alice@test.com");
        var bob = await userManager.FindByEmailAsync("bob@test.com");
        var carol = await userManager.FindByEmailAsync("carol@test.com");

        if (alice == null || bob == null || carol == null) return;

        var commissions = new[]
        {
            new Commission { UserId = alice.Id, Name = "Family Portrait", Description = "Digital portrait of a family of four", TypeId = 1, Price = 45.00m, IsCompleted = false },
            new Commission { UserId = alice.Id, Name = "Pet Painting", Description = "Traditional watercolor of a golden retriever", TypeId = 2, Price = 30.00m, IsCompleted = true },
            new Commission { UserId = bob.Id, Name = "Beaded Necklace Design", Description = "Custom beadwork pattern for a necklace", TypeId = 3, Price = 60.00m, IsCompleted = false },
            new Commission { UserId = bob.Id, Name = "Album Cover Art", Description = "Digital illustration for an indie album", TypeId = 1, Price = 50.00m, IsCompleted = false },
            new Commission { UserId = carol.Id, Name = "Wedding Invitation Art", Description = "Traditional floral border design", TypeId = 2, Price = 25.00m, IsCompleted = true },
            new Commission { UserId = carol.Id, Name = "Beaded Earring Set", Description = "Matching beadwork earrings with turquoise accents", TypeId = 3, Price = 40.00m, IsCompleted = false },
        };

        context.Commissions.AddRange(commissions);
        await context.SaveChangesAsync();
    }
}
