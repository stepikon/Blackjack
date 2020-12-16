using System;
using System.Collections.Generic;
using System.Resources;
using System.Text;

namespace Blackjack
{
    abstract class Card
    {
        public abstract void AddToHand(Card card);
        public abstract void RemoveFromHand(Card card);
    }
}
