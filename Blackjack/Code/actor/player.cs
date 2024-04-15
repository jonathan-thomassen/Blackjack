
internal class Player : Actor {
    private string _name;    
    private int _balance;

    public string Name { get => _name; set => _name = value; }
    public int Balance { get => _balance; set => _balance = value; }

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

    internal void AddNewHand(Card card) {
        Hand hand = new Hand();
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
}