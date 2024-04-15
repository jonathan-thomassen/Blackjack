
internal class Player : Actor {
    private string _name;    
    private int _balance;
    private int _insurance = 0;
    private bool _surrendered = false;
    private int _decisionsMade = 0;

    public string Name { get => _name; set => _name = value; }
    public int Balance { get => _balance; set => _balance = value; }
    public int Insurance { get => _insurance; set => _insurance = value; }
    public bool Surrendered { get => _surrendered; }
    public int DecisionsMade { get => _decisionsMade; set => _decisionsMade = value; }

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

    internal void AddNewHand(Card card, int bet) {
        Hand hand = new Hand(bet);
        hand.AddCard(card);
        Hands.Add(hand);
    }

    internal bool Stand() {
        foreach (Hand hand in Hands) {
            if (!hand.Stand) {
                return false;
            }
        }

        return true;
    }

    internal bool IsBust() {
        bool returnBool = true;

        foreach (Hand hand in Hands) {
            if (hand.CurrentTotal() < 22) {
                returnBool = false;
                break;
            }
        }

        return returnBool;
    }

    internal void TakeInsurance(int amount)
    {
        _balance -= amount;
        _insurance = amount;
    }

    internal void PayInsurance()
    {
        _balance += _insurance;
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
    }

    internal Hand DealNewHand(List<Card> discardPile, int bet)
    {
        foreach (Hand hand in Hands)
        {
            hand.Wipe(discardPile);
        }
        Hands.Clear();
        Hand returnHand = new Hand(bet);
        Hands.Add(returnHand);
        return returnHand;
    }
}