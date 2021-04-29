using System;
using System.Collections.Generic;
using System.Text;

namespace Blackjack
{
    class CardTwo:Card
    {
        public CardTwo(string name, string suit, string color, ConsoleColor colorOfConsole)
            :base(name, suit, color, colorOfConsole)
        {
        }


        public override int GetCardValue()
        {
            return 2;
        }


        public override int GetCardCountValue()
        {
            return 1;
        }


        public override void Display()
        {
            Console.WriteLine(name);
        }
    }


    class CardThree:Card
    {
        public CardThree(string name, string suit, string color, ConsoleColor colorOfConsole)
            : base(name, suit, color, colorOfConsole)
        {
        }


        public override int GetCardValue()
        {
            return 3;
        }


        public override int GetCardCountValue()
        {
            return 1;
        }


        public override void Display()
        {
            Console.WriteLine(name);
        }
    }


    class CardFour : Card
    {
        public CardFour(string name, string suit, string color, ConsoleColor colorOfConsole)
            : base(name, suit, color, colorOfConsole)
        {
        }


        public override int GetCardValue()
        {
            return 4;
        }


        public override int GetCardCountValue()
        {
            return 1;
        }


        public override void Display()
        {
            Console.WriteLine(name);
        }
    }


    class CardFive : Card
    {
        public CardFive(string name, string suit, string color, ConsoleColor colorOfConsole)
            : base(name, suit, color, colorOfConsole)
        {
        }


        public override int GetCardValue()
        {
            return 5;
        }


        public override int GetCardCountValue()
        {
            return 1;
        }


        public override void Display()
        {
            Console.WriteLine(name);
        }
    }

    class CardSix : Card
    {
        public CardSix(string name, string suit, string color, ConsoleColor colorOfConsole)
            : base(name, suit, color, colorOfConsole)
        {
        }


        public override int GetCardValue()
        {
            return 6;
        }


        public override int GetCardCountValue()
        {
            return 1;
        }


        public override void Display()
        {
            Console.WriteLine(name);
        }
    }


    class CardSeven : Card
    {
        public CardSeven(string name, string suit, string color, ConsoleColor colorOfConsole)
            : base(name, suit, color, colorOfConsole)
        {
        }


        public override int GetCardValue()
        {
            return 7;
        }


        public override int GetCardCountValue()
        {
            return 0;
        }


        public override void Display()
        {
            Console.WriteLine(name);
        }
    }

    class CardEight : Card
    {
        public CardEight(string name, string suit, string color, ConsoleColor colorOfConsole)
            : base(name, suit, color, colorOfConsole)
        {
        }


        public override int GetCardValue()
        {
            return 8;
        }


        public override int GetCardCountValue()
        {
            return 0;
        }


        public override void Display()
        {
            Console.WriteLine(name);
        }
    }

    class CardNine : Card
    {
        public CardNine(string name, string suit, string color, ConsoleColor colorOfConsole)
            : base(name, suit, color, colorOfConsole)
        {
        }


        public override int GetCardValue()
        {
            return 9;
        }


        public override int GetCardCountValue()
        {
            return 0;
        }


        public override void Display()
        {
            Console.WriteLine(name);
        }
    }

    class CardTen : Card
    {
        public CardTen(string name, string suit, string color, ConsoleColor colorOfConsole)
            : base(name, suit, color, colorOfConsole)
        {
        }


        public override int GetCardValue()
        {
            return 10;
        }


        public override int GetCardCountValue()
        {
            return -1;
        }


        public override void Display()
        {
            Console.WriteLine(name);
        }
    }

    class CardJack : Card
    {
        public CardJack(string name, string suit, string color, ConsoleColor colorOfConsole)
            : base(name, suit, color, colorOfConsole)
        {
        }


        public override int GetCardValue()
        {
            return 10;
        }


        public override int GetCardCountValue()
        {
            return -1;
        }


        public override void Display()
        {
            Console.WriteLine(name);
        }
    }

    class CardQueen : Card
    {
        public CardQueen(string name, string suit, string color, ConsoleColor colorOfConsole)
            : base(name, suit, color, colorOfConsole)
        {
        }


        public override int GetCardValue()
        {
            return 10;
        }


        public override int GetCardCountValue()
        {
            return -1;
        }


        public override void Display()
        {
            Console.WriteLine(name);
        }
    }

    class CardKing : Card
    {
        public CardKing(string name, string suit, string color, ConsoleColor colorOfConsole)
            : base(name, suit, color, colorOfConsole)
        {
        }


        public override int GetCardValue()
        {
            return 10;
        }


        public override int GetCardCountValue()
        {
            return -1;
        }


        public override void Display()
        {
            Console.WriteLine(name);
        }
    }

    class CardAce : Card
    {
        private bool aceIsOne;

        public CardAce(string name, string suit, string color, ConsoleColor colorOfConsole, bool aceIsOne)
            : base(name, suit, color, colorOfConsole)
        {
            this.aceIsOne = aceIsOne;
        }


        public bool AceIsOne {
            get { return aceIsOne; }
            set
            {
                if (value==true || value == false)
                {
                    aceIsOne = value;
                }
                else
                {
                    throw new ArgumentException();
                }
            }
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


        public override void Display()
        {
            Console.WriteLine(name);
        }
    }
}
