namespace DooblesApi.Services;

public interface IDoobleService
{
    Task<string?> GetRandomDoobleNameAsync();
    Task<List<string>> GetAllNamesAsync();
}
