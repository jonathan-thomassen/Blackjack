internal class Program
{
    private static void Main(string[] args)
    {
        Player player = new Player("player_one", 1000);
        Game game = new Game(player, 6, false);

        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.CursorVisible = false;
        Console.ForegroundColor = ConsoleColor.White;
        Console.BackgroundColor = ConsoleColor.DarkGreen;

        game.StartGameLoop();
    }
}