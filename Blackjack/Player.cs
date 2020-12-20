using System;
using System.Collections.Generic;
using System.Text;

namespace Blackjack
{
    abstract class Player:Character
    {
        protected const string CHOICE_DOUBLE = "double";
        protected const string CHOICE_SPLIT = "split";

        public Player(string name, List<Card> hand) :
            base(name, hand)
        {
        }

        public abstract void TakeTurn(Dealer dealer);
        public abstract void CountDealt();
        public abstract void UpdateRunningCount();
        public abstract void UpdateTrueCount();
        public abstract void ResetCounts();
        public abstract void ResetHands();
        public abstract void DisplayHands();
        public abstract List<int> GetHandValues();

        public override Tuple<int, int, bool> GetHandValue(List<Card> hand)
        {
            int handValue = 0;
            bool hasSoftAce = false;

            foreach (Card c in hand)
            {
                handValue += c.GetCardValue();
            }

            foreach (Card c in hand)
            {
                if (c is CardAce)
                {
                    CardAce cA = (CardAce)c;
                    if (!cA.AceIsOne)
                    {
                        hasSoftAce = true;
                        break;
                    }
                }
            }

            if (handValue > 21 && hasSoftAce) //since 11+11=22, one hand can only contain 1 soft ace.
            {
                SetSoftAceToHard();

                handValue = 0;
                hasSoftAce = false;
                foreach (Card c in hand)
                {
                    handValue += c.GetCardValue();
                }

                foreach (Card c in hand)
                {
                    if (c is CardAce)
                    {
                        CardAce cA = (CardAce)c;
                        if (!cA.AceIsOne)
                        {
                            hasSoftAce = true;
                            break;
                        }
                    }
                }
            }

            return new Tuple<int, int, bool>(hasSoftAce ? handValue - 10 : handValue, handValue, hasSoftAce);
        }

        public override void SetSoftAceToHard()
        {
            foreach (Card c in hand)
            {
                if (c is CardAce)
                {
                    CardAce cA = (CardAce)c;
                    if (!cA.AceIsOne)
                    {
                        cA.AceIsOne = true;
                        break;
                    }
                }
            }
        }
    }
}
