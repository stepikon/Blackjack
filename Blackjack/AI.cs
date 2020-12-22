using System;
using System.Collections.Generic;
using System.Text;

namespace Blackjack
{
    class AI:Player
    {
        private int runningCount;
        private int trueCount;

        public AI(string name, List<Card> hand, int chips, Tuple<int, int> tableLimits) :
            base(name, hand, chips, tableLimits)
        { }

        public override void TakeTurn(Dealer dealer)
        {
            throw new NotImplementedException();
        }

        public override string GetChoice()
        {
            throw new NotImplementedException();
        }

        public override void CountDealt()
        {
            throw new NotImplementedException();
        }

        public override void UpdateRunningCount()
        {
            throw new NotImplementedException();
        }

        public override void UpdateTrueCount()
        {
            throw new NotImplementedException();
        }

        public override void ResetCounts()
        {
            runningCount = 0;
            trueCount = 0;
        }

        public override void Bet(List<Card> hand, Tuple<int, int> limits)
        {
            throw new NotImplementedException();
        }

        public override void Bet(List<Card> hand, int bet, Tuple<int, int> limits)
        {
            if (CheckBet(bet, limits))
            {
                bets[Array.IndexOf(hands, hand)] += bet;
                chips -= bet;
            }
        }

        public override bool CheckBet(double bet, Tuple<int, int> limits)
        {
            if (bet > chips || bet < limits.Item1 || bet > limits.Item2)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public override int GetBet(int index)
        {
            return bets[index];
        }

        public override void BetInsurance()
        {
            throw new NotImplementedException();
        }

        public override void DisplayHands()
        {
            for (int i = 0; i < hands.Length; i++)
            {
                if (hands[i] != null)
                {
                    foreach (Card c in hands[i])
                    {
                        Console.WriteLine("hand {0}: {1}", i, c.Name);
                    }
                }
            }
        }

        public override List<int> GetHandValues()
        {
            List<int> handValues = new List<int>();

            for (int i = 0; i < hands.Length; i++)
            {
                if (hands[i] != null)
                {
                    handValues.Add(GetHandValue(hands[i]).Item2);
                }
            }

            return handValues;
        }

        public override void ResetHands()
        {
            hands[0].Clear();

            for (int i = 1; i < hands.Length; i++)
            {
                hands[i] = null;
            }
        }
    }
}
