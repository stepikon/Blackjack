using System;
using System.Collections.Generic;
using System.Text;

namespace Blackjack
{
    class CardTwo:Card
    {
        public CardTwo(string name, string suit, string color, ConsoleColor colorOfConsole)
            :base(name, suit, color, colorOfConsole)
        { }
    }

    class CardThree:Card
    {
        public CardThree(string name, string suit, string color, ConsoleColor colorOfConsole)
            : base(name, suit, color, colorOfConsole)
        { }
    }

    class CardFour : Card
    {
        public CardFour(string name, string suit, string color, ConsoleColor colorOfConsole)
            : base(name, suit, color, colorOfConsole)
        { }
    }

    class CardFive : Card
    {
        public CardFive(string name, string suit, string color, ConsoleColor colorOfConsole)
            : base(name, suit, color, colorOfConsole)
        { }
    }

    class CardSix : Card
    {
        public CardSix(string name, string suit, string color, ConsoleColor colorOfConsole)
            : base(name, suit, color, colorOfConsole)
        { }
    }

    class CardSeven : Card
    {
        public CardSeven(string name, string suit, string color, ConsoleColor colorOfConsole)
            : base(name, suit, color, colorOfConsole)
        { }
    }

    class CardEight : Card
    {
        public CardEight(string name, string suit, string color, ConsoleColor colorOfConsole)
            : base(name, suit, color, colorOfConsole)
        { }
    }

    class CardNine : Card
    {
        public CardNine(string name, string suit, string color, ConsoleColor colorOfConsole)
            : base(name, suit, color, colorOfConsole)
        { }
    }

    class CardTen : Card
    {
        public CardTen(string name, string suit, string color, ConsoleColor colorOfConsole)
            : base(name, suit, color, colorOfConsole)
        { }
    }

    class CardJack : Card
    {
        public CardJack(string name, string suit, string color, ConsoleColor colorOfConsole)
            : base(name, suit, color, colorOfConsole)
        { }
    }

    class CardQueen : Card
    {
        public CardQueen(string name, string suit, string color, ConsoleColor colorOfConsole)
            : base(name, suit, color, colorOfConsole)
        { }
    }

    class CardKing : Card
    {
        public CardKing(string name, string suit, string color, ConsoleColor colorOfConsole)
            : base(name, suit, color, colorOfConsole)
        { }
    }

    class CardAce : Card
    {
        private bool aceIsOne;

        public CardAce(string name, string suit, string color, ConsoleColor colorOfConsole, bool aceIsOne)
            : base(name, suit, color, colorOfConsole)
        {
            this.aceIsOne = aceIsOne;
        }
    }
}
