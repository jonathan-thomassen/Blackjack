using static Draw;

internal class Game
{
    private int _noOfDecks;
    private Dealer _dealer;
    private Player _player;
    private List<Card> _deck = new List<Card>();
    private List<Card> _discardPile = new List<Card>();

    internal Game(Player player, int decks, bool standOnSoft17)
    {
        _player = player;
        _dealer = new Dealer(standOnSoft17);
        _noOfDecks = decks;
    }

    #region MainGameplayLoops
    internal void StartGameLoop()
    {
        DrawHeader();
        CreateDeck();

        while (true)
        {
            DrawPlayfield(_player, _dealer, _deck, true);

            if (_player.Balance < 10)
            {
                DrawString("Your balance is insufficient for another round. Game over.");
                break;
            }

            int bet;
            bool betInputCorrect;
            DrawEmptyLine();
            do
            {
                betInputCorrect = true;
                DrawString("You may bet from $10 to $100, in increments of $10.");
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
                    DrawPlayfield(_player, _dealer, _deck, true);
                    DrawString("Invalid input. Try again.");
                }
                else if (bet > _player.Balance)
                {
                    betInputCorrect = false;
                    DrawHeader();
                    DrawPlayfield(_player, _dealer, _deck, true);
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
                    DrawPlayfield(_player, _dealer, _deck);

                    bool wasHandSplit = false;

                    if (_player.DecisionsMade == 0 && _dealer.Hand.Cards[0].Value == 1 && _player.Balance >= (hand.Bet / 2))
                    {
                        DrawEmptyLine();
                        DrawEmptyLine();
                        DrawEmptyLine();
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
                                DrawPlayfield(_player, _dealer, _deck);
                                DrawEmptyLine();
                                DrawString("Invalid input. Try again.");
                                DrawEmptyLine();
                            }
                        } while (!insuranceInputCorrect);

                        DrawHeader();
                        DrawPlayfield(_player, _dealer, _deck);
                    }

                    if (hand.CurrentTotal() > 21)
                    {
                        hand.Stand = true;
                        DrawEmptyLine();
                        DrawEmptyLine();
                        DrawString("You are bust. Press any key to continue...");
                        DrawEmptyLine();
                        DrawEmptyLine();
                        DrawEmptyLine();
                        DrawSolidLine(2);
                        Console.ReadKey(true);
                        DrawHeader();
                    }
                    else if (hand.CurrentTotal() == 21)
                    {
                        hand.Stand = true;
                        DrawEmptyLine();
                        DrawEmptyLine();
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
                        DrawEmptyLine();
                        DrawSolidLine(2);
                        Console.ReadKey(true);
                        DrawHeader();
                    }
                    else
                    {
                        if (hand.Cards.Count < 2)
                        {
                            DrawHeader();
                            Hit(hand);
                        }
                        else
                        {
                            bool actionInputCorrect;

                            DrawEmptyLine();
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
                                    if (hand.IsSplitPossible(_player))
                                    {
                                        optionString += (", P: Split");
                                    }
                                    optionString += (", U: Surrender");
                                }

                                DrawEmptyLine();

                                if (_player.Hands.Count > 1)
                                {
                                    DrawString("Decision for hand #" + handNo + ":");
                                }
                                else
                                {
                                    DrawEmptyLine();
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
                                            if (_player.DecisionsMade == 0 && hand.IsSplitPossible(_player))
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
                                    DrawPlayfield(_player, _dealer, _deck);
                                    DrawString("Invalid input. Try again.");
                                }
                            } while (!actionInputCorrect);
                            _player.DecisionsMade++;

                            if (wasHandSplit)
                            {
                                break;
                            }
                        }
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
            Card card = DrawACard();
            _dealer.Hand.AddCard(card);

            DrawPlayfield(_player, _dealer, _deck);

            if (_dealer.Hand.CurrentTotal() > 16)
            {
                if (!_dealer.StandOnSoft17 && _dealer.Hand.CurrentTotal() == 17)
                {
                    // Logic for determining Soft 17

                    int cardNo;
                    for (cardNo = 0; cardNo < _dealer.Hand.Cards.Count; cardNo++)
                    {
                        if (_dealer.Hand.Cards[cardNo].Value == 1)
                        {
                            Hand testHand = new Hand();
                            testHand.Cards.AddRange(_dealer.Hand.Cards);
                            testHand.Cards.Remove(_dealer.Hand.Cards[cardNo]);
                            if (testHand.CurrentTotal() == 16)
                            {
                                running = false;
                            }
                            break;
                        }
                    }
                }
                else
                {
                    running = false;
                }
            }


            if (running)
            {
                DrawEmptyLine();
                DrawEmptyLine();
                Console.Write("│                        ");
                Console.Write("Dealer draws . ");
                Console.Write("                                       │\n");
                DrawEmptyLine();
                DrawEmptyLine();
                DrawEmptyLine();
                DrawSolidLine(2);
                Thread.Sleep(333);
                Console.SetCursorPosition(40, Console.CursorTop - 5);
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
                DrawString("Dealer has blackjack. Insurance pays out and you get $" + _player.Hands[0].Bet + ".");
                _player.PayInsurance();
            }
            else
            {
                DrawString("Dealer does not have blackjack. Insurance does not pay.");
            }
        }
        else
        {
            DrawEmptyLine();
        }
        if (_player.Surrendered)
        {
            DrawString("You have surrendered. You get $" + _player.Hands[0].Bet / 2 + " back.");
        }
        else
        {
            int handNo = 1;
            foreach (Hand hand in _player.Hands)
            {
                bool awaitInput = false;
                if (_player.Hands.Count > 1)
                {
                    DrawEmptyLine();
                    DrawString("Result for hand #" + handNo++ + ":");
                    awaitInput = true;
                }
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

                if (awaitInput)
                {
                    DrawString("Press any key to continue...");
                    DrawEmptyLine();
                    DrawSolidLine(2);
                    Console.ReadKey(true);
                    DrawHeader();
                    DrawPlayfield(_player, _dealer, _deck);
                }
            }
        }

        if (_player.Hands.Count > 1)
        {
            DrawEmptyLine();
            DrawEmptyLine();
        }

        DrawEmptyLine();
        DrawString("Press any key to start a new round...");
        DrawEmptyLine();
        DrawEmptyLine();
        DrawSolidLine(2);
        Console.ReadKey(true);
        DrawHeader();
    }
    #endregion

    #region UtilityFunctions
    internal void Reshuffle()
    {
        _deck.AddRange(_discardPile);
        _discardPile.Clear();
    }

    internal Card DrawACard()
    {
        Random rng = new Random();
        int cardNo = rng.Next(_deck.Count - 1);
        Card card = _deck[cardNo];
        _deck.Remove(card);

        if (_deck.Count <= 52)
        {
            Reshuffle();
        }

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

    internal void CreateDeck(bool stacked = false)
    {
        if (!stacked)
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
        else
        {
            _deck.Add(new Card(0, 0, 1));
            _deck.Add(new Card(0, 0, 1));
            _deck.Add(new Card(0, 0, 1));
            _deck.Add(new Card(0, 0, 1));
            _deck.Add(new Card(0, 0, 1));
            _deck.Add(new Card(0, 0, 1));
            _deck.Add(new Card(0, 0, 1));
            _deck.Add(new Card(0, 0, 1));
            _deck.Add(new Card(0, 0, 1));
        }
    }
    #endregion
}