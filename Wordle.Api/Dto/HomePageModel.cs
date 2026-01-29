namespace Wordle.Api.Dto
{
    public class HomePageModel
    {
        public string Title { get; set; } = "Welcome to Wordle API";
        public int Counter { get; set; } = 5;
        public string Name { get; set; } = "Wordle User";
    }
}
