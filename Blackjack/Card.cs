using System;
using System.Collections.Generic;
using System.Resources;
using System.Text;

namespace Blackjack
{
    abstract class Card
    {
        protected string name;
        protected string suit;
        protected string color;

        protected ConsoleColor colorOfConsole;

        public Card(string name, string suit, string color, ConsoleColor colorOfConsole)
        {
            this.name = name;
            this.suit = suit;
            this.color = color;
            this.colorOfConsole = colorOfConsole;
        }


        public string Name
        {
            get { return name; }
        }


        public string Suit
        {
            get { return suit; }
        }


        public ConsoleColor ColorOfConsole
        {
            get { return colorOfConsole; }
        }


        public string Color
        {
            get { return color; }
        }

        
        public abstract int GetCardValue(); //2-9 are worth 2-9 accordingly, 10-K are worth 10, A is worth 1 or 11
        public abstract int GetCardCountValue(); //2-6 => +1, 7-9 =>0, 10-A => -1
        public abstract void Display(); //displays card.Name
    }
}
