using BluffGame.Pages;

namespace BluffGame.UnitTests;

public class BasicTest
{
    private void RemoveGame(Game game)
    {
        // Assert.True(Game.Games.Remove(game.Id));
        Game.Games.Remove(game.Id);
    }

    [Fact]
    public void AssertCreateGame()
    {
        var game = new Game(CONFIG);
        Assert.True(Game.Games.ContainsKey(game.Id));
        RemoveGame(game);
    }

    [Fact]
    public void AssertAddUsers()
    {
        var game = new Game(CONFIG);
        game.Users.Add("user1", new GamePage { GameId = game.Id, CreatorGuid = game.CreatorGuid.ToString() });
        game.Users.Add("user2", new GamePage { GameId = game.Id, CreatorGuid = game.CreatorGuid.ToString() });
        Assert.Equal(2, game.Users.Count);
        RemoveGame(game);
    }

    [Fact]
    public void AssertNewRound()
    {
        var game = new Game(CONFIG);
        game.Users.Add("user1", new GamePage { GameId = game.Id, CreatorGuid = game.CreatorGuid.ToString() });
        game.Users.Add("user2", new GamePage { GameId = game.Id, CreatorGuid = game.CreatorGuid.ToString() });
        var round = game.NewRound();
        Assert.Single(game.Rounds);
        Assert.Equal(2, round.Couples.Count);
        RemoveGame(game);
    }

    [Fact]
    public void AssertWishValue()
    {
        var game = new Game(CONFIG);
        game.Users.Add("user1", new GamePage { GameId = game.Id, CreatorGuid = game.CreatorGuid.ToString() });
        game.Users.Add("user2", new GamePage { GameId = game.Id, CreatorGuid = game.CreatorGuid.ToString() });
        game.NewRound();
        game.WishValue("user1", true, null);
        var lastRound = game.Rounds.Last();
        var couple = lastRound.Couples.First(x => x.UserAsks == "user1");
        Assert.True(couple.Wished);
        RemoveGame(game);
    }

    [Fact]
    public void AssertAnswerValue()
    {
        var game = new Game(CONFIG);
        game.Users.Add("user1", new GamePage { GameId = game.Id, CreatorGuid = game.CreatorGuid.ToString() });
        game.Users.Add("user2", new GamePage { GameId = game.Id, CreatorGuid = game.CreatorGuid.ToString() });
        game.NewRound();
        game.NewRound();  // Need at least two rounds for answering
        game.AnswerValue("user2", false);
        var round = game.Rounds[^2];
        var couple = round.Couples.First(x => x.UserAnswers == "user2");
        Assert.False(couple.Answered);
        RemoveGame(game);
    }

    // [Fact]
    // public void AssertGetUserGuessRate()
    // {
    //     var game = new Game();
    //     game.Users.Add("user1", new GamePage { GameId = game.Id.ToString(), CreatorGuid = game.CreatorGuid.ToString() });
    //     game.Users.Add("user2", new GamePage { GameId = game.Id.ToString(), CreatorGuid = game.CreatorGuid.ToString() });
    //     var round = game.NewRound();
    //     round.Couples.First().Guessed = true;
    //     round.IsArchived = true;
    //     Assert.Equal(1.0, game.GetUserGuessRate("user2"));
    //     RemoveGame(game);
    // }

    // [Fact]
    // public void AssertGetUserLieRate()
    // {
    //     var game = new Game();
    //     game.Users.Add("user1", new GamePage { GameId = game.Id.ToString(), CreatorGuid = game.CreatorGuid.ToString() });
    //     game.Users.Add("user2", new GamePage { GameId = game.Id.ToString(), CreatorGuid = game.CreatorGuid.ToString() });
    //     var round = game.NewRound();
    //     round.Couples.First().Lied = true;
    //     round.IsArchived = true;
    //     Assert.Equal(1.0, game.GetUserLieRate("user2"));
    //     RemoveGame(game);
    // }

    // [Fact]
    // public void AssertHaveIWished()
    // {
    //     var game = new Game();
    //     game.Users.Add("user1", new GamePage { GameId = game.Id.ToString(), CreatorGuid = game.CreatorGuid.ToString() });
    //     game.Users.Add("user2", new GamePage { GameId = game.Id.ToString(), CreatorGuid = game.CreatorGuid.ToString() });
    //     var round = game.NewRound();
    //     round.Couples.First().UserAsks = "user1";
    //     round.Couples.First().Wished = true;
    //     Assert.True(game.HaveIWished("user1"));
    //     RemoveGame(game);
    // }

