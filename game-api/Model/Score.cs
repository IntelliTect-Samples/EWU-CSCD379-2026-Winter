namespace game_api.Model {
    public class Score {
        public int Id { get; set; } 
        public string PlayerName { get; set; } = string.Empty;
        public double Time { get; set; }
        public string Difficulty { get; set; } = "Medium";
        public DateTime DateAchieved { get; set; } = DateTime.UtcNow;
        
    }
}