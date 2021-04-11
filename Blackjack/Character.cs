using System;
using System.Collections.Generic;
using System.Text;

namespace Blackjack
{
    abstract class Character
    {
        public const int MINIMUM_WINDIW_WIDTH = 7 * 25;
        public const int MINIMUM_WINDIW_HEIGHT = 39;

        protected const string CHOICE_HIT = "hit";
        protected const string CHOICE_STAND = "stand";

        private string name;

        protected bool hasBlackjack;

        public List<Card> hand;

        //You can have up to 4 hands. https://doubledowncasino1.zendesk.com/hc/en-us/articles/204588494-Advanced-Blackjack-rules
        public List<Card>[] hands = new List<Card>[4];

        protected BetterUI betterUI;

        public Character(string name, List<Card> hand, BetterUI betterUI, bool hasBlackjack = false)
        { 
            this.name = name;
            this.hand = hand;
            this.betterUI = betterUI;
            this.hasBlackjack = hasBlackjack;

            hands[0] = hand;
        }

        public string Name { get { return name; } }

        public bool HasBlackjack { get { return hasBlackjack; } }

        public abstract Tuple<int, int, bool> GetHandValue(List<Card> hand);
        public abstract void SetSoftAceToHard(List<Card> hand);
        public abstract void SetHasBlackjack();
    }
}
