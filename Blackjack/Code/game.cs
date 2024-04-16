internal class Game
{
    private int _noOfDecks;
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

    #region MainGameplayLoops
    internal void StartGameLoop()
    {
        CreateDeck();

        while (true)
        {
            WriteBalance();
            ShowMockHands();

            if (_player.Balance < 10)
            {
                Console.WriteLine("Your balance is insufficient for another round. Game over.");
                Console.WriteLine();

                break;
            }

            int bet;
            bool betInputCorrect;
            Console.Write("|                                                                              |\n");
            Console.Write("|      ");
            Console.Write("You may bet from $10 to $100, in increments of $10.");
            Console.Write("                     |\n");
            do
            {
                betInputCorrect = true;
                Console.Write("|      ");
                Console.Write("How much would you like to bet?");

                if (_player.PreviousBet != null)
                {
                    Console.Write($" (leave blank to bet ${_player.PreviousBet}.)");
                    Console.Write("               |\n");
                }
                else
                {
                    Console.Write("                                         |\n");
                }
                Console.Write("|                                                                              |\n");
                Console.Write("|                                 $                                            |\n");
                Console.Write("|                                                                              |\n");
                Console.Write("--------------------------------------------------------------------------------\n");

                Console.SetCursorPosition(36, Console.CursorTop - 3);
                string? betString = Console.ReadLine();
                if (betString == "" && _player.PreviousBet != null)
                {
                    bet = (int)_player.PreviousBet;
                }
                else if (!int.TryParse(betString, out bet) || !(bet > 0 && bet <= 100 && bet % 10 == 0))
                {
                    betInputCorrect = false;
                    DrawHeader();
                    WriteBalance();
                    ShowMockHands();
                    Console.WriteLine("You may bet from 10 to 100 USD, in increments of 10 USD.");
                    Console.WriteLine("Invalid input. Try again.");
                }

                if (bet > _player.Balance)
                {
                    betInputCorrect = false;
                    DrawHeader();
                    WriteBalance();
                    ShowMockHands();
                    Console.WriteLine("You may bet from 10 to 100 USD, in increments of 10 USD.");
                    Console.WriteLine("Invalid input. Try again.");
                }
            } while (!betInputCorrect);

            DrawHeader();

            Hand dealerHand = _dealer.DealNewHand(_discardPile);
            Hand playerHand = _player.DealNewHand(_discardPile, bet);

            dealerHand.AddCard(DrawACard());
            playerHand.AddCard(DrawACard());
            playerHand.AddCard(DrawACard());

            _player.Reset();
            _player.Balance -= playerHand.Bet;

            PlayLoop();
            DealerLoop();
            EndRound();
        }
    }

    internal void PlayLoop()
    {
        while (!_player.Stand())
        {
            int handNo = 0;
            foreach (Hand hand in _player.Hands)
            {
                handNo++;
                if (!hand.Stand)
                {
                    DrawPlayfield(_player, hand, handNo);
                    Console.Write("|                                                                              |\n");

                    bool wasHandSplit = false;

                    if (_player.DecisionsMade == 0 && _dealer.Hand.Cards[0].Value == 1 && _player.Balance >= (hand.Bet / 2))
                    {
                        bool insuranceInputCorrect;
                        do
                        {
                            insuranceInputCorrect = true;
                            Console.WriteLine("Dealer drew an Ace. Would you like to take insurance? Y/N");
                            var input = Console.ReadKey(true);
                            Console.WriteLine();

                            if (input.Key == ConsoleKey.Y)
                            {
                                _player.InsuranceDecision(true);
                            }
                            else if (input.Key == ConsoleKey.N)
                            {
                                _player.InsuranceDecision(false);
                            }
                            else
                            {
                                insuranceInputCorrect = false;
                                DrawHeader();
                                DrawPlayfield(_player, hand, handNo);
                                Console.WriteLine("Invalid input. Try again.");
                            }
                        } while (!insuranceInputCorrect);

                        DrawHeader();
                        DrawPlayfield(_player, hand, handNo);
                        Console.WriteLine();
                    }

                    if (hand.CurrentTotal() > 21)
                    {
                        hand.Stand = true;
                        Console.Write("|             ");
                        Console.Write("You are bust. Press any key to continue...");
                        Console.Write("                       |\n");
                        Console.Write("|                                                                              |\n");
                        Console.Write("--------------------------------------------------------------------------------\n");
                        Console.ReadKey(true);
                        DrawHeader();
                    }
                    else if (hand.CurrentTotal() == 21)
                    {
                        hand.Stand = true;
                        if (!_player.HasBlackjack())
                        {
                            Console.WriteLine("21! You stand. Press any key to continue...");
                        }
                        else
                        {
                            Console.WriteLine("You got a Blackjack! Press any key to continue...");
                        }
                        Console.ReadKey(true);
                        DrawHeader();
                    }
                    else
                    {
                        bool actionInputCorrect;

                        do
                        {
                            actionInputCorrect = true;
                            string optionString = "|            H: Hit, S: Stand";
                            if (_player.Balance >= hand.Bet)
                            {
                                optionString += ", D: Double down";
                            }
                            if (_player.DecisionsMade == 0)
                            {
                                if (hand.IsSplitPossible())
                                {
                                    optionString += (", P: Split");
                                }
                                optionString += (", U: Surrender");
                            }

                            Console.Write(optionString);
                            for (int i = 0; i < 79 - optionString.Length; i++)
                            {
                                Console.Write(" ");
                            }
                            Console.Write("|\n");
                            Console.Write("|                                                                              |\n");
                            Console.Write("--------------------------------------------------------------------------------\n");

                            Console.CursorVisible = false;
                            var input = Console.ReadKey(true);
                            DrawHeader();
                            Console.CursorVisible = true;

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
                                        if (_player.Balance >= hand.Bet)
                                        {
                                            DoubleDown(hand);
                                        }
                                        else
                                        {
                                            actionInputCorrect = false;
                                        }
                                        break;
                                    }
                                case ConsoleKey.P:
                                    {
                                        if (_player.DecisionsMade == 0 && hand.IsSplitPossible())
                                        {
                                            wasHandSplit = hand.Split(_player);
                                        }
                                        else
                                        {
                                            actionInputCorrect = false;
                                        }
                                        break;
                                    }
                                case ConsoleKey.U:
                                    {
                                        if (_player.DecisionsMade == 0)
                                        {
                                            _player.Surrender();
                                        }
                                        else
                                        {
                                            actionInputCorrect = false;
                                        }
                                        break;
                                    }
                                default:
                                    {
                                        actionInputCorrect = false;
                                        break;
                                    }
                            }

                            if (!actionInputCorrect)
                            {
                                DrawPlayfield(_player, hand, handNo);
                                Console.WriteLine("Invalid input. Try again.");
                            }
                        } while (!actionInputCorrect);
                        _player.DecisionsMade++;
                    }
                    if (wasHandSplit)
                    {
                        break;
                    }
                }
            }
        }
    }

    internal void DealerLoop()
    {
        bool running = true;
        int handNo = 0;
        while (running)
        {
            WriteBalance();
            Console.Write("|                                                                              |\n");

            if (_dealer.Hand.CurrentTotal() < 17)
            {
                Card card = DrawACard();
                _dealer.Hand.AddCard(card);
            }

            if (_dealer.Hand.CurrentTotal() > 16) running = false;

            ShowDealerHand();

            foreach (Hand hand in _player.Hands)
            {
                ShowPlayerHand(hand, handNo++);
            }

            if (running)
            {
                Console.Write("|                                                                              |\n");
                Console.Write("|           ");
                Console.Write("Dealer draws . ");
                Console.Write("                                                          |\n");
                Thread.Sleep(333);
                Console.SetCursorPosition(27, Console.CursorTop - 1);
                Console.Write(". ");
                Thread.Sleep(333);
                Console.Write(". ");
                Thread.Sleep(333);
                DrawHeader();
            }
        }
    }

    internal void EndRound()
    {
        if (_player.InsuranceTaken != null && (bool)_player.InsuranceTaken)
        {
            if (_dealer.HasBlackjack())
            {
                Console.WriteLine("Dealer has blackjack. Insurance pays out and you get " + _player.Hands[0].Bet + " USD.");
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

        Console.Write("|                                                                              |\n");
        Console.WriteLine("Press any key to start a new round...");
        Console.ReadKey(true);
        DrawHeader();
    }
    #endregion

    #region UtilityFunctions
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
    #endregion

    #region UI
    internal void ShowDealerHand()
    {
        Console.Write("|                        ");
        Console.Write("Dealer's hand: " + _dealer.ShowHand());
        for (int i = 0; i < 39 - _dealer.ShowHand().Length; i++)
        {
            Console.Write(" ");
        }
        Console.Write("|\n");
        Console.Write("|                        ");
        Console.Write("Dealer's total: " + _dealer.Hand.CurrentTotal());
        for (int i = 0; i < 38 - _dealer.Hand.CurrentTotal().ToString().Length; i++)
        {
            Console.Write(" ");
        }
        Console.Write("|\n");
    }

    internal void ShowPlayerHand(Hand hand, int handNo)
    {
        if (_player.Hands.Count > 1)
        {
            Console.WriteLine($"Your hand #{handNo}: {hand.Show()}");
            Console.WriteLine($"Your hand #{handNo} total: {hand.CurrentTotal()}");
            Console.WriteLine();
        }
        else
        {
            Console.Write("|                        ");
            Console.Write($"Your hand: {hand.Show()}");
            for (int i = 0; i < 43 - hand.Show().Length; i++)
            {
                Console.Write(" ");
            }
            Console.Write("|\n");
            Console.Write("|                        ");
            Console.Write($"Your total: {hand.CurrentTotal()}");
            for (int i = 0; i < 42 - hand.CurrentTotal().ToString().Length; i++)
            {
                Console.Write(" ");
            }
            Console.Write("|\n");
        }
    }

    internal void DrawHeader()
    {
        Console.Clear();

        for (int i = 0; i < 80; i++)
        {
            Console.Write("-");
        }
        Console.Write("\n");

        Console.Write("| ");
        Console.Write("C# Blackjack Alpha v0.001");
        for (int i = 0; i < 52; i++)
        {
            Console.Write(" ");
        }
        Console.Write("|\n");

        for (int i = 0; i < 80; i++)
        {
            Console.Write("-");
        }
        Console.Write("\n");
    }

    internal void WriteBalance()
    {
        Console.Write("| ");
        Console.Write("Balance: ");
        string balance = _player.Balance.ToString();
        for (int i = 0; i < 5 - balance.Length; i++)
        {
            Console.Write(" ");
        }
        Console.Write(balance + " $");
        Console.Write("                                                             |\n");
    }

    internal void DrawPlayfield(Player player, Hand hand, int handNo)
    {

        WriteBalance();
        Console.Write("|                                                                              |\n");
        ShowDealerHand();
        Console.Write("|                                                                              |\n");
        ShowPlayerHand(hand, handNo);
        ShowBet(player);
        Console.Write("|                                                                              |\n");
        Console.Write("|                                                                              |\n");
        Console.Write("|                                                                              |\n");
    }

    internal void ShowBet(Player player)
    {
        Console.Write($"|                        Your bet: ${player.Hands[0].Bet}");
        if (player.InsuranceTaken != null)
        {
            if ((bool)player.InsuranceTaken)
            {
                Console.Write(" Insurance taken.");
            }
            else
            {
                Console.Write(" Insurance not taken.");
            }
        }
        Console.Write("                                         |\n");
    }

    internal void ShowMockHands()
    {
        Console.Write("|                                                                              |\n");
        Console.Write("|                        Dealer's hand:                                        |\n");
        Console.Write("|                        Dealer's total:                                       |\n");
        Console.Write("|                                                                              |\n");
        Console.Write("|                        Your hand:                                            |\n");
        Console.Write("|                        Your total:                                           |\n");
        Console.Write("|                        Your bet:                                             |\n");
    }

    #endregion
}