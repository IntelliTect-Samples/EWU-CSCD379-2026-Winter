namespace Wordle.Api.Models
{
    public class WordOfTheDayResponse
    {
        public string WordOfTheDay { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Insult { get; set; } = string.Empty;
    }
}
