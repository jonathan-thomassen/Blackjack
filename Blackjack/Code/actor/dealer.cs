internal class Dealer : Actor
{
    internal string ShowHand()
    {
        return Hands[0].Show();
    }

    internal Hand Hand { get => Hands[0] ; set => Hands[0] = value; }
}