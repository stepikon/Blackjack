﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;

namespace Blackjack
{
    class HumanPlayer:Player
    {
        public HumanPlayer(string name, List<Card> hand, int chips, Tuple<int,int> tableLimits) :
            base(name, hand, chips, tableLimits)
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
            string choice;
            bool isDouble = false;

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

                    if (GetHandValue(hands[i]).Item1 >= 21 || GetHandValue(hands[i]).Item2 >= 21 || hasBlackjack) //obvious choice; you always stand on 21 and you lose after going over 21.
                    {
                        choice = CHOICE_STAND;
                    }
                    else if (isDouble||(hands[i].Count==1)&&hands[i][0] is CardAce) //after doubling you only get 1 card. The same rule apply if you split AA.
                    {
                        Hit(dealer, i);
                        DisplayHands();
                        Console.WriteLine("Last card on hand {0}", i);
                        choice = CHOICE_STAND;
                        isDouble = false;
                    }
                    else if (hands[i].Count == 1) //automatic choice, you are always dealt a card do BOTH hands after splitting.
                    {
                        choice = CHOICE_HIT;
                    }
                    else
                    {
                        choice = GetChoice();
                    }

                    switch (choice)
                    {
                        case CHOICE_HIT:
                            Hit(dealer, i);
                            break;
                        case CHOICE_SPLIT:
                            Split(hands[i]);
                            break;
                        case CHOICE_DOUBLE:
                            if (hands[i].Count == 2 && chips >= 0)
                            {
                                Double(i, new Tuple<int, int>(0, bets[i]));
                                isDouble = true;
                            }
                            else
                            {
                                Console.WriteLine("Cannot double");
                            }                           
                            break;
                        case CHOICE_STAND:
                            Stand();
                            break;
                        default:
                            break;
                    }
                } while (choice != CHOICE_STAND);
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

        public override void Bet(List<Card> hand, Tuple<int, int> limits)
        {
            int bet;

            do
            {
                Console.WriteLine("How much do you want to bet?\n" +
                    "You have {0} chips, bet limits are {1}-{2}", chips, limits.Item1, limits.Item2);
            } while (!(int.TryParse(Console.ReadLine(), out bet) && bet >= limits.Item1 && bet <= limits.Item2 && CheckBet(bet, limits)));

            bets[Array.IndexOf(hands, hand)] += bet;
            chips -= bet;
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
            string choice;

            do
            {
                Console.WriteLine("Insurance is open. Bet? (Y/N)");
                switch (Console.ReadKey().KeyChar)
                {
                    case 'y':
                    case 'Y':
                        choice = "yes";
                        break;
                    case 'n':
                    case 'N':
                        choice = "no";
                        break;
                    default:
                        choice = "";
                        break;
                }
            } while (!(choice == "yes" || choice == "no"));

            if (choice == "yes" && CheckBet(bets[0] * 0.5, new Tuple<int, int>(0, tableLimits.Item2)))
            {
                insurance = bets[0] * 0.5;
                chips -= insurance;
            }
        }

        public override void BetPair(Tuple<int, int> limits)
        {
            if (allowPairBets)
            {
                int bet;

                do
                {
                    Console.WriteLine("How much do you want to bet?\n" +
                        "You have {0} chips, bet limits are {1}-{2}", chips, limits.Item1, limits.Item2);
                    Console.WriteLine("Bet 0 to skip this bet.");
                } while (!((int.TryParse(Console.ReadLine(), out bet) && bet >= limits.Item1 && bet <= limits.Item2 && CheckBet(bet, limits)) || bet==0));

                if (bet==0)
                {
                    string choice;

                    do
                    {
                        Console.WriteLine("Allow this side bet for future rounds? (Y/N)");
                        switch (Console.ReadKey().KeyChar)
                        {
                            case 'y':
                            case 'Y':
                                choice = "yes";
                                break;
                            case 'n':
                            case 'N':
                                choice = "no";
                                break;
                            default:
                                choice = "";
                                break;
                        }
                    } while (!(choice == "yes" || choice == "no"));

                    if (choice == "no")
                    {
                        allowPairBets = false;
                    }
                }

                pairBet = bet;
                chips -= bet;
            }
        }

        private void Hit(Dealer dealer, int handIndex)
        {
            dealer.Deal(this, handIndex);
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

                if (temp is CardAce)
                {
                    CardAce cA = (CardAce)temp;
                    CardAce cA2 = (CardAce)hand[0];

                    cA.AceIsOne = false;
                    cA2.AceIsOne = false;
                }

                hand.Remove(hand[1]);
                hands[newHandIndex] = new List<Card>();
                hands[newHandIndex].Add(temp);
                Bet(hands[newHandIndex], bets[Array.IndexOf(hands, hand)], tableLimits);
            }
        }

        private void Double(int index, Tuple<int, int> limits)
        {
            if (chips >= limits.Item1)
            {
                Bet(hands[index], limits);
            }
            else
            {
                Console.WriteLine("Cannot double");
            }
        }

        //splitting is valid if and only if it contains exactly 2 cards of the same VALUE (not rank!!) and the hand has not been splitted twice.
        private int IsSplittingValid(List<Card> hand)
        {
            switch (Array.IndexOf(hands, hand))
            {
                case 0:
                    if (hand.Count==2
                        && ((hand[0].GetCardValue()==hand[1].GetCardValue()) || (hand[0].GetType()==hand[1].GetType())) //GetType for AA hands
                        &&(hands[1]==null||hands[2]==null)
                        &&CheckBet(bets[0], tableLimits))
                    {
                        return hands[1]==null? 1 : 2;
                    }
                    else
                    {
                        return -1;
                    }
                case 1:
                    if (hand.Count == 2
                        && ((hand[0].GetCardValue() == hand[1].GetCardValue()) || (hand[0].GetType() == hand[1].GetType()))
                        && (hands[2] == null || hands[3] == null)
                        && CheckBet(bets[1], tableLimits))
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
            bets[0] = 0;

            for (int i = 1; i < hands.Length; i++)
            {
                hands[i] = null;
                bets[i] = 0;
            }

            insurance = 0;
            pairBet = 0;
            hasBlackjack = false;
        }
    }
}
