using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wordle.Api.Data;
using Wordle.Api.Models;

namespace Wordle.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WordleController : ControllerBase
    {
        private readonly AppDbContext _context;
        public WordleController(AppDbContext context)
        {
            _context = context;
        }

        private static readonly Random Random = new Random();

        [HttpGet("wordoftheday")]
        public ActionResult<WordOfTheDayResponse> GetWordOfTheDay()
        {
            int wordIndex = Random.Next(FiveLetterWords.Length);
            int insultIndex = Random.Next(Insults.Length);
            
            var response = new WordOfTheDayResponse
            {
                WordOfTheDay = FiveLetterWords[wordIndex],
                Date = DateTime.UtcNow,
                Insult = Insults[insultIndex]
            };
            
            return Ok(response);
        }

        [HttpGet("randomword")]
        public async Task<ActionResult<string>> GetRandomWord()
        {
            int count = await _context.Words.CountAsync();

            int wordIndex = Random.Next(count);
            Word word = await _context.Words.Skip(wordIndex).FirstAsync();
            return Ok(word.Text);
        }

        private static readonly string[] FiveLetterWords = 
        {
            "about", "above", "abuse", "actor", "acute", "admit", "adopt", "adult", "after", "again",
            "agent", "agree", "ahead", "alarm", "album", "alert", "align", "alike", "alive", "allow",
            "alone", "along", "alter", "amber", "amuse", "angel", "anger", "angle", "angry", "apart",
            "apple", "apply", "arena", "argue", "arise", "array", "aside", "asset", "audio", "audit",
            "avoid", "award", "aware", "badly", "baker", "bases", "basic", "basis", "beach", "began",
            "begin", "begun", "being", "below", "bench", "billy", "birth", "black", "blame", "blind",
            "block", "blood", "board", "boost", "booth", "bound", "brain", "brand", "brave", "bread",
            "break", "breed", "brief", "bring", "broad", "broke", "brown", "build", "built", "buyer",
            "cable", "calif", "carry", "catch", "cause", "chain", "chair", "chart", "chase", "cheap",
            "check", "chest", "chief", "child", "china", "chose", "civil", "claim", "class", "clean",
            "clear", "click", "clock", "close", "coach", "coast", "could", "count", "court", "cover",
            "craft", "crash", "crazy", "cream", "crime", "cross", "crowd", "crown", "crude", "cycle",
            "daily", "dance", "dated", "dealt", "death", "debut", "delay", "depth", "doing", "doubt",
            "dozen", "draft", "drama", "drank", "drawn", "dream", "dress", "drill", "drink", "drive",
            "drove", "dying", "eager", "early", "earth", "eight", "elite", "empty", "enemy", "enjoy",
            "enter", "entry", "equal", "error", "event", "every", "exact", "exist", "extra", "faith",
            "false", "fault", "fiber", "field", "fifth", "fifty", "fight", "final", "first", "fixed",
            "flash", "fleet", "floor", "fluid", "focus", "force", "forth", "forty", "forum", "found",
            "frame", "frank", "fraud", "fresh", "front", "fruit", "fully", "funny", "giant", "given",
            "glass", "globe", "going", "grace", "grade", "grand", "grant", "grass", "great", "green",
            "gross", "group", "grown", "guard", "guess", "guest", "guide", "happy", "harry", "heart",
            "heavy", "hence", "henry", "horse", "hotel", "house", "human", "ideal", "image", "imply",
            "index", "inner", "input", "issue", "japan", "jimmy", "joint", "jones", "judge", "known",
            "label", "large", "laser", "later", "laugh", "layer", "learn", "lease", "least", "leave",
            "legal", "lemon", "level", "lewis", "light", "limit", "links", "lives", "local", "logic",
            "loose", "lower", "lucky", "lunch", "lying", "magic", "major", "maker", "march", "maria",
            "match", "maybe", "mayor", "meant", "media", "metal", "might", "minor", "minus", "mixed",
            "model", "money", "month", "moral", "motor", "mount", "mouse", "mouth", "movie", "music",
            "needs", "never", "newly", "night", "noise", "north", "noted", "novel", "nurse", "occur",
            "ocean", "offer", "often", "order", "other", "ought", "paint", "panel", "paper", "party",
            "peace", "peter", "phase", "phone", "photo", "piece", "pilot", "pitch", "place", "plain",
            "plane", "plant", "plate", "point", "pound", "power", "press", "price", "pride", "prime",
            "print", "prior", "prize", "proof", "proud", "prove", "queen", "quick", "quiet", "quite",
            "radio", "raise", "range", "rapid", "ratio", "reach", "ready", "refer", "right", "rival",
            "river", "robin", "roger", "roman", "rough", "round", "route", "royal", "rural", "scale",
            "scene", "scope", "score", "sense", "serve", "seven", "shall", "shape", "share", "sharp",
            "sheet", "shelf", "shell", "shift", "shine", "shirt", "shock", "shoot", "short", "shown",
            "sight", "since", "sixth", "sixty", "sized", "skill", "sleep", "slide", "small", "smart",
            "smile", "smith", "smoke", "solid", "solve", "sorry", "sound", "south", "space", "spare",
            "speak", "speed", "spend", "spent", "split", "spoke", "sport", "staff", "stage", "stake",
            "stand", "start", "state", "steam", "steel", "stick", "still", "stock", "stone", "stood",
            "store", "storm", "story", "strip", "stuck", "study", "stuff", "style", "sugar", "suite",
            "super", "sweet", "table", "taken", "taste", "taxes", "teach", "terry", "texas", "thank",
            "theft", "their", "theme", "there", "these", "thick", "thing", "think", "third", "those",
            "three", "threw", "throw", "tight", "times", "title", "today", "topic", "total", "touch",
            "tough", "tower", "track", "trade", "train", "trait", "treat", "trend", "trial", "tribe",
            "trick", "tried", "tries", "truck", "truly", "trust", "truth", "twice", "under", "undue",
            "union", "unity", "until", "upper", "upset", "urban", "usage", "usual", "valid", "value",
            "video", "virus", "visit", "vital", "vocal", "voice", "waste", "watch", "water", "wheel",
            "where", "which", "while", "white", "whole", "whose", "woman", "women", "world", "worry",
            "worse", "worst", "worth", "would", "wound", "write", "wrong", "wrote", "yield", "young",
            "youth"
        };

        private static readonly string[] Insults =
        {
            "Perhaps word puzzles aren't your forte.",
            "Even a broken clock is right twice a day... you're not.",
            "I've seen better guesses from a random number generator.",
            "Your vocabulary needs some serious work.",
            "Did you even try?",
            "Maybe stick to tic-tac-toe.",
            "That was... impressively wrong.",
            "I'm not mad, just disappointed.",
            "Better luck next time, champ.",
            "Is English your first language?",
            "You might want to read a book sometime.",
            "My grandmother could guess better than that.",
            "Do you need a dictionary?",
            "That's embarrassing.",
            "Please tell me you weren't trying your best.",
            "I've seen toddlers with better word skills.",
            "Maybe words just aren't your thing.",
            "Are you even paying attention?",
            "That's not even close to correct.",
            "You should probably give up now."
        };
    }
}
