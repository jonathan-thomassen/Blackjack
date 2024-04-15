internal class Game
{
    private bool _playLoopConcluded = false;
    private int _noOfDecks;
    private Dealer _dealer;
    private Player _player;
    private List<Card> _deck = new List<Card>();
    private List<Card> _discardPile = new List<Card>();
    private bool _gameOver = false;
    private int _decisionNo = 0;
    private Card? _dealersFirstCard;

    internal List<Card> Cards { get => _deck; set => _deck = value; }

    internal Game(Player player, int decks)
    {
        _player = player;
        _dealer = new Dealer();
        _noOfDecks = decks;
    }

    internal void StartGameLoop()
    {
        CreateDeck();

        while (!_gameOver)
        {
            Console.WriteLine("Current balance: " + _player.Balance + " USD");
            Console.WriteLine();

            int bet;
            bool betInputCorrect;
            do
            {
                betInputCorrect = true;
                Console.WriteLine("You may bet from 10 to 100 dollars, in increments of 10 dollars.");
                Console.WriteLine("How much would you like to bet?");
                if (!int.TryParse(Console.ReadLine(), out bet) || !(bet > 0 && bet <= 100 && bet % 10 == 0))
                {
                    betInputCorrect = false;
                    Console.WriteLine("Invalid input. Try again.");
                    Console.WriteLine();
                }

            } while (!betInputCorrect);

            Hand dealerHand = _dealer.WipeAllHands(_discardPile, 0);
            Hand playerHand = _player.WipeAllHands(_discardPile, bet);

            _dealersFirstCard = DrawACard();
            dealerHand.AddCard(_dealersFirstCard);
            playerHand.AddCard(DrawACard());
            playerHand.AddCard(DrawACard());

            _player.Reset();
            _decisionNo = 0;
            _player.Balance -= playerHand.Bet;
            _playLoopConcluded = false;

            while (!_playLoopConcluded)
            {
                _playLoopConcluded = true;
                PlayLoop();
            }

            DealerLoop();
            EndRound();
        }
    }

    internal void PlayLoop()
    {
        while (!_player.Stand())
        {
            Console.WriteLine("Current balance: " + _player.Balance + " USD");
            Console.WriteLine();

            Console.WriteLine("Dealer's hand: " + _dealer.ShowHand());
            Console.WriteLine("Dealer's total: " + _dealer.Hand.CurrentTotal());

            foreach (Hand hand in _player.Hands)
            {
                if (!hand.Stand)
                {
                    Console.WriteLine("Your hand: " + hand.Show());
                    Console.WriteLine("Your total: " + hand.CurrentTotal());
                    Console.WriteLine();

                    bool wasHandSplit = false;

                    if (_decisionNo == 0 && _dealersFirstCard?.Value == 1)
                    {
                        Console.WriteLine("Dealer drew an Ace. Would you like to take insurance? Y/N");

                        var input = Console.ReadKey();
                        Console.WriteLine();

                        if (input.Key == ConsoleKey.Y)
                        {
                            _player.TakeInsurance(hand.Bet / 2);
                        }
                    }

                    if (hand.CurrentTotal() > 21)
                    {
                        hand.Stand = true;
                    }
                    else if (hand.CurrentTotal() == 21)
                    {
                        hand.Stand = true;
                        if (!_player.HasBlackjack())
                        {
                            Console.WriteLine("21. Press any key to continue...");
                            Console.ReadKey(true);
                        }
                    }
                    else
                    {
                        Console.Write("H: Hit, S: Stand, D: Double down");
                        if (_decisionNo == 0)
                        {
                            if (hand.IsSplitPossible())
                            {
                                Console.Write(", P: Split, U: Surrender\n");

                                var input = Console.ReadKey();
                                Console.WriteLine();

                                _decisionNo++;

                                switch (input.Key)
                                {
                                    case ConsoleKey.H:
                                        {
                                            Hit(hand);
                                            break;
                                        }
                                    case ConsoleKey.S:
                                        {
                                            hand.Stand = true;
                                            break;
                                        }
                                    case ConsoleKey.D:
                                        {
                                            DoubleDown(hand);
                                            break;
                                        }
                                    case ConsoleKey.P:
                                        {
                                            wasHandSplit = hand.Split(_player);
                                            break;
                                        }
                                    case ConsoleKey.U:
                                        {
                                            _player.Surrender();
                                            break;
                                        }
                                    default:
                                        {
                                            throw new SystemException("Unsupported input.");
                                        }
                                }
                            }
                            else
                            {
                                Console.Write(", U: Surrender\n");

                                var input = Console.ReadKey();
                                Console.WriteLine();

                                _decisionNo++;

                                switch (input.Key)
                                {
                                    case ConsoleKey.H:
                                        {
                                            Hit(hand);
                                            break;
                                        }
                                    case ConsoleKey.S:
                                        {
                                            hand.Stand = true;
                                            break;
                                        }
                                    case ConsoleKey.D:
                                        {
                                            DoubleDown(hand);
                                            break;
                                        }
                                    case ConsoleKey.U:
                                        {
                                            _player.Surrender();
                                            break;
                                        }
                                    default:
                                        {
                                            throw new SystemException("Unsupported input.");
                                        }
                                }
                            }
                        }
                        else
                        {
                            Console.Write("\n");

                            var input = Console.ReadKey();
                            Console.WriteLine();

                            switch (input.Key)
                            {
                                case ConsoleKey.H:
                                    {
                                        Hit(hand);
                                        break;
                                    }
                                case ConsoleKey.S:
                                    {
                                        hand.Stand = true;
                                        break;
                                    }
                                case ConsoleKey.D:
                                    {
                                        DoubleDown(hand);
                                        break;
                                    }
                                default:
                                    {
                                        throw new SystemException("Unsupported input.");
                                    }
                            }
                        }
                    }

                    if (wasHandSplit)
                    {
                        _playLoopConcluded = false;
                        break;
                    }
                }
            }
        }
    }

    internal void DealerLoop()
    {
        bool running = true;
        while (running)
        {
            if (_dealer.Hand.CurrentTotal() < 17)
            {
                Card card = DrawACard();
                Console.WriteLine("Dealer drew a " + card.Show());
                Console.WriteLine();
                _dealer.Hand.AddCard(card);
            }

            if (_dealer.Hand.CurrentTotal() > 16) running = false;

            Console.WriteLine("Dealer's hand: " + _dealer.ShowHand());
            Console.WriteLine("Dealer's total: " + _dealer.Hand.CurrentTotal());

            foreach (Hand hand in _player.Hands)
            {
                Console.WriteLine("Your hand: " + hand.Show());
                Console.WriteLine("Your total: " + hand.CurrentTotal());
                Console.WriteLine();
            }

            if (running)
            {
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
                Console.WriteLine();
            }
        }
    }

    internal void EndRound()
    {
        if (_player.Insurance > 0)
        {
            if (_dealer.HasBlackjack())
            {
                Console.WriteLine("Dealer has blackjack. Insurance pays out!");
                _player.PayInsurance();
            }
            else
            {
                Console.WriteLine("Dealer does not have blackjack. Insurance does not pay.");
            }
        }
        if (_player.Surrendered)
        {
            Console.WriteLine("You have surrendered.");
        }
        else
        {
            foreach (Hand hand in _player.Hands)
            {
                if (hand.CurrentTotal() < 22)
                {
                    if (hand.IsBlackjack())
                    {
                        if (!_dealer.HasBlackjack())
                        {
                            Console.WriteLine("Blackjack! You win!");
                            _player.Balance += hand.Bet + ((hand.Bet * 3) / 2);
                        }
                        else
                        {
                            Console.WriteLine("Both you and the dealer have blackjack. Push.");
                            _player.Balance += hand.Bet;
                        }
                    }
                    else if (_dealer.Hand.CurrentTotal() < 22)
                    {
                        switch (hand.CurrentTotal() - _dealer.Hand.CurrentTotal())
                        {
                            case > 0:
                                {
                                    Console.WriteLine("You win!");
                                    _player.Balance += hand.Bet * 2;
                                    break;
                                }
                            case 0:
                                {
                                    if (_dealer.HasBlackjack())
                                    {
                                        Console.WriteLine("Dealer has blackjack. Dealer wins.");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Push.");
                                        _player.Balance += hand.Bet;
                                    }
                                    break;
                                }
                            case < 0:
                                {
                                    Console.WriteLine("Dealer wins.");
                                    break;
                                }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Dealer is bust! You win!");
                        _player.Balance += hand.Bet * 2;
                    }
                }
                else
                {
                    Console.WriteLine("You are bust. Dealer wins.");
                }
            }
        }

        Console.WriteLine("Current balance: " + _player.Balance + " USD");
        Console.WriteLine();

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey(true);
    }

    internal void Reshuffle()
    {
        _deck = new List<Card>(_discardPile);
        _discardPile.Clear();
    }

    internal Card DrawACard()
    {
        Random rng = new Random();
        int cardNo = rng.Next(_deck.Count - 1);
        Card card = _deck[cardNo];
        _deck.Remove(card);

        if (_deck.Count == 0) Reshuffle();

        return card;
    }

    internal void Hit(Hand hand)
    {
        Card card = DrawACard();
        Console.WriteLine("You drew a " + card.Show());
        Console.WriteLine();
        hand.AddCard(card);
    }

    internal void DoubleDown(Hand hand)
    {
        _player.Balance -= hand.Bet;
        hand.Bet *= 2;

        Hit(hand);

        hand.Stand = true;
    }

    internal void CreateDeck()
    {
        for (int i = 0; i < _noOfDecks; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                for (int k = 1; k < 14; k++)
                {
                    _deck.Add(new Card(i, (Suits)j, k));
                }
            }
        }
    }
}