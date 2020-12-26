using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Blackjack
{
    class CardCountingAI:Player
    {
        private int betUnit;

        public CardCountingAI(string name, List<Card> hand, int chips, Tuple<int, int> tableLimits, int betUnit,
            int runningCount = 0, double trueCount = 0) :
            base(name, hand, chips, tableLimits)
        {
            this.runningCount = runningCount;
            this.trueCount = trueCount;
            this.betUnit = betUnit;
        }

        public override void TakeTurn(Player[] players, Dealer dealer)
        {
            string choice;
            bool isDouble = false;

            for (int i = 0; i < hands.Length; i++)
            {
                if (hands[i] == null)
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
                    else if (isDouble || (hands[i].Count == 1 && hands[i][0] is CardAce)) //after doubling you only get 1 card. The same rule apply if you split AA.
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
                        choice = GetChoice(hands[i], tableLimits, players, dealer);
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
                                choice = CHOICE_HIT;
                                Hit(dealer, i);
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

        //Basic strategy data: https://wizardofodds.com/games/blackjack/strategy/4-decks/
        //Deviations data: https://www.reddit.com/r/blackjack/comments/5fgf1a/deviations/, https://quizlet.com/18561678/blackjack-h17-deviations-flash-cards/
        public string GetChoice(List<Card> hand, Tuple<int, int> limits, Player[] players, Dealer dealer)
        {
            if (hand != null && dealer.hand[0] != null)
            {
                //check for split opportunity first
                if (IsSplittingValid(hand) != -1)
                {
                    switch (hand[0].GetCardValue())
                    {
                        case 2:
                        case 3:
                        case 7:
                            if ((dealer.hand[0].GetCardValue() > 1 && dealer.hand[0].GetCardValue() <= 7)
                                && CheckBet(bets[Array.IndexOf(hands, hand)], limits))
                            {
                                return CHOICE_SPLIT;
                            }
                            else
                            {
                                break;
                            }
                        case 4:
                            if ((dealer.hand[0].GetCardValue() == 5 || dealer.hand[0].GetCardValue() == 6)
                                && CheckBet(bets[Array.IndexOf(hands, hand)], limits))
                            {
                                return CHOICE_SPLIT;
                            }
                            else
                            {
                                break;
                            }
                        case 5:
                            break;
                        case 6:
                            if (dealer.hand[0].GetCardValue() >= 2 && dealer.hand[0].GetCardValue() <= 6
                                && CheckBet(bets[Array.IndexOf(hands, hand)], limits))
                            {
                                return CHOICE_SPLIT;
                            }
                            else
                            {
                                break;
                            }
                        case 8:
                            if (CheckBet(bets[Array.IndexOf(hands, hand)], limits))
                            {
                                return CHOICE_SPLIT;
                            }
                            else
                            {
                                break;
                            }
                        case 9:
                            if (!(dealer.hand[0].GetCardValue() == 7 || dealer.hand[0].GetCardValue() == 10 || dealer.hand[0] is CardAce)
                                && CheckBet(bets[Array.IndexOf(hands, hand)], limits))
                            {
                                return CHOICE_SPLIT;
                            }
                            else
                            {
                                break;
                            }
                        case 10:
                            if ((trueCount >= 6 && dealer.hand[0].GetCardCountValue() == 4)
                                ||(trueCount >= 5 && dealer.hand[0].GetCardCountValue() == 5)
                                || (trueCount >= 4 && dealer.hand[0].GetCardCountValue() == 6)) //deviation
                            {
                                return CHOICE_SPLIT;
                            }
                            else
                            {
                                break;
                            }
                        default:
                            if (CheckBet(bets[Array.IndexOf(hands, hand)], limits))
                            {
                                return CHOICE_SPLIT;
                            }
                            else
                            {
                                break;
                            }
                    }
                }

                //now check for double opportunity
                if (hand.Count == 2)
                {
                    Console.WriteLine("AI: CHECK DOUBLE");

                    if (GetHandValue(hand).Item3) //soft total
                    {
                        if ((GetHandValue(hand).Item2 == 13 || GetHandValue(hand).Item2 == 14)
                            && (dealer.hand[0].GetCardValue() == 5 || dealer.hand[0].GetCardValue() == 6)
                            && chips >= 1) //not > 0 - could be in theory 0.5 and I only allow whole-number bets
                        {
                            Console.WriteLine("AI: DOUBLE");
                            return CHOICE_DOUBLE;
                        }
                        else if ((GetHandValue(hand).Item2 == 15 || GetHandValue(hand).Item2 == 16)
                            && (dealer.hand[0].GetCardValue() == 4 || dealer.hand[0].GetCardValue() == 5 || dealer.hand[0].GetCardValue() == 6)
                            && chips >= 1)
                        {
                            Console.WriteLine("AI: DOUBLE");
                            return CHOICE_DOUBLE;
                        }
                        else if ((GetHandValue(hand).Item2 == 17 || GetHandValue(hand).Item2 == 18)
                            && (dealer.hand[0].GetCardValue() == 3 || dealer.hand[0].GetCardValue() == 4 || dealer.hand[0].GetCardValue() == 5 || dealer.hand[0].GetCardValue() == 6)
                            && chips >= 1)
                        {
                            Console.WriteLine("AI: DOUBLE");
                            return CHOICE_DOUBLE;
                        }
                        else if ((GetHandValue(hand).Item2 == 18 && dealer.HitSoft17)
                            && (dealer.hand[0].GetCardValue() == 2)
                            && chips >= 1)
                        {
                            Console.WriteLine("AI: DOUBLE");
                            return CHOICE_DOUBLE;
                        }
                        else if ((GetHandValue(hand).Item2 == 19 && dealer.HitSoft17) 
                            && (dealer.hand[0].GetCardValue() == 6)
                            && trueCount > 0 //deviation
                            && chips >= 1)
                        {
                            Console.WriteLine("AI: DOUBLE");
                            return CHOICE_DOUBLE;
                        }
                        else if ((GetHandValue(hand).Item2 == 17)//deviation
                            && (dealer.hand[0].GetCardValue() == 2)
                            && trueCount >= 1 
                            && chips >= 1)
                        {
                            Console.WriteLine("AI: DOUBLE");
                            return CHOICE_DOUBLE;
                        }
                        else if ((GetHandValue(hand).Item2 == 19)//deviation
                            && (dealer.hand[0].GetCardValue() == 5)
                            && trueCount >= 1 
                            && chips >= 1)
                        {
                            Console.WriteLine("AI: DOUBLE");
                            return CHOICE_DOUBLE;
                        }
                        else if ((GetHandValue(hand).Item2 == 19)//deviation
                            && (dealer.hand[0].GetCardValue() == 4)
                            && trueCount >= 3
                            && chips >= 1)
                        {
                            Console.WriteLine("AI: DOUBLE");
                            return CHOICE_DOUBLE;
                        }
                    }
                    else //hard total
                    {
                        if ((GetHandValue(hand).Item2 == 9)
                            && (dealer.hand[0].GetCardValue() >= 3 && dealer.hand[0].GetCardValue() <= 6)
                            && chips >= 1)
                        {
                            Console.WriteLine("AI: DOUBLE");
                            return CHOICE_DOUBLE;
                        }
                        else if ((GetHandValue(hand).Item2 == 10)
                            && (dealer.hand[0].GetCardValue() >= 2 && dealer.hand[0].GetCardValue() <= 9)
                            && chips >= 1)
                        {
                            Console.WriteLine("AI: DOUBLE");
                            return CHOICE_DOUBLE;
                        }
                        else if ((GetHandValue(hand).Item2 == 11)
                            && ((dealer.hand[0].GetCardValue() >= 2 && dealer.hand[0].GetCardValue() <= 10) || (dealer.hand[0] is CardAce && dealer.HitSoft17 && trueCount >= -1)) //trueCount part - deviation
                            && chips >= 1)
                        {
                            Console.WriteLine("AI: DOUBLE");
                            return CHOICE_DOUBLE;
                        }
                        else if ((GetHandValue(hand).Item2 == 11) //deviation
                            && (trueCount >= 1)
                            && chips >= 1)
                        {
                            Console.WriteLine("AI: DOUBLE");
                            return CHOICE_DOUBLE;
                        }
                        else if ((GetHandValue(hand).Item2 == 10) //deviation
                            && dealer.hand[0].GetCardValue() == 10
                            && trueCount >= 4
                            && chips >= 1)
                        {
                            Console.WriteLine("AI: DOUBLE");
                            return CHOICE_DOUBLE;
                        }
                        else if ((GetHandValue(hand).Item2 == 10) //deviation
                            && dealer.hand[0] is CardAce
                            && ((trueCount >= 4 && !dealer.HitSoft17) || (trueCount >= 3 && dealer.HitSoft17))
                            && chips >= 1)
                        {
                            Console.WriteLine("AI: DOUBLE");
                            return CHOICE_DOUBLE;
                        }
                        else if ((GetHandValue(hand).Item2 == 9) //deviation
                            && dealer.hand[0].GetCardValue() == 2
                            && trueCount >= 1
                            && chips >= 1)
                        {
                            Console.WriteLine("AI: DOUBLE");
                            return CHOICE_DOUBLE;
                        }
                        else if ((GetHandValue(hand).Item2 == 9) //deviation
                            && dealer.hand[0].GetCardValue() == 7
                            && trueCount >= 3
                            && chips >= 1)
                        {
                            Console.WriteLine("AI: DOUBLE");
                            return CHOICE_DOUBLE;
                        }
                        else if ((GetHandValue(hand).Item2 == 8) //deviation
                            && dealer.hand[0].GetCardValue() == 6
                            && trueCount >= 2
                            && chips >= 1)
                        {
                            Console.WriteLine("AI: DOUBLE");
                            return CHOICE_DOUBLE;
                        }
                    }
                }

                //if split and double are unavailable
                if (GetHandValue(hand).Item3) //soft total
                {
                    if (GetHandValue(hand).Item2 <= 17)
                    {
                        return CHOICE_HIT;
                    }
                    else if ((GetHandValue(hand).Item2 == 18)
                            && !(dealer.hand[0].GetCardValue() >= 2 && dealer.hand[0].GetCardValue() <= 8))
                    {
                        return CHOICE_HIT;
                    }
                    else
                    {
                        return CHOICE_STAND;
                    }
                }
                else //hard total
                {
                    if (GetHandValue(hand).Item2 <= 11)
                    {
                        return CHOICE_HIT;
                    }
                    else if ((GetHandValue(hand).Item2 == 12)
                           && !(dealer.hand[0].GetCardValue() >= 4 && dealer.hand[0].GetCardValue() <= 6))
                    {
                        if (GetHandValue(hand).Item2 == 12 && dealer.hand[0].GetCardValue() == 2 && trueCount >= 3) //deviation
                        {
                            return CHOICE_STAND;
                        }

                        if (GetHandValue(hand).Item2 == 12 && dealer.hand[0].GetCardValue() == 3 && trueCount >= 2) //deviation
                        {
                            return CHOICE_STAND;
                        }

                        return CHOICE_HIT;
                    }
                    else if ((GetHandValue(hand).Item2 >= 13 && GetHandValue(hand).Item2 <= 16)
                           && !(dealer.hand[0].GetCardValue() >= 2 && dealer.hand[0].GetCardValue() <= 6))
                    {
                        if (GetHandValue(hand).Item2 == 16 && dealer.hand[0] is CardAce && trueCount >= 3) //deviation
                        {
                            return CHOICE_STAND;
                        }

                        if (GetHandValue(hand).Item2 == 16 && dealer.hand[0].GetCardValue() == 10 && GetCurrentRunningCount(players, dealer.hand) > 0) //deviation
                        {
                            return CHOICE_STAND;
                        }

                        if (GetHandValue(hand).Item2 == 16 && dealer.hand[0].GetCardValue() == 9 && trueCount >= 4) //deviation
                        {
                            return CHOICE_STAND;
                        }

                        if (GetHandValue(hand).Item2 == 15 && dealer.hand[0].GetCardValue() == 10 && trueCount >= 4) //deviation
                        {
                            return CHOICE_STAND;
                        }

                        return CHOICE_HIT;
                    }
                    else
                    {
                        if (GetHandValue(hand).Item2 == 13 && dealer.hand[0].GetCardValue() == 2 && trueCount <= -1) //deviation
                        {
                            return CHOICE_HIT;
                        }

                        if (GetHandValue(hand).Item2 == 13 && dealer.hand[0].GetCardValue() == 3 && trueCount < -2) //deviation
                        {
                            return CHOICE_HIT;
                        }

                        if (GetHandValue(hand).Item2 == 12 && dealer.hand[0].GetCardValue() == 4 && GetCurrentRunningCount(players, dealer.hand) < 0) //deviation
                        {
                            return CHOICE_HIT;
                        }

                        if (GetHandValue(hand).Item2 == 12 && dealer.hand[0].GetCardValue() == 5 && trueCount < -1) //deviation
                        {
                            return CHOICE_HIT;
                        }

                        if (GetHandValue(hand).Item2 == 12 && dealer.hand[0].GetCardValue() == 6 
                            && ((trueCount < -1 && !dealer.HitSoft17) || (trueCount < -3 && dealer.HitSoft17))) //deviation
                        {
                            return CHOICE_HIT;
                        }

                        return CHOICE_STAND;
                    }
                }
            }
            else
            {
                return CHOICE_STAND;
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

            if (newHandIndex != -1)
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
                Console.WriteLine("AI: SPLIT");
            }
        }

        private void Double(int index, Tuple<int, int> limits)
        {
            int bet;

            if (chips >= limits.Item2)
            {
                bet = limits.Item2;
            }
            else if (chips >= limits.Item1)
            {
                bet = (int)chips;
            }
            else
            {
                bet = 0;
            }

            Bet(hands[index], bet, limits);
        }

        //splitting is valid if and only if it contains exactly 2 cards of the same VALUE (not rank!!) and the hand has not been splitted twice.
        private int IsSplittingValid(List<Card> hand)
        {
            switch (Array.IndexOf(hands, hand))
            {
                case 0:
                    if (hand.Count == 2
                        && ((hand[0].GetCardValue() == hand[1].GetCardValue()) || (hand[0].GetType() == hand[1].GetType())) //GetType for AA hands
                        && (hands[1] == null || hands[2] == null)
                        && CheckBet(bets[0], tableLimits))
                    {
                        return hands[1] == null ? 1 : 2;
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

        //does NOT change this.runningCount. Only counts cards on the table and updates this.trueCount
        public override void CountDealt(Player[] players, List<Card> dealerHand, double remainingDecks)
        {           
            UpdateTrueCount(GetCurrentRunningCount(players, dealerHand), remainingDecks);
        }

        //updates this.runningCount
        public override void UpdateRunningCount(Player[] players, List<Card> dealerHand)
        {
            foreach (Player p in players)
            {
                if (!(p == null || p.IsRuined || p.IsGone))
                {
                    foreach (List<Card> h in p.hands)
                    {
                        if (h != null)
                        {
                            foreach (Card c in h)
                            {
                                runningCount += c.GetCardCountValue();
                            }
                        }
                    }
                }
            }

            foreach (Card c in dealerHand)
            {
                if (c != null)
                {
                    runningCount += c.GetCardCountValue();
                }
            }
        }

        public override void UpdateTrueCount(int runningCount, double remainingDecks)
        {
            int wholeTrueCount = (int)(runningCount / remainingDecks);
            double halves;
            trueCount = runningCount / remainingDecks;

            if (trueCount - wholeTrueCount < -0.5)
            {
                halves = -2;
            }
            else if (trueCount - wholeTrueCount >= -0.5 && trueCount - wholeTrueCount < 0)
            {
                halves = -1;
            }
            else if (trueCount - wholeTrueCount >= 0 && trueCount - wholeTrueCount < 0.5)
            {
                halves = 0;
            }
            else 
            {
                halves = 1;
            }

            trueCount = wholeTrueCount + 0.5 * halves;
        }

        public override void UpdateTrueCount(double remainingDecks)
        {
            int wholeTrueCount = (int)(runningCount / remainingDecks);
            double halves;
            trueCount = runningCount / remainingDecks;

            if (trueCount - wholeTrueCount < -0.5)
            {
                halves = -2;
            }
            else if (trueCount - wholeTrueCount >= -0.5 && trueCount - wholeTrueCount < 0)
            {
                halves = -1;
            }
            else if (trueCount - wholeTrueCount >= 0 && trueCount - wholeTrueCount < 0.5)
            {
                halves = 0;
            }
            else
            {
                halves = 1;
            }

            trueCount = wholeTrueCount + 0.5 * halves;
        }

        public int GetCurrentRunningCount(Player[] players, List<Card> dealerHand)
        {
            int count = 0;

            foreach (Player p in players)
            {
                if (!(p == null || p.IsRuined || p.IsGone))
                {
                    foreach (List<Card> h in p.hands)
                    {
                        if (h != null)
                        {
                            foreach (Card c in h)
                            {
                                count += c.GetCardCountValue();
                            }
                        }
                    }
                }
            }

            foreach (Card c in dealerHand)
            {
                if (c != null)
                {
                    count += c.GetCardCountValue();
                }
            }

            return runningCount + count;
        }

        public override void ResetCounts()
        {
            runningCount = 0;
            trueCount = 0;
        }

        public override void Bet(List<Card> hand, Tuple<int, int> limits)
        {
            if (hand != null)
            {
                if (chips >= limits.Item1)
                {
                    int betMultiplier;
                    for (betMultiplier = Math.Min((int)trueCount*2 - 1, 12); betMultiplier > 0; betMultiplier--)
                    {
                        if (chips >= betUnit * betMultiplier && CheckBet(betUnit * betMultiplier, limits))
                        {
                            break;
                        }
                    }

                    if (betMultiplier > 0)
                    {
                        bets[Array.IndexOf(hands, hand)] += betUnit * betMultiplier;
                        chips -= betUnit * betMultiplier;
                    }
                    else
                    {
                        bets[Array.IndexOf(hands, hand)] += limits.Item1;
                        chips -= limits.Item1;
                    }
                }
                else
                {
                    IsRuined = true;
                    IsGone = true;
                }
            }            
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
            double bet;

            if (trueCount >= 3 && CheckBet(bets[0] * 0.5, new Tuple<int, int>(0, tableLimits.Item2)))
            {
                Console.WriteLine("DEBUG: Insurance");
                bet = bets[0] * 0.5;
            }
            else
            {
                Console.WriteLine("DEBUG: NoInsurance");
                bet = 0;
            }

            chips -= bet;
            insurance = bet;
        }

        public override void BetPair(Tuple<int, int> limits)
        {
            if (allowPairBets)
            {
                allowPairBets = false;
            }
        }

        public override void DisplayHands()
        {
            for (int i = 0; i < hands.Length; i++)
            {
                if (hands[i] != null)
                {
                    if (bets[i] >= 1000000000)
                    {
                        Console.WriteLine("Billions");
                    }
                    else
                    {
                        Console.WriteLine("Bet: {0}", String.Format("${0:#,#,0.##}", bets[i]));
                    }

                    Console.Write("hand {0}: ", i + 1);
                    foreach (Card c in hands[i])
                    {
                        Console.Write("{0} ", c.Name);
                    }
                    Console.WriteLine();
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
