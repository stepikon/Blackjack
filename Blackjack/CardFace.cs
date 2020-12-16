using System;
using System.Collections.Generic;
using System.Text;

namespace Blackjack
{
    abstract class CardFace:Card
    {
        private string name;
        private string suit;
        private string color;

        private ConsoleColor colorOfConsole;

        public CardFace(string name, string suit, string color, ConsoleColor colorOfConsole)
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

        public override void AddToHand(Card card)
        {
            Console.WriteLine("Cannot add to {0}", card);
        }

        public override void RemoveFromHand(Card card)
        {
            Console.WriteLine("Cannot remove from {0}", card);
        }
    }
}
