namespace Wordle.Api.Models
{
    public class Word
    {
        public int WordId { get; set; }
        public required string Text { get; set; }
    }
}