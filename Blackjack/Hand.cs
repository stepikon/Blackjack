using System;
using System.Collections.Generic;
using System.Text;

namespace Blackjack
{
    class Hand:Card
    {
        public List<Card> hand = new List<Card>();

        public override void AddToHand(Card card)
        {
            hand.Add(card);
        }

        public override void RemoveFromHand(Card card)
        {
            hand.Remove(card);
        }
    }
}
