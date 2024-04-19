internal class Hand
{
    private List<Card> _cards = new List<Card>();
    private bool _stand = false;
    private int _bet;

    internal bool Stand { get => _stand; set => _stand = value; }
    internal int Bet { get => _bet; set => _bet = value; }
    internal List<Card> Cards { get => _cards; }

    internal Hand()
    {
        _bet = 0;
    }

    internal Hand(int bet)
    {
        _bet = bet;
    }

    internal Hand(Card card, int bet)
    {
        _cards.Add(card);
        _bet = bet;
    }

    internal void AddCard(Card card)
    {
        _cards.Add(card);
    }

    internal void DrawHand()
    {
        foreach (Card card in Cards)
        {
            card.DrawCard();
            Console.Write(" ");
        }
        Console.Write("\b");
    }

    internal int CurrentTotal()
    {
        int total = 0;
        int noOfAces = 0;

        foreach (Card card in Cards)
        {
            if (card.Value > 9)
            {
                total += 10;
            }
            else if (card.Value == 1)
            {
                noOfAces++;
            }
            else
            {
                total += card.Value;
            }
        }

        if (noOfAces > 1)
        {
            if (total + 11 + noOfAces - 1 < 22)
            {
                total += 11 + noOfAces - 1;
            }
            else
            {
                total += noOfAces;
            }
        }
        else if (noOfAces == 1)
        {
            if (total + 11 < 22)
            {
                total += 11;
            }
            else
            {
                total += 1;
            }
        }

        return total;
    }

    internal int LowTotal()
    {
        int total = 0;

        foreach (Card card in Cards)
        {
            if (card.Value > 9)
            {
                total += 10;
            }
            else
            {
                total += card.Value;
            }
        }

        return total;
    }

    internal void Wipe(List<Card> discardPile)
    {
        discardPile.AddRange(Cards);
        Cards.Clear();
    }

    internal bool IsBlackjack()
    {
        if (Cards.Count == 2)
        {
            if (CurrentTotal() == 21)
            {
                return true;
            }
        }

        return false;
    }

    internal bool IsSplitPossible(Player player)
    {
        if (Cards.Count == 2 && Cards[0].Value == Cards[1].Value && player.Balance >= _bet)
        {
            return true;
        }

        return false;
    }

    internal bool Split(Player player)
    {
        if (IsSplitPossible(player))
        {
            player.AddNewHand(Cards[1], _bet);
            Cards.Remove(Cards[1]);
            player.Balance -= _bet;
            return true;
        }
        return false;
    }
}