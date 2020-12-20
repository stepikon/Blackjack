using System;
using System.Collections.Generic;
using System.Text;

namespace Blackjack
{
    abstract class Character
    {

        protected const string CHOICE_HIT = "hit";
        protected const string CHOICE_STAND = "stand";

        private string name;

        public List<Card> hand;

        //You can have up to 4 hands. https://doubledowncasino1.zendesk.com/hc/en-us/articles/204588494-Advanced-Blackjack-rules
        public List<Card>[] hands = new List<Card>[4];

        public Character(string name, List<Card> hand)
        { 
            this.name = name;
            this.hand = hand;

            hands[0] = hand;
        }

        public abstract Tuple<int, int, bool> GetHandValue(List<Card>hand);
        public abstract void SetSoftAceToHard();
        public abstract string GetChoice();
    }
}
