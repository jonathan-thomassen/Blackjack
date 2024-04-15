internal abstract class Actor
{
    private List<Hand> _hands = new List<Hand>();

    internal List<Hand> Hands { get => _hands; set => _hands = value; }

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
}