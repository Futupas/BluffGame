using BluffGame.Models;
using BluffGame.Pages;
using BluffGame.UnitTests.Models;

namespace BluffGame.UnitTests;

public class GameplayTest
{
    //todo hints 
    
    private readonly Random random = new ();
    private bool RandomBool() => random.NextDouble() >= .5;
    
    [Theory]
    [InlineData(2, 2)]
    [InlineData(3, 5)]
    [InlineData(10, 10)]
    [InlineData(32, 128)]
    public void TestGuessRate(int playersCount, int roundsCount)
    {
        var game = new Game();

        var data = new Dictionary<string, StatisticsRow>();

        // Initialize game
        InitializeGame(playersCount, game, data);

        Assert.Equal(playersCount, game.Users.Count);

        var wishes = new Dictionary<string, bool>();
        PlayRounds(roundsCount, game, data, wishes);
        
        // Checks
        foreach (var (username, stats) in data)
        {
            var guessRate = game.GetUserGuessRate(username);
            Assert.Equal(stats.GetGuessRate(), guessRate, 3);
        }
    }

    private static void InitializeGame(int playersCount, Game game, Dictionary<string, StatisticsRow> data)
    {
        for (int i = 0; i < playersCount; i++)
        {
            var gamePage = i == 0
                ? new GamePage() { GameId = game.Id.ToString(), CreatorGuid = game.CreatorGuid.ToString() }
                : new GamePage() { GameId = game.Id.ToString() };

            var username = "player" + i;

            data[username] = new();
            game.Users[username] = gamePage;
        }

        game.NewRound();
        game.Status = GameStatus.Playing;
    }
    private void PlayRounds(int roundsCount, Game game, Dictionary<string, StatisticsRow> data, Dictionary<string, bool> wishes)
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
                    data[username].All++;
                    if (wishes[opponent] == myValue)
                    {
                        data[username].Guessed++;
                    }
                }
            }
            
            // Everyone asks
            foreach (var (username, value) in game.Users)
            {
                var wish = RandomBool();
                wishes[username] = wish;
                game.WishValue(username, wish, null);
            }

            game.NewRound();
        }
    }
}

