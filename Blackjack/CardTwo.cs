using System;
using System.Collections.Generic;
using System.Text;

namespace Blackjack
{
    class CardTwo:CardFace
    {
        public CardTwo(string name, string suit, string color, ConsoleColor colorOfConsole)
            :base(name, suit, color, colorOfConsole)
        { }

        public override int GetCardValue()
        {
            return 2;
        }

        public override int GetCardCountValue()
        {
            return 1;
        }
    }

    class CardThree: CardFace
    {
        public CardThree(string name, string suit, string color, ConsoleColor colorOfConsole)
            : base(name, suit, color, colorOfConsole)
        { }

        public override int GetCardValue()
        {
            return 3;
        }

        public override int GetCardCountValue()
        {
            return 1;
        }
    }

    class CardFour : CardFace
    {
        public CardFour(string name, string suit, string color, ConsoleColor colorOfConsole)
            : base(name, suit, color, colorOfConsole)
        { }

        public override int GetCardValue()
        {
            return 4;
        }

        public override int GetCardCountValue()
        {
            return 1;
        }
    }

    class CardFive : CardFace
    {
        public CardFive(string name, string suit, string color, ConsoleColor colorOfConsole)
            : base(name, suit, color, colorOfConsole)
        { }

        public override int GetCardValue()
        {
            return 5;
        }

        public override int GetCardCountValue()
        {
            return 1;
        }
    }

    class CardSix : CardFace
    {
        public CardSix(string name, string suit, string color, ConsoleColor colorOfConsole)
            : base(name, suit, color, colorOfConsole)
        { }

        public override int GetCardValue()
        {
            return 6;
        }

        public override int GetCardCountValue()
        {
            return 1;
        }
    }

    class CardSeven : CardFace
    {
        public CardSeven(string name, string suit, string color, ConsoleColor colorOfConsole)
            : base(name, suit, color, colorOfConsole)
        { }

        public override int GetCardValue()
        {
            return 7;
        }

        public override int GetCardCountValue()
        {
            return 0;
        }
    }

    class CardEight : CardFace
    {
        public CardEight(string name, string suit, string color, ConsoleColor colorOfConsole)
            : base(name, suit, color, colorOfConsole)
        { }

        public override int GetCardValue()
        {
            return 8;
        }

        public override int GetCardCountValue()
        {
            return 0;
        }
    }

    class CardNine : CardFace
    {
        public CardNine(string name, string suit, string color, ConsoleColor colorOfConsole)
            : base(name, suit, color, colorOfConsole)
        { }

        public override int GetCardValue()
        {
            return 9;
        }

        public override int GetCardCountValue()
        {
            return 0;
        }
    }

    class CardTen : CardFace
    {
        public CardTen(string name, string suit, string color, ConsoleColor colorOfConsole)
            : base(name, suit, color, colorOfConsole)
        { }

        public override int GetCardValue()
        {
            return 10;
        }

        public override int GetCardCountValue()
        {
            return -1;
        }
    }

    class CardJack : CardFace
    {
        public CardJack(string name, string suit, string color, ConsoleColor colorOfConsole)
            : base(name, suit, color, colorOfConsole)
        { }

        public override int GetCardValue()
        {
            return 10;
        }

        public override int GetCardCountValue()
        {
            return -1;
        }
    }

    class CardQueen : CardFace
    {
        public CardQueen(string name, string suit, string color, ConsoleColor colorOfConsole)
            : base(name, suit, color, colorOfConsole)
        { }

        public override int GetCardValue()
        {
            return 10;
        }

        public override int GetCardCountValue()
        {
            return -1;
        }
    }

    class CardKing : CardFace
    {
        public CardKing(string name, string suit, string color, ConsoleColor colorOfConsole)
            : base(name, suit, color, colorOfConsole)
        { }

        public override int GetCardValue()
        {
            return 10;
        }

        public override int GetCardCountValue()
        {
            return -1;
        }
    }

    class CardAce : CardFace
    {
        private bool aceIsOne;

        public CardAce(string name, string suit, string color, ConsoleColor colorOfConsole, bool aceIsOne)
            : base(name, suit, color, colorOfConsole)
        {
            this.aceIsOne = aceIsOne;
        }

        public override int GetCardValue()
        {
            if (aceIsOne)
            {
                return 1;
            }
            else
            {
                return 11;
            }
        }

        public override int GetCardCountValue()
        {
            return -1;
        }
    }
}
