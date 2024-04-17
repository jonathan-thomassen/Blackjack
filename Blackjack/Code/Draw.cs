using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackjack.Code
{
    internal static class Draw
    {
        internal static void DrawDealerHand(Dealer dealer, bool mock = false)
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

        internal static void DrawPlayerHand(Player player, Hand hand, int handNo, bool mock = false)
        {
            if (!mock)
            {
                if (player.Hands.Count > 1)
                {
                    Console.Write($"Your hand #{handNo}: ");
                    hand.DrawHand();
                    Console.Write($"Your hand #{handNo} total: {hand.CurrentTotal()}");
                }
                else
                {
                    Console.Write("│                          ");
                    Console.Write($"Your hand: ");
                    hand.DrawHand();

                    for (int i = 0; i < 42 - hand.Cards.Count * 3; i++)
                    {
                        Console.Write(" ");
                    }

                    foreach (Card card in hand.Cards)
                    {
                        if (card.Value == 10)
                        {
                            Console.Write("\b");
                        }
                    }

                    Console.Write("│\n");
                    Console.Write("│                          ");
                    Console.Write($"Your total: {hand.CurrentTotal()}");
                    for (int i = 0; i < 40 - hand.CurrentTotal().ToString().Length; i++)
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

        internal static void DrawPlayfield(Player player, Dealer dealer, List<Card> deck, Hand hand, int handNo)
        {

            DrawBalance(player,deck);
            DrawEmptyLine();
            DrawDealerHand(dealer);
            DrawEmptyLine();
            DrawPlayerHand(player, hand, handNo);
            DrawEmptyLine();
            DrawBet(player);
            DrawEmptyLine();
            DrawEmptyLine();
        }

        internal static void DrawBet(Player player, bool mock = false)
        {
            string text = $"Your bet: ";
            if (!mock)
            {
                text += player.Hands[0].Bet + "$";
                if (player.InsuranceTaken != null)
                {
                    if ((bool)player.InsuranceTaken)
                    {
                        text += " Insurance taken.";
                    }
                    else
                    {
                        text += " Insurance not taken.";
                    }
                }
            }

            Console.Write("│                          ");
            Console.Write(text);
            for (int i = 0; i < 52 - text.Length; i++)
            {
                Console.Write(" ");
            }
            Console.Write("│\n");
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