    // [Fact]
    // public void AssertHaveIAnswered()
    // {
    //     var game = new Game();
    //     game.Users.Add("user1", new GamePage { GameId = game.Id.ToString(), CreatorGuid = game.CreatorGuid.ToString() });
    //     game.Users.Add("user2", new GamePage { GameId = game.Id.ToString(), CreatorGuid = game.CreatorGuid.ToString() });
    //     game.NewRound();
    //     var round = game.NewRound();  // Need at least two rounds for answering
    //     round.Couples.First().UserAnswers = "user2";
    //     round.Couples.First().Answered = true;
    //     Assert.True(game.HaveIAnswered("user2"));
    //     RemoveGame(game);
    // }

    // [Fact]
    // public void AssertTryFindDidIWinInPreviousRound()
    // {
    //     var game = new Game();
    //     game.Users.Add("user1", new GamePage { GameId = game.Id.ToString(), CreatorGuid = game.CreatorGuid.ToString() });
    //     game.Users.Add("user2", new GamePage { GameId = game.Id.ToString(), CreatorGuid = game.CreatorGuid.ToString() });
    //     var round = game.NewRound();
    //     round.Couples.First().UserAnswers = "user2";
    //     round.Couples.First().Guessed = true;
    //     bool didIWin;
    //     Assert.True(game.TryFindDidIWinInPreviousRound("user2", out didIWin));
    //     Assert.True(didIWin);
    //     RemoveGame(game);
    // }

    // [Fact]
    // public void AssertWhosValueAmIGuessing()
    // {
    //     var game = new Game();
    //     game.Users.Add("user1", new GamePage { GameId = game.Id.ToString(), CreatorGuid = game.CreatorGuid.ToString() });
    //     game.Users.Add("user2", new GamePage { GameId = game.Id.ToString(), CreatorGuid = game.CreatorGuid.ToString() });
    //     game.NewRound();
    //     var round = game.NewRound();
    //     round.Couples.First().UserAnswers = "user2";
    //     round.Couples.First().UserAsks = "user1";
    //     Assert.Equal("user1", game.WhosValueAmIGuessing("user2"));
    //     RemoveGame(game);
    // }

    // [Fact]
    // public void AssertTryGetMyHint()
    // {
    //     var game = new Game();
    //     game.Users.Add("user1", new GamePage { GameId = game.Id.ToString(), CreatorGuid = game.CreatorGuid.ToString() });
    //     game.Users.Add("user2", new GamePage { GameId = game.Id.ToString(), CreatorGuid = game.CreatorGuid.ToString() });
    //     game.NewRound();
    //     var round = game.NewRound();
    //     round.Couples.First().UserAnswers = "user2";
    //     round.Couples.First().Hint = true;
    //     bool hint;
    //     Assert.True(game.TryGetMyHint("user2", out hint));
    //     Assert.True(hint);
    //     RemoveGame(game);
    // }

    // [Fact]
    // public void AssertWhoAmIGuessingFor()
    // {
    //     var game = new Game();
    //     game.Users.Add("user1", new GamePage { GameId = game.Id.ToString(), CreatorGuid = game.CreatorGuid.ToString() });
    //     game.Users.Add("user2", new GamePage { GameId = game.Id.ToString(), CreatorGuid = game.CreatorGuid.ToString() });
    //     var round = game.NewRound();
    //     round.Couples.First().UserAsks = "user1";
    //     round.Couples.First().UserAnswers = "user2";
    //     Assert.Equal("user2", game.WhoAmIGuessingFor("user1"));
    //     RemoveGame(game);
    // }

    // [Fact]
    // public void AssertCannotAddUserWithoutGameId()
    // {
    //     var game = new Game();
    //     var ex = Assert.Throws<ArgumentNullException>(() => game.Users.Add("user1", new GamePage { CreatorGuid = game.CreatorGuid.ToString() }));
    //     Assert.Equal("GameId", ex.ParamName);
    //     RemoveGame(game);
    // }

    // [Fact]
    // public void AssertCannotAddUserWithoutCreatorGuid()
    // {
    //     var game = new Game();
    //     var ex = Assert.Throws<ArgumentNullException>(() => game.Users.Add("user1", new GamePage { GameId = game.Id.ToString() }));
    //     Assert.Equal("CreatorGuid", ex.ParamName);
    //     RemoveGame(game);
    // }

    // [Fact]
    // public void AssertCannotJoinGameWithInvalidCreatorGuid()
    // {
    //     var game = new Game();
    //     var invalidCreatorGuid = Guid.NewGuid().ToString();
    //     game.Status = GameStatus.WaitingForCreator;
    //
    //     var page = new GamePage { GameId = game.Id.ToString(), CreatorGuid = invalidCreatorGuid };
    //
    //     page.OnInitialized();
    //     Assert.NotNull(page._exception);
    //     Assert.Equal("Seems like you are not the creator, you cannot join the game right now.", page._exception.Message);
    //     RemoveGame(game);
    // }
}
