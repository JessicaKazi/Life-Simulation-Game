try
{
    using var game = new LifeGame.LifeGame();
    game.Run();
}
catch (System.Exception ex)
{
    System.IO.File.WriteAllText("crash.log", ex.ToString());
    System.Console.Error.WriteLine(ex);
    throw;
}