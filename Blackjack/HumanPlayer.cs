using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Text;

namespace Blackjack
{
    class HumanPlayer:Player
    {
        public HumanPlayer(string name, List<Card> hand) :
            base(name, hand)
        { }

        public override string GetChoice()
        {
            switch (Console.ReadLine().ToLower())
            {
                case "h":
                    return CHOICE_HIT;
                case "s":
                    return CHOICE_STAND;
                case "sp":
                    return CHOICE_SPLIT;
                case "do":
                    return CHOICE_DOUBLE;
                default:
                    return "";
            }
        }

        public override void TakeTurn(Dealer dealer)
        {
            string choice = "";

            for (int i = 0; i<hands.Length; i++)
            {
                if (hands[i]==null)
                {
                    continue;
                }

                do
                {
                    DisplayHands();
                    Console.WriteLine("{0} or {1} (soft ace: {2})",
                        GetHandValue(hands[i]).Item1, GetHandValue(hands[i]).Item2, GetHandValue(hands[i]).Item3);
                    choice = GetChoice();

                    switch (choice)
                    {
                        case CHOICE_HIT:
                            Hit(dealer, i);
                            break;
                        case CHOICE_SPLIT:
                            Split(hands[i]);
                            break;
                        default:
                            break;
                    }
                } while (choice != "stand");
            }
        }

        public override void CountDealt()
        {
            Console.WriteLine("Human does this on his own");
        }

        public override void UpdateRunningCount()
        {
            Console.WriteLine("Human does this on his own");
        }

        public override void UpdateTrueCount()
        {
            Console.WriteLine("Human does this on his own");
        }

        public override void ResetCounts()
        {
            Console.WriteLine("Human does this on his own");
        }

        private void Hit(Dealer dealer, int hand)
        {
            dealer.Deal(this, hand);
        }

        private void Stand()
        { 
        }

        private void Split(List<Card> hand)
        {
            int newHandIndex = IsSplittingValid(hand);

            if (newHandIndex!=-1)
            {
                Card temp = hand[1];
                hand.Remove(hand[1]);
                hands[newHandIndex] = new List<Card>();
                hands[newHandIndex].Add(temp);
            }
        }

        //splitting is valid if and only if there are exactly 2 cards of the same VALUE (not rank!!) and the hand has not been splitted twice.
        private int IsSplittingValid(List<Card> hand)
        {
            switch (Array.IndexOf(hands, hand))
            {
                case 0:
                    if (hand.Count==2&&(hand[0].GetCardValue()==hand[1].GetCardValue())
                        &&(hands[1]==null||hands[2]==null))
                    {
                        return hands[1]==null? 1 : 2;
                    }
                    else
                    {
                        return -1;
                    }
                case 1:
                    if (hand.Count == 2 && (hand[0].GetCardValue() == hand[1].GetCardValue())
                        && (hands[2] == null || hands[3] == null))
                    {
                        return hands[2] == null ? 2 : 3;
                    }
                    else
                    {
                        return -1;
                    }
                case 2: //splitted twice
                    return -1;
                case 3: //splitted twice
                    return -1;
                default:
                    return -1;
            }
        }

        public override List<int> GetHandValues()
        {
            List<int> handValues = new List<int>();

            for (int i = 0; i < hands.Length; i++)
            {
                if (hands[i]!=null)
                {
                    handValues.Add(GetHandValue(hands[i]).Item2);
                }
            }

            return handValues;
        }

        public override void DisplayHands()
        {
            for (int i = 0; i < hands.Length; i++)
            {
                if (hands[i]!=null)
                {
                    foreach (Card c in hands[i])
                    {
                        Console.WriteLine("hand {0}: {1}",i, c.Name);
                    }
                }               
            }
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
