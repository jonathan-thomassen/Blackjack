
internal abstract class Actor
{
    private List<Hand> _hands = new List<Hand>();

    internal List<Hand> Hands { get => _hands; set => _hands = value; }

    internal Actor()
    {
    }

    internal virtual Hand WipeAllHands(List<Card> discardPile, int newBet)
    {
        foreach (Hand hand in _hands)
        {
            hand.Wipe(discardPile);
        }
        _hands.Clear();
        Hand returnHand = new Hand(newBet);
        _hands.Add(returnHand);
        return returnHand;
    }

    internal virtual bool HasBlackjack()
    {
        foreach (Hand hand in _hands)
        {
            if (hand.IsBlackjack())
            {
                return true;
            }
        }
        return false;
    }

    /* public override string ToString()
    {
        string returnString = "";

        int n = 0;
        foreach (Hand hand in Hands)
        {
            foreach (Card card in hand)
            {
                returnString += "Hand " + n + ": " + card.Show() + " ";
            }
            n++;
        }

        return returnString;
    } */
}