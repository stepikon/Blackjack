using System;
using System.Collections.Generic;
using System.Text;

namespace Blackjack
{
    abstract class Player:Character
    {
        protected const string CHOICE_DOUBLE = "double";
        protected const string CHOICE_SPLIT = "split";

        protected int chips;

        protected Tuple<int, int> tableLimits;
        protected int[] bets = { 0,0,0,0 }; //you can have up to 4 hands => you can have up to 4 bets. TODO: new int[4]

        public Player(string name, List<Card> hand, int chips, Tuple<int,int> tableLimits) :
            base(name, hand)
        {
            this.chips = chips;
            this.tableLimits = tableLimits;
        }

        public int Chips 
        {
            get
            { return chips; }
            set 
            {
                if (value<0)
                {
                    throw new ArgumentException();
                }
                else
                {
                    chips = value;
                }
            }
        }

        public abstract void TakeTurn(Dealer dealer);
        public abstract void Bet(List<Card> hand);
        public abstract void Bet(List<Card> hand, int bet);
        public abstract bool CheckBet(int bet);
        public abstract int GetBet(int index);
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

        public override void SetHasBlackjack()
        {
            if (hands[0].Count == 2 && (hands[1]==null && hands[2] == null && hands[3] == null)
                && (hands[0][0].GetCardValue() + hands[0][1].GetCardValue() == 21))
            {
                Console.WriteLine("BJ");
                hasBlackjack = true;
            }
            else
            {
                Console.WriteLine("no BJ");
                hasBlackjack = false;
            }
        }
    }
}
