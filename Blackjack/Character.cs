using System;
using System.Collections.Generic;
using System.Text;

namespace Blackjack
{
    abstract class Character
    {
        private string name;

        private Card hand;

        public Character(string name)
        { 
            this.name = name;
        }
    }
}
