using System;
using System.Collections.Generic;
using System.Text;

namespace Blackjack
{
    abstract class Player:Character
    {
        protected const string CHOICE_DOUBLE = "double";
        protected const string CHOICE_SPLIT = "split";
        protected const string CHOICE_SURRENDER = "surrender";

        protected bool isSurrenderAllowed;
        protected bool isDASAllowed; //double after split
        protected bool isResplitAllowed; //split up to 4 hands

        protected double chips;
        protected double insurance;
        protected int pairBet;

        protected int runningCount;
        protected double trueCount;

        protected bool surrender;

        private bool isRuined;
        private bool isGone;

        protected bool allowPairBets;

        protected Tuple<int, int> tableLimits;
        protected int[] bets = { 0,0,0,0 }; //you can have up to 4 hands => you can have up to 4 bets. TODO: new int[4]

        public Player(string name, List<Card> hand, BetterUI betterUI, double chips, Tuple<int,int> tableLimits,
            bool isSurrenderAllowed, bool isDASAllowed, bool isResplitAllowed,
            bool allowPairBets = true, bool isRuined = false, bool isGone = false) :
            base(name, hand, betterUI)
        {
            this.chips = chips;
            this.tableLimits = tableLimits;
            this.allowPairBets = allowPairBets;

            this.isSurrenderAllowed = isSurrenderAllowed;
            this.isDASAllowed = isDASAllowed;
            this.isResplitAllowed = isResplitAllowed;

            this.isRuined = isRuined;
            this.isGone = isGone;
        }

        public double Chips 
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

        public double Insurance
        {
            get
            { return insurance; }
        }

        public int PairBet
        {
            get
            { return pairBet; }
        }

        public int RunningCount
        {
            get
            { return runningCount; }
        }

        public double TrueCount
        {
            get
            { return trueCount; }
        }

        public bool IsRuined
        {
            get 
            { return isRuined; }
            set
            {
                if (value==true||value==false)
                {
                    isRuined = value;
                }
                else
                {
                    throw new ArgumentException();
                }
            }
        }

        public bool IsGone
        {
            get
            { return isGone; }
            set
            {
                if (value == true || value == false)
                {
                    isGone = value;
                }
                else
                {
                    throw new ArgumentException();
                }
            }
        }

        public bool Surrender
        {
            get { return surrender; }
        }

        public int[] Bets
        { get
            { return bets; } 
        }

        public abstract void TakeTurn(Player[] players, Dealer dealer);
        public abstract void Bet(List<Card> hand, Tuple<int, int> limits);
        public abstract void Bet(List<Card> hand, int bet, Tuple<int, int> limits);
        public abstract bool CheckBet(double bet, Tuple<int, int> limits);
        public abstract int GetBet(int index);
        public abstract void BetInsurance();
        public abstract void BetPair(Tuple<int, int> limits);
        public abstract void CountDealt(Player[] players, List<Card> dealerHand, double remainingDecks);
        public abstract void UpdateRunningCount(Player[] players, List<Card> dealerHand);
        public abstract void UpdateTrueCount(int runningCount, double remainingDecks);
        public abstract void UpdateTrueCount(double remainingDecks);
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
                SetSoftAceToHard(hand);

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

        public override void SetSoftAceToHard(List<Card> hand)
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
                hasBlackjack = true;
            }
            else
            {
                hasBlackjack = false;
            }
        }
    }
}
