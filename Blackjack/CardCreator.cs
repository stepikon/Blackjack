using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace Blackjack
{
    //part of a <Card Factory pattern>
    //STRUKTURU FACTORY PATTERNU JSEM PREVZAL Z https://www.dofactory.com/net/factory-method-design-pattern

    class CardCreator
    {
        public Card CreateCard(string name, string suit, string color, ConsoleColor consoleColor)
        {
            switch (name)
            {
                case "2":
                    return new CardTwo(name, suit, color, consoleColor);
                case "3":
                    return new CardThree(name, suit, color, consoleColor);
                case "4":
                    return new CardFour(name, suit, color, consoleColor);
                case "5":
                    return new CardFive(name, suit, color, consoleColor);
                case "6":
                    return new CardSix(name, suit, color, consoleColor);
                case "7":
                    return new CardSeven(name, suit, color, consoleColor);
                case "8":
                    return new CardEight(name, suit, color, consoleColor);
                case "9":
                    return new CardNine(name, suit, color, consoleColor);
                case "10":
                    return new CardTen(name, suit, color, consoleColor);
                case "J":
                    return new CardJack(name, suit, color, consoleColor);
                case "Q":
                    return new CardQueen(name, suit, color, consoleColor);
                case "K":
                    return new CardKing(name, suit, color, consoleColor);
                default:
                    return new CardAce(name, suit, color, consoleColor, false);
            }
        }
    }

    //</Card Factory pattern>
}
