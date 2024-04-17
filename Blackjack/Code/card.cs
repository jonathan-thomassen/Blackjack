using System;

internal class Card {
    private readonly int _deckNo;
    private readonly Suits _suit;
    private readonly int _value;

    public int Value => _value;

    internal Card(int deckNo, Suits suit, int value)
    {
        _deckNo = deckNo;
        _suit = suit;
        _value = value;
    }

    internal void DrawCard()
    {
        switch (_suit)
        {
            case Suits.Spades:
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write("♠");
                    break;
                }
            case Suits.Hearts:
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write("♥");
                    break;
                }
            case Suits.Diamonds:
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write("♦");
                    break;
                }
            case Suits.Clubs:
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write("♣");
                    break;
                }
        }

        switch (Value)
        {
            case 1:
                {
                    Console.Write("A");
                    break;
                }

            case 11:
                {
                    Console.Write("J");
                    break;
                }

            case 12:
                {
                    Console.Write("Q");
                    break;
                }

            case 13:
                {
                    Console.Write("K");
                    break;
                }

            default:
                {
                    Console.Write(Value);
                    break;
                }
        }

        Console.ForegroundColor = ConsoleColor.White;        
    }

    public override string ToString()
    {
        string returnString = "Deck: ";

        returnString += _deckNo;

        returnString += ", Suit: ";
       
        switch (_suit)
        {
            case Suits.Spades:
            {
                returnString += "♠";
                break;
            }  
            case Suits.Hearts:
            {
                returnString += "♥";
                break;
            }
            case Suits.Diamonds:
            {
                returnString += "♦";
                break;
            }
            case Suits.Clubs:
            {
                returnString += "♣";
                break;
            }  
            default:
            {
                break;
            }            
        }

        returnString += ", Value: ";

        switch (Value)
        {
            case 1:
            {
                returnString += "A";
                break;
            }

            case 11:
            {
                returnString += "J";
                break;
            }

            case 12:
            {
                returnString += "Q";
                break;
            }

            case 13:
            {
                returnString += "K";
                break;
            }
            
            default:
            {
                returnString += Value;
                break;
            }
        }

        return returnString;
    }
}