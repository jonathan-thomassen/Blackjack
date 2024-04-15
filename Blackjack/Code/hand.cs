internal class Hand
{
    private List<Card> _cards = new List<Card>();
    private bool _stand = false;
    private int _bet;

    internal bool Stand { get => _stand; set => _stand = value; }
    internal int Bet { get => _bet; set => _bet = value; }

    internal Hand(int bet) {
        Bet = bet;
    }
    internal Hand(Card card, int bet)
    {
        _cards.Add(card);
        Bet = bet;
    }

    internal void AddCard(Card card)
    {
        _cards.Add(card);
    }

    internal virtual string Show()
    {
        string returnString = "";

        foreach (Card card in _cards)
        {
            returnString += card.Show() + " ";
        }

        return returnString;
    }

    internal virtual int CurrentTotal()
    {
        int total = 0;
        int noOfAces = 0;

        foreach (Card card in _cards)
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

        for (int i = 0; i < noOfAces; i++)
        {
            if (total + 11 > 21)
            {
                total++;
            }
            else
            {
                total += 11;
            }
        }

        return total;
    }

    internal virtual void Wipe(List<Card> discardPile)
    {
        discardPile.AddRange(_cards);
        _cards.Clear();
    }

    internal bool IsBlackjack()
    {
        if (_cards.Count == 2)
        {
            if (CurrentTotal() == 21)
            {
                return true;
            }
        }

        return false;
    }

    internal bool IsSplitPossible()
    {
        if (_cards.Count == 2)
        {
            if (_cards[0].Value == _cards[1].Value)
            {
                return true;
            }
        }

        return false;
    }

    internal bool Split(Player player)
    {
        if (IsSplitPossible())
        {
            player.AddNewHand(_cards[1], _bet);
            _cards.Remove(_cards[1]);
            player.Balance -= _bet;
            return true;
        }
        return false;
    }
}