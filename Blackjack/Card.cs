using System;
using System.Collections.Generic;
using System.Resources;
using System.Text;

namespace Blackjack
{
    class Card
    {
        private string rank;
        private string suit;
        private string color;

        private ConsoleColor colorOfConsole;

        public Card(string rank, string suit, string color, ConsoleColor colorOfConsole) 
        {
            this.rank = rank;
            this.suit = suit;
            this.color = color;
            this.colorOfConsole = colorOfConsole;
        }

        public string Rank 
        {
            get { return rank; }
        }

        public string Suit
        {
            get { return suit; }
        }

        public ConsoleColor C
        { 
            get { return colorOfConsole; } 
        }

        public string Color
        { 
            get { return color; } 
        }

        public int GetCardValue(bool isOne)
        {
            switch (rank)
            {
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                    return int.Parse(rank);
                case "10":
                case "J":
                case "Q":
                case "K":
                    return 10;
                default:
                    return 11;
            }
        }
    }
}
