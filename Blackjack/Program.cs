﻿internal class Program
{
    private static void Main(string[] args)
    {
        Player player = new Player("player_one", 1000);
        Game game = new Game(player, 1);

        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.CursorVisible = false;

        game.DrawHeader();
        game.StartGameLoop();
    }
}