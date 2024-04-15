internal class Game
{
    private bool _playLoopConcluded = false;
    private int _noOfDecks;
    private bool _gameOver = false;
    private Dealer _dealer;
    private Player _player;
    private List<Card> _deck = new List<Card>();
    private List<Card> _discardPile = new List<Card>();

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
                Console.WriteLine("You may bet from 10 to 100 USD, in increments of 10 USD.");
                Console.Write("How much would you like to bet?");
                if (_player.PreviousBet != null)
                {
                    Console.Write($" (leave blank to bet {_player.PreviousBet} USD)");
                }
                Console.Write("\n");

                if (!int.TryParse(Console.ReadLine(), out bet) || !(bet > 0 && bet <= 100 && bet % 10 == 0))
                {
                    betInputCorrect = false;
                    Console.WriteLine("Invalid input. Try again.");
                    Console.WriteLine();
                }

            } while (!betInputCorrect);

            Hand dealerHand = _dealer.DealNewHand(_discardPile);
            Hand playerHand = _player.DealNewHand(_discardPile, bet);

            dealerHand.AddCard(DrawACard());
            playerHand.AddCard(DrawACard());
            playerHand.AddCard(DrawACard());

            _player.Reset();
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

            ShowDealerHand();

            int handNo = 0;
            foreach (Hand hand in _player.Hands)
            {
                handNo++;
                if (!hand.Stand)
                {
                    ShowPlayerHand(hand, handNo);

                    bool wasHandSplit = false;

                    if (_player.DecisionsMade == 0 && _dealer.Hand.Cards[0].Value == 1)
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
                        Console.WriteLine("You are bust. Press any key to continue...");
                        Console.ReadKey(true);
                        hand.Stand = true;
                    }
                    else if (hand.CurrentTotal() == 21)
                    {
                        hand.Stand = true;
                        if (!_player.HasBlackjack())
                        {
                            Console.WriteLine("21! You stand. Press any key to continue...");
                            Console.ReadKey(true);
                        } else
                        {
                            Console.WriteLine("Blackjack! Press any key to continue...");
                            Console.ReadKey(true);
                        }
                    }
                    else
                    {
                        Console.Write("H: Hit, S: Stand, D: Double down");
                        if (_player.DecisionsMade == 0)
                        {
                            if (hand.IsSplitPossible())
                            {
                                Console.Write(", P: Split, U: Surrender\n");

                                var input = Console.ReadKey();
                                Console.WriteLine();

                                _player.DecisionsMade++;

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

                                _player.DecisionsMade++;

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

            ShowDealerHand();

            foreach (Hand hand in _player.Hands)
            {
                ShowPlayerHand(hand);
            }

            if (running)
            {
                Console.WriteLine("Dealer will hit. Press any key to continue...");
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
                Console.WriteLine("Dealer has blackjack. Insurance pays out and you get " + _player.Insurance + " USD.");
                _player.PayInsurance();
            }
            else
            {
                Console.WriteLine("Dealer does not have blackjack. Insurance does not pay.");
            }
        }
        if (_player.Surrendered)
        {
            Console.WriteLine("You have surrendered. You get " + _player.Hands[0].Bet / 2 + " USD back.");
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
                            Console.WriteLine("Blackjack! You win " + (hand.Bet + ((hand.Bet * 3) / 2)) + " USD!");
                            _player.Balance += hand.Bet + ((hand.Bet * 3) / 2);
                        }
                        else
                        {
                            Console.WriteLine("Both you and the dealer have blackjack. Push. You get your bet of " + hand.Bet + " USD back.");
                            _player.Balance += hand.Bet;
                        }
                    }
                    else if (_dealer.Hand.CurrentTotal() < 22)
                    {
                        switch (hand.CurrentTotal() - _dealer.Hand.CurrentTotal())
                        {
                            case > 0:
                                {
                                    Console.WriteLine("Your hand wins and nets you " + hand.Bet * 2 + " USD!");
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
                                        Console.WriteLine("Push. You get your bet of " + hand.Bet + " USD back.");
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
                        Console.WriteLine("Dealer is bust! You win " + hand.Bet * 2 + " USD!");
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

        Console.WriteLine("Press any key to start a new round...");
        Console.ReadKey(true);
        Console.Clear();
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

    internal void ShowDealerHand()
    {
        Console.WriteLine("Dealer's hand: " + _dealer.ShowHand());
        Console.WriteLine("Dealer's total: " + _dealer.Hand.CurrentTotal());
    }

    internal void ShowPlayerHand(Hand hand, int handNo = 0)
    {
        if (_player.Hands.Count > 1)
        {
            Console.WriteLine($"Your hand #{handNo}: {hand.Show()}");
            Console.WriteLine($"Your hand #{handNo} total: {hand.CurrentTotal()}");
            Console.WriteLine();
        }
        else
        {
            Console.WriteLine($"Your hand: {hand.Show()}");
            Console.WriteLine($"Your total: {hand.CurrentTotal()}");
            Console.WriteLine();
        }
    }
}