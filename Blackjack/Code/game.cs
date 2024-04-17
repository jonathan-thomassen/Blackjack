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
            DrawBalance();
            DrawEmptyLine();
            DrawDealerHand(true);
            DrawEmptyLine();
            DrawPlayerHand(new Hand(), 0, true);
            DrawEmptyLine();
            DrawBet(_player, true);

            if (_player.Balance < 10)
            {
                DrawString("Your balance is insufficient for another round. Game over.");
                break;
            }

            int bet;
            bool betInputCorrect;
            DrawEmptyLine();
            DrawString("You may bet from $10 to $100, in increments of $10.");
            do
            {
                betInputCorrect = true;
                string betAsk = "How much would you like to bet?";

                if (_player.PreviousBet != null)
                {
                    betAsk += $" (leave blank to bet ${_player.PreviousBet}.)";
                }

                DrawString(betAsk);
                DrawEmptyLine();
                DrawString("$       ");
                DrawEmptyLine();
                DrawSolidLine(2);
                Console.SetCursorPosition(39, Console.CursorTop - 3);
                Console.CursorVisible = true;
                string? betString = Console.ReadLine();
                Console.CursorVisible = false;
                if (betString == "" && _player.PreviousBet != null)
                {
                    bet = (int)_player.PreviousBet;
                }
                else if (!int.TryParse(betString, out bet) || !(bet > 0 && bet <= 100 && bet % 10 == 0))
                {
                    betInputCorrect = false;
                    DrawHeader();                    
                    DrawBalance();
                    DrawEmptyLine();
                    DrawDealerHand(true);
                    DrawEmptyLine();
                    DrawPlayerHand(new Hand(), 0, true);
                    DrawEmptyLine();
                    DrawBet(_player, true);
                    DrawEmptyLine();
                    DrawString("Invalid input. Try again.");
                }

                if (bet > _player.Balance)
                {
                    betInputCorrect = false;
                    DrawHeader();
                    DrawBalance();
                    DrawEmptyLine();
                    DrawDealerHand(true);
                    DrawEmptyLine();
                    DrawPlayerHand(new Hand(), 0, true);
                    DrawEmptyLine();
                    DrawBet(_player, true);
                    DrawEmptyLine();
                    DrawString("Insufficient balance. Try again.");
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
                    DrawEmptyLine();

                    bool wasHandSplit = false;

                    if (_player.DecisionsMade == 0 && _dealer.Hand.Cards[0].Value == 1 && _player.Balance >= (hand.Bet / 2))
                    {
                        bool insuranceInputCorrect;
                        do
                        {
                            insuranceInputCorrect = true;
                            DrawString("Dealer drew an Ace. Would you like to take insurance? Y/N");
                            DrawEmptyLine();
                            DrawEmptyLine();
                            DrawSolidLine(2);
                            var input = Console.ReadKey(true);

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
                                DrawString("Invalid input. Try again.");
                                DrawEmptyLine();
                            }
                        } while (!insuranceInputCorrect);

                        DrawHeader();
                        DrawPlayfield(_player, hand, handNo);
                    }

                    if (hand.CurrentTotal() > 21)
                    {
                        hand.Stand = true;
                        DrawEmptyLine();
                        DrawString("You are bust. Press any key to continue...");
                        DrawEmptyLine();
                        DrawSolidLine(2);
                        Console.ReadKey(true);
                        DrawHeader();
                    }
                    else if (hand.CurrentTotal() == 21)
                    {
                        hand.Stand = true;
                        if (!_player.HasBlackjack())
                        {
                            DrawString("21! You stand. Press any key to continue...");
                        }
                        else
                        {
                            DrawString("You got a Blackjack! Press any key to continue...");
                        }
                        DrawEmptyLine();
                        DrawEmptyLine();
                        DrawSolidLine(2);
                        Console.ReadKey(true);
                        DrawHeader();
                    }
                    else
                    {
                        bool actionInputCorrect;

                        do
                        {
                            actionInputCorrect = true;
                            string optionString = "H: Hit, S: Stand";
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

                            DrawString(optionString);
                            DrawEmptyLine();
                            DrawEmptyLine();
                            DrawSolidLine(2);

                            var input = Console.ReadKey(true);
                            DrawHeader();

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
                                DrawString("Invalid input. Try again.");
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
            DrawBalance();
            DrawEmptyLine();

            if (_dealer.Hand.CurrentTotal() < 17)
            {
                Card card = DrawACard();
                _dealer.Hand.AddCard(card);
            }

            if (_dealer.Hand.CurrentTotal() > 16) running = false;

            DrawDealerHand();
            DrawEmptyLine();

            foreach (Hand hand in _player.Hands)
            {
                DrawPlayerHand(hand, handNo++);
                DrawEmptyLine();
                DrawBet(_player);
            }

            DrawEmptyLine();

            if (running)
            {
                DrawEmptyLine();
                DrawEmptyLine();
                Console.Write("│                     ");
                Console.Write("Dealer draws . ");
                Console.Write("                                          │\n");
                DrawEmptyLine();
                DrawEmptyLine();
                DrawSolidLine(2);
                Thread.Sleep(333);
                Console.SetCursorPosition(34, Console.CursorTop - 4);
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
        DrawEmptyLine();
        if (_player.InsuranceTaken != null && (bool)_player.InsuranceTaken)
        {
            if (_dealer.HasBlackjack())
            {
                DrawString("Dealer has blackjack. Insurance pays out and you get $" + _player.Hands[0].Bet + ".");
                _player.PayInsurance();
            }
            else
            {
                DrawString("Dealer does not have blackjack. Insurance does not pay.");
            }
        }
        if (_player.Surrendered)
        {
            DrawString("You have surrendered. You get $" + _player.Hands[0].Bet / 2 + " back.");
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
                            DrawString("Blackjack! You win $" + (hand.Bet + ((hand.Bet * 3) / 2)) + "!");
                            _player.Balance += hand.Bet + ((hand.Bet * 3) / 2);
                        }
                        else
                        {
                            DrawString("Both you and the dealer have blackjack. Push. You get your bet of $" + hand.Bet + " back.");
                            _player.Balance += hand.Bet;
                        }
                    }
                    else if (_dealer.Hand.CurrentTotal() < 22)
                    {
                        switch (hand.CurrentTotal() - _dealer.Hand.CurrentTotal())
                        {
                            case > 0:
                                {
                                    DrawString("Your hand wins and nets you $" + hand.Bet * 2 + "!");
                                    _player.Balance += hand.Bet * 2;
                                    break;
                                }
                            case 0:
                                {
                                    if (_dealer.HasBlackjack())
                                    {
                                        DrawString("Dealer has blackjack. Dealer wins.");
                                    }
                                    else
                                    {
                                        DrawString("Push. You get your bet of $" + hand.Bet + " back.");
                                        _player.Balance += hand.Bet;
                                    }
                                    break;
                                }
                            case < 0:
                                {
                                    DrawString("Dealer wins.");
                                    break;
                                }
                        }
                    }
                    else
                    {
                        DrawString("Dealer is bust! You win $" + hand.Bet * 2 + "!");
                        _player.Balance += hand.Bet * 2;
                    }
                }
                else
                {
                    DrawString("You are bust. Dealer wins.");
                }
            }
        }

        DrawEmptyLine();
        DrawString("Press any key to start a new round...");
        DrawEmptyLine();
        DrawSolidLine(2);
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

    internal void DrawDealerHand(bool mock = false)
    {
        DrawEmptyLine();
        Console.Write("│                          ");
        Console.Write("Dealer's hand: ");
        if (!mock)
        {
            _dealer.DrawHand();

            for (int i = 0; i < 38 - _dealer.Hand.Cards.Count * 3; i++)
            {
                Console.Write(" ");
            }

            foreach (Card card in _dealer.Hand.Cards)
            {
                if (card.Value == 10)
                {
                    Console.Write("\b");
                }
            }
        }
        else
        {
            for (int i = 0; i < 37; i++)
            {
                Console.Write(" ");
            }
        }

        Console.Write("│\n");
        Console.Write("│                          ");
        Console.Write("Dealer's total: ");
        if (!mock)
        {
            Console.Write(_dealer.Hand.CurrentTotal());

            for (int i = 0; i < 36 - _dealer.Hand.CurrentTotal().ToString().Length; i++)
            {
                Console.Write(" ");
            }
        }
        else
        {
            for (int i = 0; i < 36; i++)
            {
                Console.Write(" ");
            }
        }
        Console.Write("│\n");
    }

    internal void DrawPlayerHand(Hand hand, int handNo, bool mock = false)
    {
        if (!mock)
        {
            if (_player.Hands.Count > 1)
            {
                Console.Write($"Your hand #{handNo}: ");
                hand.DrawHand();
                Console.Write($"Your hand #{handNo} total: {hand.CurrentTotal()}");
            }
            else
            {
                Console.Write("│                          ");
                Console.Write($"Your hand: ");
                hand.DrawHand();

                for (int i = 0; i < 42 - hand.Cards.Count * 3; i++)
                {
                    Console.Write(" ");
                }

                foreach (Card card in hand.Cards)
                {
                    if (card.Value == 10)
                    {
                        Console.Write("\b");
                    }
                }

                Console.Write("│\n");
                Console.Write("│                          ");
                Console.Write($"Your total: {hand.CurrentTotal()}");
                for (int i = 0; i < 40 - hand.CurrentTotal().ToString().Length; i++)
                {
                    Console.Write(" ");
                }
                Console.Write("│\n");
            }
        }
        else
        {
            Console.WriteLine("│                          Your hand:                                          │");
            Console.WriteLine("│                          Your total:                                         │");
        }
    }

    internal void DrawHeader()
    {
        Console.Clear();

        DrawSolidLine(0);

        Console.Write("│ ");
        Console.Write("C# Blackjack Alpha v0.001");
        for (int i = 0; i < 52; i++)
        {
            Console.Write(" ");
        }
        Console.Write("│\n");

        DrawSolidLine(1);
    }

    internal void DrawBalance()
    {
        Console.Write("│ ");
        Console.Write("Balance: ");
        string balance = _player.Balance.ToString();
        for (int i = 0; i < 5 - balance.Length; i++)
        {
            Console.Write(" ");
        }
        Console.Write(balance + " $");
        Console.Write("                                                             │\n");
    }

    internal void DrawPlayfield(Player player, Hand hand, int handNo)
    {

        DrawBalance();
        DrawEmptyLine();
        DrawDealerHand();
        DrawEmptyLine();
        DrawPlayerHand(hand, handNo);
        DrawEmptyLine();
        DrawBet(player);
        DrawEmptyLine();
        DrawEmptyLine();
    }

    internal void DrawBet(Player player, bool mock = false)
    {
        string text = $"Your bet: ";
        if (!mock)
        {
            text += player.Hands[0].Bet + "$";
            if (player.InsuranceTaken != null)
            {
                if ((bool)player.InsuranceTaken)
                {
                    text += " Insurance taken.";
                }
                else
                {
                    text += " Insurance not taken.";
                }
            }
        }

        Console.Write("│                          ");
        Console.Write(text);
        for (int i = 0; i < 52 - text.Length; i++)
        {
            Console.Write(" ");
        }
        Console.Write("│\n");
    }

    internal void DrawEmptyLine()
    {
        Console.Write("│");
        for (int i = 0; i < 78; i++)
        {
            Console.Write(" ");
        }
        Console.Write("│\n");
    }

    internal void DrawSolidLine(int location)
    {
        switch (location)
        {
            case 0:
                {
                    Console.Write("┌");
                    break;
                }
            case 1:
                {
                    Console.Write("├");
                    break;
                }
            case 2:
                {
                    Console.Write("└");
                    break;
                }
        }

        for (int i = 0; i < 78; i++)
        {
            Console.Write("─");
        }

        switch (location)
        {
            case 0:
                {
                    Console.Write("┐");
                    break;
                }
            case 1:
                {
                    Console.Write("┤");
                    break;
                }
            case 2:
                {
                    Console.Write("┘");
                    break;
                }
        }

        Console.Write("\n");
    }

    internal void DrawString(string text)
    {
        Console.Write("│");
        for (int i = 0; i < 39 - (text.Length / 2); i++)
        {
            Console.Write(" ");
        }
        Console.Write(text);
        for (int i = 0; i < 38 - (text.Length / 2); i++)
        {
            Console.Write(" ");
        }
        if (text.Length % 2 == 0)
        {
            Console.Write(" ");
        }
        Console.Write("│\n");
    }
    #endregion
}