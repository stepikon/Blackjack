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

        public ConsoleColor C
        {
            get { return colorOfConsole; }
        }

        public string Color
        {
            get { return color; }
        }

        public abstract int GetCardValue();
        public abstract int GetCardCountValue();
        public abstract void Display();
    }
}
