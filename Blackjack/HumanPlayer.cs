using System;
using System.Collections.Generic;
using System.Text;

namespace Blackjack
{
    class HumanPlayer:Player
    {
        private int chips;


        public HumanPlayer(string name, Card hand) :
            base(name, hand)
        { }
    }
}
