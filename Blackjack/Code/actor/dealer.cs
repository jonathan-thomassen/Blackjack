internal class Dealer : Actor
{
    private bool _standOnSoft17;

    internal Hand Hand { get => Hands[0]; set => Hands[0] = value; }
    internal bool StandOnSoft17 { get => _standOnSoft17; set => _standOnSoft17 = value; }

    internal Dealer(bool standOnSoft17)
    {
        _standOnSoft17 = standOnSoft17;
    }

    internal void DrawHand()
    {
        if (Hands.Count > 0)
        {
            Hands[0].DrawHand();
        }
    }

    internal Hand DealNewHand(List<Card> discardPile)
    {
        foreach (Hand hand in Hands)
        {
            hand.Wipe(discardPile);
        }
        Hands.Clear();
        Hand returnHand = new();
        Hands.Add(returnHand);
        return returnHand;
    }
}