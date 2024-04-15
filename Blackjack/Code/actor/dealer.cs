internal class Dealer : Actor
{
    internal Hand Hand { get => Hands[0] ; set => Hands[0] = value; }

    internal string ShowHand()
    {
        return Hands[0].Show();
    }

    internal Hand DealNewHand(List<Card> discardPile)
    {
        foreach (Hand hand in Hands)
        {
            hand.Wipe(discardPile);
        }
        Hands.Clear();
        Hand returnHand = new Hand();
        Hands.Add(returnHand);
        return returnHand;
    }
}