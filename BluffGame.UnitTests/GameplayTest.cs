using BluffGame.Models;
using BluffGame.Pages;
using BluffGame.UnitTests.Models;

namespace BluffGame.UnitTests;

public class GameplayTest
{
    private const int DOUBLE_COMPARSION_TOLERANCE = 4; // 4 digits after delimiter
    
    private readonly Random _random = new ();
    private bool RandomBool() => _random.NextDouble() >= .5;
    private bool? RandomNullBool() => _random.Next(3) switch
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
    [InlineData(8, 256)]
    [InlineData(8, 1024)]
    // [InlineData(7, 4097)]
    public void TestGuessRateAndLieRate(int playersCount, int roundsCount)
    {
        var game = new Game(CONFIG);

        var guessStats = new Dictionary<string, StatsRow>();
        var lieStats = new Dictionary<string, StatsRow>();
        var guessCoupleStats = new Dictionary<(string who, string toWhom), StatsRow>();
        var lieCoupleStats = new Dictionary<(string who, string toWhom), StatsRow>();

        // Initialize game
        InitializeGame(playersCount, game, guessStats, lieStats, guessCoupleStats, lieCoupleStats);

        Assert.Equal(playersCount, game.Users.Count);

        var wishes = new Dictionary<string, bool>();
        PlayRounds(roundsCount, game, guessStats, lieStats, guessCoupleStats, lieCoupleStats, wishes);
        
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

        var statistics = game.GetStatistics();
        if (statistics is null)
        {
            Assert.True(roundsCount < 2);
        }
        else
        {
            Assert.True(roundsCount >= 2);

            foreach (var user1 in game.Users.Keys)
            {
                // Guess rate
                Assert.Equal(guessStats[user1].GetRate(), statistics.GuessRates.Single(x => x.username == user1).rate, DOUBLE_COMPARSION_TOLERANCE);
                
                // Lie rate
                Assert.Equal(lieStats[user1].GetRate(), statistics.LieRates.Single(x => x.username == user1).rate, DOUBLE_COMPARSION_TOLERANCE);
                
                foreach (var user2 in game.Users.Keys.Where(user2 => user1 != user2))
                {
                    // couple guess rate
                    {
                        var expected = guessCoupleStats[(user1, user2)].GetRate();
                        var actual = statistics.GuessCouplesRates.Single(x => x.who == user1 && x.toWhom == user2).rate;
                        Assert.Equal(expected, actual, DOUBLE_COMPARSION_TOLERANCE);
                    }
                    
                    // couple lie rate
                    {
                        var expected = lieCoupleStats[(user1, user2)].GetRate();
                        var actual = statistics.LieCouplesRates.Single(x => x.who == user1 && x.toWhom == user2).rate;
                        Assert.Equal(expected, actual, DOUBLE_COMPARSION_TOLERANCE);
                    }
                }
            }
        }
    }

    private static void InitializeGame(
        int playersCount, 
        Game game, 
        Dictionary<string, StatsRow> guessStats, 
        Dictionary<string, StatsRow> lieStats,
        Dictionary<(string who, string toWhom), StatsRow> guessCoupleStats,
        Dictionary<(string who, string toWhom), StatsRow> lieCoupleStats
    ) {
        for (int i = 0; i < playersCount; i++)
        {
            var gamePage = i == 0
                ? new GamePage() { GameId = game.Id, CreatorGuid = game.CreatorGuid.ToString() }
                : new GamePage() { GameId = game.Id };

            var username = "player" + i;

            guessStats[username] = new();
            lieStats[username] = new();
            game.Users[username] = gamePage;
        }

        foreach (var user1 in game.Users.Keys)
        {
            foreach (var user2 in game.Users.Keys)
            {
                if (user1 == user2) continue;
                guessCoupleStats[(user1, user2)] = new();
                lieCoupleStats[(user1, user2)] = new();
            }
        }

        game.NewRound();
        game.Status = GameStatus.Playing;
    }
    private void PlayRounds(
        int roundsCount, 
        Game game, 
        Dictionary<string, StatsRow> guessStats, 
        Dictionary<string, StatsRow> lieStats, 
        Dictionary<(string who, string toWhom), StatsRow> guessCoupleStats,
        Dictionary<(string who, string toWhom), StatsRow> lieCoupleStats,
        Dictionary<string, bool> wishes
    ) {
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
                    guessCoupleStats[(username, opponent)].All++;
                    if (wishes[opponent] == myValue)
                    {
                        guessStats[username].True++;
                        guessCoupleStats[(username, opponent)].True++;
                    }
                }
            }
            
            // Everyone asks
            foreach (var (username, value) in game.Users)
            {
                var opponent = game.WhoAmIGuessingFor(username)!;
                var wish = RandomBool();
                wishes[username] = wish;
                var hint = RandomNullBool();
                game.WishValue(username, wish, hint);

                lieStats[username].All++;
                lieCoupleStats[(username, opponent)].All++;
                if (hint is not null && hint.Value != wish)
                {
                    lieStats[username].True++;
                    lieCoupleStats[(username, opponent)].True++;
                }
            }

            game.NewRound();
        }
    }
}

