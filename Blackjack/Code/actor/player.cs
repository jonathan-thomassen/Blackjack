internal class Player : Actor
{
    private string _name;
    private int _balance;
    private bool? _insuranceTaken = null;
    private bool _surrendered = false;
    private int _decisionsMade = 0;
    private int? _previousBet = null;

    internal string Name { get => _name; set => _name = value; }
    internal int Balance { get => _balance; set => _balance = value; }
    internal bool? InsuranceTaken { get => _insuranceTaken; }
    internal bool Surrendered { get => _surrendered; }
    internal int DecisionsMade { get => _decisionsMade; set => _decisionsMade = value; }
    internal int? PreviousBet { get => _previousBet; }

    internal Player(string name, int balance)
    {
        if (name == null || name == "")
        {
            _name = "Default";
        }
        else
        {
            _name = name;
        }

        _balance = balance;
    }

    internal void AddNewHand(Card card, int bet)
    {
        Hand hand = new(bet);
        hand.AddCard(card);
        Hands.Add(hand);
    }

    internal bool Stand()
    {
        foreach (Hand hand in Hands)
        {
            if (!hand.Stand)
            {
                return false;
            }
        }

        return true;
    }

    internal bool IsBust()
    {
        bool returnBool = true;

        foreach (Hand hand in Hands)
        {
            if (hand.CurrentTotal() < 22)
            {
                returnBool = false;
                break;
            }
        }

        return returnBool;
    }

    internal void InsuranceDecision(bool insuranceTaken)
    {
        _balance -= Hands[0].Bet / 2;
        _insuranceTaken = insuranceTaken;
    }

    internal void PayInsurance()
    {
        _balance += Hands[0].Bet;
    }

    internal void Surrender()
    {
        _balance += Hands[0].Bet / 2;
        Hands[0].Stand = true;
        _surrendered = true;
    }

    internal void Reset()
    {
        _surrendered = false;
        _decisionsMade = 0;
        _previousBet = Hands[0].Bet;
        _insuranceTaken = null;
    }

    internal Hand DealNewHand(List<Card> discardPile, int bet)
    {
        foreach (Hand hand in Hands)
        {
            hand.Wipe(discardPile);
        }
        Hands.Clear();
        Hand returnHand = new(bet);
        Hands.Add(returnHand);
        return returnHand;
    }
}