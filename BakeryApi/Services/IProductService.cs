using BakeryApi.Models;
using Microsoft.AspNetCore.Http;

namespace BakeryApi.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetAllAsync();
        Task<Product> AddAsync(Product product);
        Task<bool> DeleteAsync(int id);
        Task<string?> UploadImageAsync(IFormFile file, HttpRequest request);
    }
}