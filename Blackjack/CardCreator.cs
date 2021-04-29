using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace Blackjack
{
    //part of a <Card Factory pattern>
    //STRUKTURU FACTORY PATTERNU JSEM PREVZAL Z https://www.dofactory.com/net/factory-method-design-pattern

    abstract class CardCreator
    {
        protected string name;


        public CardCreator(string name)
        {
            this.name = name;
        }


        public abstract Card CreateCard(string suit, string color, ConsoleColor consoleColor);
    }


    class CardTwoCreator : CardCreator
    {
        public CardTwoCreator(string name) : base(name)
        { 
        }


        public override Card CreateCard(string suit, string color, ConsoleColor consoleColor)
        {
            return new CardTwo(name, suit, color, consoleColor);
        }
    }


    class CardThreeCreator : CardCreator
    {
        public CardThreeCreator(string name) : base(name)
        {
        }


        public override Card CreateCard(string suit, string color, ConsoleColor consoleColor)
        {
            return new CardThree(name, suit, color, consoleColor);
        }
    }


    class CardFourCreator : CardCreator
    {
        public CardFourCreator(string name) : base(name)
        {
        }


        public override Card CreateCard(string suit, string color, ConsoleColor consoleColor)
        {
            return new CardFour(name, suit, color, consoleColor);
        }
    }


    class CardFiveCreator : CardCreator
    {
        public CardFiveCreator(string name) : base(name)
        {
        }


        public override Card CreateCard(string suit, string color, ConsoleColor consoleColor)
        {
            return new CardFive(name, suit, color, consoleColor);
        }
    }


    class CardSixCreator : CardCreator
    {
        public CardSixCreator(string name) : base(name)
        {
        }


        public override Card CreateCard(string suit, string color, ConsoleColor consoleColor)
        {
            return new CardSix(name, suit, color, consoleColor);
        }
    }


    class CardSevenCreator : CardCreator
    {
        public CardSevenCreator(string name) : base(name)
        {
        }


        public override Card CreateCard(string suit, string color, ConsoleColor consoleColor)
        {
            return new CardSeven(name, suit, color, consoleColor);
        }
    }


    class CardEightCreator : CardCreator
    {
        public CardEightCreator(string name) : base(name)
        {
        }


        public override Card CreateCard(string suit, string color, ConsoleColor consoleColor)
        {
            return new CardEight(name, suit, color, consoleColor);
        }
    }


    class CardNineCreator : CardCreator
    {
        public CardNineCreator(string name) : base(name)
        {
        }


        public override Card CreateCard(string suit, string color, ConsoleColor consoleColor)
        {
            return new CardNine(name, suit, color, consoleColor);
        }
    }


    class CardTenCreator : CardCreator
    {
        public CardTenCreator(string name) : base(name)
        {
        }


        public override Card CreateCard(string suit, string color, ConsoleColor consoleColor)
        {
            return new CardTen(name, suit, color, consoleColor);
        }
    }


    class CardJackCreator : CardCreator
    {
        public CardJackCreator(string name) : base(name)
        {
        }


        public override Card CreateCard(string suit, string color, ConsoleColor consoleColor)
        {
            return new CardJack(name, suit, color, consoleColor);
        }
    }


    class CardQueenCreator : CardCreator
    {
        public CardQueenCreator(string name) : base(name)
        {
        }


        public override Card CreateCard(string suit, string color, ConsoleColor consoleColor)
        {
            return new CardQueen(name, suit, color, consoleColor);
        }
    }


    class CardKingCreator : CardCreator
    {
        public CardKingCreator(string name) : base(name)
        {
        }


        public override Card CreateCard(string suit, string color, ConsoleColor consoleColor)
        {
            return new CardKing(name, suit, color, consoleColor);
        }
    }


    class CardAceCreator : CardCreator
    {
        bool aceIsOne;


        public CardAceCreator(string name, bool aceIsOne) : base(name)
        {
            this.aceIsOne = aceIsOne;
        }


        public override Card CreateCard(string suit, string color, ConsoleColor consoleColor)
        {
            return new CardAce(name, suit, color, consoleColor, aceIsOne);
        }
    }

    //</Card Factory pattern>
}
