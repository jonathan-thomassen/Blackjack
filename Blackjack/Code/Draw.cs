using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Blackjack.Code
{
    internal static class Draw
    {
        private static void DrawDealerHand(Dealer dealer, bool mock = false)
        {
            DrawEmptyLine();
            Console.Write("│                          ");
            Console.Write("Dealer's hand: ");
            if (!mock)
            {
                dealer.DrawHand();

                for (int i = 0; i < 38 - dealer.Hand.Cards.Count * 3; i++)
                {
                    Console.Write(" ");
                }

                foreach (Card card in dealer.Hand.Cards)
                {
                    if (card.Value == 10)
                    {
                        Console.Write("\b");
                    }
                }
            }
            else
            {
                for (int i = 0; i < 37; i++)
                {
                    Console.Write(" ");
                }
            }

            Console.Write("│\n");
            Console.Write("│                          ");
            Console.Write("Dealer's total: ");
            if (!mock)
            {
                Console.Write(dealer.Hand.CurrentTotal());

                for (int i = 0; i < 36 - dealer.Hand.CurrentTotal().ToString().Length; i++)
                {
                    Console.Write(" ");
                }
            }
            else
            {
                for (int i = 0; i < 36; i++)
                {
                    Console.Write(" ");
                }
            }
            Console.Write("│\n");
        }

        private static void DrawPlayerHands(Player player, bool mock = false)
        {
            if (!mock)
            {
                if (player.Hands.Count == 1)
                {
                    Console.Write("│                          ");
                    Console.Write($"Your hand: ");
                    player.Hands[0].DrawHand();

                    for (int i = 0; i < 42 - player.Hands[0].Cards.Count * 3; i++)
                    {
                        Console.Write(" ");
                    }

                    foreach (Card card in player.Hands[0].Cards)
                    {
                        if (card.Value == 10)
                        {
                            Console.Write("\b");
                        }
                    }
                    Console.Write("│\n");
                    Console.Write("│                          ");
                    Console.Write($"Your total: {player.Hands[0].CurrentTotal()}");
                    for (int i = 0; i < 40 - player.Hands[0].CurrentTotal().ToString().Length; i++)
                    {
                        Console.Write(" ");
                    }
                    Console.Write("│\n");

                    Console.Write("│                          ");
                    Console.Write($"Your bet: ${player.Hands[0].Bet}");
                    for (int i = 0; i < 41 - player.Hands[0].Bet.ToString().Length; i++)
                    {
                        Console.Write(" ");
                    }
                    Console.Write("│\n");
                }
                else if (player.Hands.Count == 2)
                {
                    Console.Write("│               ");
                    Console.Write($"Your hand #1: ");
                    player.Hands[0].DrawHand();
                    for (int i = 0; i < 20 - player.Hands[0].Cards.Count * 3; i++)
                    {
                        Console.Write(" ");
                    }
                    foreach (Card card in player.Hands[0].Cards)
                    {
                        if (card.Value == 10)
                        {
                            Console.Write("\b");
                        }
                    }
                    Console.Write($"Your hand #2: ");
                    player.Hands[1].DrawHand();
                    for (int i = 0; i < 17 - player.Hands[1].Cards.Count * 3; i++)
                    {
                        Console.Write(" ");
                    }
                    foreach (Card card in player.Hands[1].Cards)
                    {
                        if (card.Value == 10)
                        {
                            Console.Write("\b");
                        }
                    }
                    Console.Write("│\n");
                    Console.Write("│               ");
                    Console.Write($"Your total #1: {player.Hands[0].CurrentTotal()}");
                    Console.Write("                ");
                    Console.Write($"Your total #2: {player.Hands[1].CurrentTotal()}");
                    Console.Write("             ");
                    Console.Write("│\n");
                    Console.Write("│               ");
                    Console.Write($"Your bet #1: ${player.Hands[0].Bet}");
                    for (int i = 0; i < 19 - player.Hands[0].Bet.ToString().Length; i++)
                    {
                        Console.Write(" ");
                    }
                    Console.Write($"Your bet #2: ${player.Hands[1].Bet}");
                    for (int i = 0; i < 16 - player.Hands[1].Bet.ToString().Length; i++)
                    {
                        Console.Write(" ");
                    }
                    Console.Write("│\n");
                }
            }
            else
            {
                Console.WriteLine("│                          Your hand:                                          │");
                Console.WriteLine("│                          Your total:                                         │");
                Console.WriteLine("│                          Your bet:                                           │");
            }
        }

        internal static void DrawHeader()
        {
            Console.Clear();

            DrawSolidLine(0);

            Console.Write("│ ");
            Console.Write("C# Blackjack Alpha v0.001");
            for (int i = 0; i < 52; i++)
            {
                Console.Write(" ");
            }
            Console.Write("│\n");

            DrawSolidLine(1);
        }

        internal static void DrawBalance(Player player, List<Card> deck)
        {
            Console.Write("│ ");
            Console.Write("Balance: ");
            string balance = player.Balance.ToString();
            for (int i = 0; i < 5 - balance.Length; i++)
            {
                Console.Write(" ");
            }
            Console.Write("$" + balance);
            Console.Write("                                     ");
            Console.Write("# of Cards in deck: ");
            string deckSize = deck.Count.ToString();
            for (int i = 0; i < 4 - deckSize.Length; i++)
            {
                Console.Write(" ");
            }
            Console.Write(deckSize);
            Console.Write(" │\n");
        }

        internal static void DrawPlayfield(Player player, Dealer dealer, List<Card> deck, bool mock = false)
        {

            DrawBalance(player, deck);
            DrawEmptyLine();
            DrawDealerHand(dealer, mock);
            DrawEmptyLine();
            DrawEmptyLine();
            DrawPlayerHands(player, mock);
            DrawEmptyLine();
        }

        internal static void DrawEmptyLine()
        {
            Console.Write("│");
            for (int i = 0; i < 78; i++)
            {
                Console.Write(" ");
            }
            Console.Write("│\n");
        }

        internal static void DrawSolidLine(int location)
        {
            switch (location)
            {
                case 0:
                    {
                        Console.Write("┌");
                        break;
                    }
                case 1:
                    {
                        Console.Write("├");
                        break;
                    }
                case 2:
                    {
                        Console.Write("└");
                        break;
                    }
            }

            for (int i = 0; i < 78; i++)
            {
                Console.Write("─");
            }

            switch (location)
            {
                case 0:
                    {
                        Console.Write("┐");
                        break;
                    }
                case 1:
                    {
                        Console.Write("┤");
                        break;
                    }
                case 2:
                    {
                        Console.Write("┘");
                        break;
                    }
            }

            Console.Write("\n");
        }

        internal static void DrawString(string text)
        {
            Console.Write("│");
            for (int i = 0; i < 39 - (text.Length / 2); i++)
            {
                Console.Write(" ");
            }
            Console.Write(text);
            for (int i = 0; i < 38 - (text.Length / 2); i++)
            {
                Console.Write(" ");
            }
            if (text.Length % 2 == 0)
            {
                Console.Write(" ");
            }
            Console.Write("│\n");
        }
    }
}
