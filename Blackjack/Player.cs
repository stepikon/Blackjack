using System;
using System.Collections.Generic;
using System.Text;

namespace Blackjack
{
    abstract class Player:Character
    {
        public Player(string name):
            base(name)
        { }
    }
}
