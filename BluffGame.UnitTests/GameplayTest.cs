using BluffGame.Models;
using BluffGame.Pages;
using BluffGame.UnitTests.Models;

namespace BluffGame.UnitTests;

public class GameplayTest
{
    private const int DOUBLE_COMPARSION_TOLERANCE = 4; // 4 digits after delimiter
    
    private readonly Random random = new ();
    private bool RandomBool() => random.NextDouble() >= .5;
    private bool? RandomNullBool() => random.Next(3) switch
    {
        0 => true,
        1 => false,
        _ => null
    };
    
    [Theory]
    [InlineData(2, 2)]
    [InlineData(3, 5)]
    [InlineData(10, 10)]
    [InlineData(32, 128)]
    public void TestGuessRateAndLieRate(int playersCount, int roundsCount)
    {
        var game = new Game(CONFIG);

        var guessStats = new Dictionary<string, StatsRow>();
        var lieStats = new Dictionary<string, StatsRow>();

        // Initialize game
        InitializeGame(playersCount, game, guessStats, lieStats);

        Assert.Equal(playersCount, game.Users.Count);

        var wishes = new Dictionary<string, bool>();
        PlayRounds(roundsCount, game, guessStats, lieStats, wishes);
        
        // Checks (guess)
        foreach (var (username, stats) in guessStats)
        {
            var guessRate = game.GetUserGuessRate(username);
            Assert.Equal(stats.GetRate(), guessRate, DOUBLE_COMPARSION_TOLERANCE);
        }
        
        // Checks (lie)
        foreach (var (username, stats) in lieStats)
        {
            var lieRate = game.GetUserLieRate(username);
            Assert.Equal(stats.GetRate(), lieRate, DOUBLE_COMPARSION_TOLERANCE);
        }
    }

    private static void InitializeGame(int playersCount, Game game, Dictionary<string, StatsRow> guessStats, Dictionary<string, StatsRow> lieStats)
    {
        for (int i = 0; i < playersCount; i++)
        {
            var gamePage = i == 0
                ? new GamePage() { GameId = game.Id.ToString(), CreatorGuid = game.CreatorGuid.ToString() }
                : new GamePage() { GameId = game.Id.ToString() };

            var username = "player" + i;

            guessStats[username] = new();
            lieStats[username] = new();
            game.Users[username] = gamePage;
        }

        game.NewRound();
        game.Status = GameStatus.Playing;
    }
    private void PlayRounds(int roundsCount, Game game, Dictionary<string, StatsRow> guessStats, Dictionary<string, StatsRow> lieStats, Dictionary<string, bool> wishes)
    {
        for (int i = 0; i < roundsCount; i++)
        {
            if (i > 0)
            {
                // Everyone answers
                foreach (var (username, value) in game.Users)
                {
                    var opponent = game.WhosValueAmIGuessing(username)!;
                    var myValue = RandomBool();
                    game.AnswerValue(username, myValue);
                    
                    
                    // Add values
                    guessStats[username].All++;
                    if (wishes[opponent] == myValue)
                    {
                        guessStats[username].True++;
                    }
                }
            }
            
            // Everyone asks
            foreach (var (username, value) in game.Users)
            {
                var wish = RandomBool();
                wishes[username] = wish;
                var hint = RandomNullBool();
                game.WishValue(username, wish, hint);

                lieStats[username].All++;
                if (hint is not null && hint.Value != wish)
                {
                    lieStats[username].True++;
                }
            }

            game.NewRound();
        }
    }
}

