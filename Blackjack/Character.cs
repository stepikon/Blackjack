using System;
using System.Collections.Generic;
using System.Text;

namespace Blackjack
{
    abstract class Character
    {
        private string name;

        public Card hand;

        public Character(string name, Card hand)
        { 
            this.name = name;
            this.hand = hand;
        }
    }
}
