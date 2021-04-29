using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;

namespace Blackjack
{
    class CardCountingAI:Player
    {
        private int betUnit;
        private int betSpreadMultiplier;

        bool wait;
        bool isVisible;


        public CardCountingAI(string name, List<Card> hand, BetterUI betterUI, int originalChips, Tuple<int, int> tableLimits,
            bool isSurrenderAllowed, bool isDASAllowed, bool isResplitAllowed, bool isResplitAcesAllowed, int betUnit, int betSpreadMultiplier, bool wait,
            int runningCount = 0, double trueCount = 0, bool isVisible = true) :
            base(name, hand, betterUI, originalChips, tableLimits, isSurrenderAllowed, isDASAllowed, isResplitAllowed, isResplitAcesAllowed)
        {
            this.runningCount = runningCount;
            this.trueCount = trueCount;
            this.betUnit = betUnit;
            this.betSpreadMultiplier = betSpreadMultiplier;
            this.wait = wait;
            this.isVisible = isVisible;
        }


        public override void TakeTurn(Player[] players, Dealer dealer)
        {
            string choice;
            bool isDouble = false; //after AI doubles, isDouble will be true

            //displays turn prompt
            if (isVisible)
            {
                if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                {
                    betterUI.DisplayTurn(Name);
                }
                else
                {
                    Console.WriteLine("It's {0}'s turn", Name);
                }
            }

            for (int i = 0; i < hands.Length; i++)
            {
                if (hands[i] == null)
                {
                    continue;
                }

                do
                {
                    //displays AI if if AI is visible
                    if (isVisible)
                    {
                        if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                        {
                            betterUI.DisplayPlayersStatus(players);
                        }
                        else
                        {
                            DisplayHands();
                        }

                        if (wait)
                        {
                            Thread.Sleep(1000);
                        }
                    }                   

                    if (GetHandValue(hands[i]).Item1 >= 21 || GetHandValue(hands[i]).Item2 >= 21 || hasBlackjack) //obvious choice; you always stand on 21 and you lose after going over 21.
                    {
                        choice = CHOICE_STAND;
                    }
                    else if (isDouble) //after doubling you only get 1 card.
                    {
                        Hit(dealer, i);

                        if (isVisible)
                        {
                            if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                            {
                                betterUI.DisplayPlayersStatus(players);
                                betterUI.DisplayMessage(String.Format("Last card on hand {0}", i + 1));
                            }
                            else
                            {
                                DisplayHands();
                                Console.WriteLine("Last card on hand {0}", i + 1);
                            }
                        }

                        choice = CHOICE_STAND;
                        isDouble = false;
                    }
                    else if (hands[i].Count == 1 && hands[i][0] is CardAce) //after splitting aces you only get 1 card. If its an ace and splitting is a valid choice, you can split again.
                    {
                        Hit(dealer, i);

                        if (!(hands[i][1] is CardAce))
                        {
                            if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                            {
                                if (isVisible)
                                {
                                    betterUI.DisplayPlayersStatus(players);
                                    betterUI.DisplayMessage(String.Format("Last card on hand {0}", i + 1));
                                }
                                choice = CHOICE_STAND;
                            }
                            else
                            {
                                if (isVisible)
                                {
                                    DisplayHands();
                                    Console.WriteLine("Last card on hand {0}", i + 1);
                                }
                                choice = CHOICE_STAND;
                            }
                        }
                        else
                        {
                            if (IsSplittingValid(hands[i]) != -1 && isResplitAcesAllowed)
                            {
                                choice = CHOICE_SPLIT;
                            }
                            else
                            {
                                if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                                {
                                    if (isVisible)
                                    {
                                        betterUI.DisplayPlayersStatus(players);
                                        betterUI.DisplayMessage(String.Format("Last card on hand {0}", i + 1));
                                    }
                                }
                                else
                                {
                                    if (isVisible)
                                    {
                                        DisplayHands();
                                        Console.WriteLine("Last card on hand {0}", i + 1);
                                    }
                                }
                                choice = CHOICE_STAND;
                            }
                        }
                    }
                    else if (hands[i].Count == 1) //automatic choice, you are always dealt a card on BOTH hands after splitting.
                    {
                        choice = CHOICE_HIT;
                    }
                    else //ai makes its choice
                    {
                        if (isVisible)
                        {
                            if (Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH)
                            {
                                Console.WriteLine(GetHandValue(hands[i]).Item1 != GetHandValue(hands[i]).Item2 ?
                               String.Format("hand {0}: {1} or {2}", i + 1, GetHandValue(hands[i]).Item1, GetHandValue(hands[i]).Item2)
                               : String.Format("hand {0}: {1}", i + 1, GetHandValue(hands[i]).Item1));
                            }
                        }

                        CountDealt(players, dealer.hand, dealer.DeckAmount - dealer.GetDecksInDiscard());
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
                                choice = CHOICE_HIT;
                                Hit(dealer, i);
                            }
                            break;
                        case CHOICE_STAND:
                            Stand();
                            break;
                        case CHOICE_SURRENDER:
                            SetSurrender();
                            break;
                        default:
                            break;
                    }
                } while (!(choice == CHOICE_STAND || choice == CHOICE_SURRENDER));
            }
        }


        //gets the correct choice
        //Basic strategy data: https://wizardofodds.com/games/blackjack/strategy/4-decks/
        //Deviations data: https://www.reddit.com/r/blackjack/comments/5fgf1a/deviations/, https://quizlet.com/18561678/blackjack-h17-deviations-flash-cards/, https://www.888casino.com/blog/advanced-card-counting-blackjack-strategy-deviations, 
        //best deviations data https://digitalcommons.usu.edu/cgi/viewcontent.cgi?article=1528&context=gradreports, https://www.blackjacktheforum.com/showthread.php?17600-H17-Deviations-Correct-Expert-OpinionS, https://wizardofodds.com/games/blackjack/card-counting/high-low/
        public string GetChoice(List<Card> hand, Tuple<int, int> limits, Player[] players, Dealer dealer)
        {
            if (dealer.HitSoft17) //Hit 17 decision-making
            {
                if (hand != null && dealer.hand[0] != null)
                {
                    if (GetHandValue(hand).Item3) //soft total
                    {
                        //check for split. in soft total, only possible pair is AA, which you always split
                        if (hand.Count == 2 && IsSplittingValid(hand) != -1 && CheckBet(bets[Array.IndexOf(hands, hand)], limits))
                        {
                            return CHOICE_SPLIT;
                        }

                        //double
                        if (GetHandValue(hand).Item2 == 12
                            && dealer.hand[0].GetCardValue() == 6
                            && chips >= 1)//not > 0 - could be in theory 0.5 and I only allow whole-number bets
                        {
                            return CHOICE_DOUBLE;
                        }
                        else if ((GetHandValue(hand).Item2 == 13 || GetHandValue(hand).Item2 == 14)
                                && (dealer.hand[0].GetCardValue() == 5 || dealer.hand[0].GetCardValue() == 6)
                                && chips >= 1) 
                        {
                            if ((Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH) && isVisible)
                            {
                                Console.WriteLine(Name + ": double");
                            }
                            return CHOICE_DOUBLE;
                        }
                        else if ((GetHandValue(hand).Item2 == 15 || GetHandValue(hand).Item2 == 16)
                            && (dealer.hand[0].GetCardValue() == 4 || dealer.hand[0].GetCardValue() == 5 || dealer.hand[0].GetCardValue() == 6)
                            && chips >= 1)
                        {
                            if ((Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH) && isVisible)
                            {
                                Console.WriteLine(Name + ": double");
                            }
                            return CHOICE_DOUBLE;
                        }
                        else if ((GetHandValue(hand).Item2 == 17 || GetHandValue(hand).Item2 == 18)
                            && (dealer.hand[0].GetCardValue() == 3 || dealer.hand[0].GetCardValue() == 4 || dealer.hand[0].GetCardValue() == 5 || dealer.hand[0].GetCardValue() == 6)
                            && chips >= 1)
                        {
                            if ((Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH) && isVisible)
                            {
                                Console.WriteLine(Name + ": double");
                            }
                            return CHOICE_DOUBLE;
                        }
                        else if ((GetHandValue(hand).Item2 == 18)
                            && (dealer.hand[0].GetCardValue() == 2)
                            && chips >= 1)
                        {
                            if ((Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH) && isVisible)
                            {
                                Console.WriteLine(Name + ": double");
                            }
                            return CHOICE_DOUBLE;
                        }
                        else if ((GetHandValue(hand).Item2 == 19)
                            && (dealer.hand[0].GetCardValue() == 6)
                            && trueCount >= 0 //deviation
                            && chips >= 1)
                        {
                            if ((Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH) && isVisible)
                            {
                                Console.WriteLine(Name + ": double");
                            }
                            return CHOICE_DOUBLE;
                        }
                        else if ((GetHandValue(hand).Item2 == 17)//deviation
                            && (dealer.hand[0].GetCardValue() == 2)
                            && trueCount >= 1
                            && chips >= 1)
                        {
                            if ((Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH) && isVisible)
                            {
                                Console.WriteLine(Name + ": double");
                            }
                            return CHOICE_DOUBLE;
                        }
                        else if ((GetHandValue(hand).Item2 == 19)//deviation
                            && (dealer.hand[0].GetCardValue() == 5)
                            && trueCount >= 1
                            && chips >= 1)
                        {
                            if ((Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH) && isVisible)
                            {
                                Console.WriteLine(Name + ": double");
                            }
                            return CHOICE_DOUBLE;
                        }
                        else if ((GetHandValue(hand).Item2 == 19)//deviation
                            && (dealer.hand[0].GetCardValue() == 4)
                            && trueCount >= 3
                            && chips >= 1)
                        {
                            if (Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH && isVisible)
                            {
                                Console.WriteLine(Name + ": double");
                            }
                            return CHOICE_DOUBLE;
                        }

                        //soft total - other
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
                        //check surrender first
                        if (isSurrenderAllowed && hand.Count == 2 && Array.IndexOf(hands, hand) == 0
                            && hands[1] == null && hands[2] == null && hands[3] == null) //in order to surrender: surrender must be allowed and it MUST be your first choice - before doubling down or splitting
                        {
                            if (GetHandValue(hand).Item2 == 17 && dealer.hand[0] is CardAce)
                            {
                                return CHOICE_SURRENDER;
                            }

                            if (GetHandValue(hand).Item2 == 16 &&
                                ((dealer.hand[0] is CardAce || dealer.hand[0].GetCardValue() == 10)
                                || (dealer.hand[0].GetCardValue() == 9 && trueCount >= -1)
                                || (dealer.hand[0].GetCardValue() == 8 && trueCount >= 4)))
                            {
                                if (hand[0].GetCardValue() == hand[1].GetCardValue() && IsSplittingValid(hand) != -1 && !(dealer.hand[0] is CardAce)) //pair of 8s
                                {
                                    return CHOICE_SPLIT;
                                }
                                else
                                {
                                    return CHOICE_SURRENDER;
                                }
                            }

                            if (GetHandValue(hand).Item2 == 15 &&
                                ((dealer.hand[0] is CardAce && trueCount >= -1)//deviation
                                || (dealer.hand[0].GetCardValue() == 10 && trueCount >= 0) //deviation
                                || (dealer.hand[0].GetCardValue() == 9 && trueCount >= 2))) //deviation
                            {
                                return CHOICE_SURRENDER;
                            }
                        }

                        //check for split opportunity
                        if (IsSplittingValid(hand) != -1)
                        {
                            switch (hand[0].GetCardValue())
                            {
                                case 2:
                                case 3:
                                    if ((dealer.hand[0].GetCardValue() >= 4 && dealer.hand[0].GetCardValue() <= 7)
                                        && CheckBet(bets[Array.IndexOf(hands, hand)], limits))
                                    {
                                        return CHOICE_SPLIT;
                                    }
                                    else if ((dealer.hand[0].GetCardValue() >= 2 && dealer.hand[0].GetCardValue() <= 3)
                                        && CheckBet(bets[Array.IndexOf(hands, hand)], limits)
                                        && isDASAllowed)
                                    {
                                        return CHOICE_SPLIT;
                                    }
                                    else
                                    {
                                        break;
                                    }
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
                                        && CheckBet(bets[Array.IndexOf(hands, hand)], limits)
                                        && isDASAllowed)
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
                                    if ((trueCount >= 6 && dealer.hand[0].GetCardCountValue() == 4) //deviation
                                        || (trueCount >= 5 && dealer.hand[0].GetCardCountValue() == 5) //deviation
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

                        //check for double opportunity
                        if (hand.Count == 2 && ((hands[1] == null && hands[2] == null && hands[3] == null) || isDASAllowed))
                        {
                            if ((GetHandValue(hand).Item2 == 9)
                                && (dealer.hand[0].GetCardValue() >= 3 && dealer.hand[0].GetCardValue() <= 6)
                                && chips >= 1)
                            {
                                if ((Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH) && isVisible)
                                {
                                    Console.WriteLine(Name + ": double");
                                }
                                return CHOICE_DOUBLE;
                            }
                            else if ((GetHandValue(hand).Item2 == 10)
                                && (dealer.hand[0].GetCardValue() >= 2 && dealer.hand[0].GetCardValue() <= 9)
                                && chips >= 1)
                            {
                                if ((Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH) && isVisible)
                                {
                                    Console.WriteLine(Name + ": double");
                                }
                                return CHOICE_DOUBLE;
                            }
                            else if ((GetHandValue(hand).Item2 == 11)
                                && chips >= 1)
                            {
                                if ((Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH) && isVisible)
                                {
                                    Console.WriteLine(Name + ": double");
                                }
                                return CHOICE_DOUBLE;
                            }
                            else if ((GetHandValue(hand).Item2 == 10) //deviation
                                && dealer.hand[0].GetCardValue() == 10
                                && trueCount >= 4
                                && chips >= 1)
                            {
                                if ((Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH) && isVisible)
                                {
                                    Console.WriteLine(Name + ": double");
                                }
                                return CHOICE_DOUBLE;
                            }
                            else if ((GetHandValue(hand).Item2 == 10) //deviation
                                && dealer.hand[0] is CardAce
                                && trueCount >= 3
                                && chips >= 1)
                            {
                                if ((Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH) && isVisible)
                                {
                                    Console.WriteLine(Name + ": double");
                                }
                                return CHOICE_DOUBLE;
                            }
                            else if ((GetHandValue(hand).Item2 == 9) //deviation
                                && dealer.hand[0].GetCardValue() == 2
                                && trueCount >= 1
                                && chips >= 1)
                            {
                                if ((Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH) && isVisible)
                                {
                                    Console.WriteLine(Name + ": double");
                                }
                                return CHOICE_DOUBLE;
                            }
                            else if ((GetHandValue(hand).Item2 == 9) //deviation
                                && dealer.hand[0].GetCardValue() == 7
                                && trueCount >= 3
                                && chips >= 1)
                            {
                                if ((Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH) && isVisible)
                                {
                                    Console.WriteLine(Name + ": double");
                                }
                                return CHOICE_DOUBLE;
                            }
                            else if ((GetHandValue(hand).Item2 == 8) //deviation
                                && dealer.hand[0].GetCardValue() == 6
                                && trueCount >= 2
                                && chips >= 1)
                            {
                                if ((Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH) && isVisible)
                                {
                                    Console.WriteLine(Name + ": double");
                                }
                                return CHOICE_DOUBLE;
                            }
                        }

                        //if split and double are unavailable
                        if (GetHandValue(hand).Item2 <= 11)
                        {
                            return CHOICE_HIT;
                        }
                        else if (GetHandValue(hand).Item2 == 12)
                        {
                            if (GetHandValue(hand).Item2 == 12 && dealer.hand[0].GetCardValue() == 2 && trueCount >= 3) //deviation
                            {
                                return CHOICE_STAND;
                            }

                            if (GetHandValue(hand).Item2 == 12 && dealer.hand[0].GetCardValue() == 3 && trueCount >= 2) //deviation
                            {
                                return CHOICE_STAND;
                            }

                            if (GetHandValue(hand).Item2 == 12 && dealer.hand[0].GetCardValue() == 4 && trueCount >= 0) //deviation
                            {
                                return CHOICE_STAND;
                            }

                            if (GetHandValue(hand).Item2 == 12 && dealer.hand[0].GetCardValue() == 5 && trueCount >= -2) //deviation
                            {
                                return CHOICE_STAND;
                            }

                            if (GetHandValue(hand).Item2 == 12 && dealer.hand[0].GetCardValue() == 6 && trueCount >= -1) //deviation
                            {
                                return CHOICE_STAND;
                            }

                            return CHOICE_HIT;
                        }
                        else if (GetHandValue(hand).Item2 == 13 && !(dealer.hand[0].GetCardValue() >= 4 && dealer.hand[0].GetCardValue() <= 6))
                        {
                            if (GetHandValue(hand).Item2 == 13 && dealer.hand[0].GetCardValue() == 2 && trueCount >= -1)
                            {
                                return CHOICE_STAND;
                            }

                            if (GetHandValue(hand).Item2 == 13 && dealer.hand[0].GetCardValue() == 3 && trueCount >= -2)
                            {
                                return CHOICE_STAND;
                            }

                            return CHOICE_HIT;
                        }
                        else if ((GetHandValue(hand).Item2 >= 14 && GetHandValue(hand).Item2 <= 16)
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

                            if (GetHandValue(hand).Item2 == 16 && dealer.hand[0].GetCardValue() == 9 && trueCount >= 5) //deviation
                            {
                                return CHOICE_STAND;
                            }

                            if (GetHandValue(hand).Item2 == 15 && dealer.hand[0].GetCardValue() == 10 && trueCount >= 4) //deviation
                            {
                                return CHOICE_STAND;
                            }

                            if (GetHandValue(hand).Item2 == 15 && dealer.hand[0] is CardAce && trueCount >= 5) //deviation
                            {
                                return CHOICE_STAND;
                            }

                            return CHOICE_HIT;
                        }
                        else
                        {
                            return CHOICE_STAND;
                        }
                    }                    
                }
                else
                {
                    return CHOICE_STAND;
                }
            }
            else //Stand 17 decision making
            {
                if (hand != null && dealer.hand[0] != null)
                {
                    if (GetHandValue(hand).Item3) //soft total
                    {
                        //check for split. in soft total, only possible pair is AA, which you always split
                        if (hand.Count == 2 && IsSplittingValid(hand) != -1 && CheckBet(bets[Array.IndexOf(hands, hand)], limits))
                        {
                            return CHOICE_SPLIT;
                        }

                        //double
                        if (GetHandValue(hand).Item2 == 12
                            && dealer.hand[0].GetCardValue() == 6
                            && chips >= 1)//not > 0 - could be in theory 0.5 and I only allow whole-number bets
                        {
                            return CHOICE_DOUBLE;
                        }
                        else if ((GetHandValue(hand).Item2 == 13 || GetHandValue(hand).Item2 == 14)
                                && (dealer.hand[0].GetCardValue() == 5 || dealer.hand[0].GetCardValue() == 6)
                                && chips >= 1)
                        {
                            if ((Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH) && isVisible)
                            {
                                Console.WriteLine(Name + ": double");
                            }
                            return CHOICE_DOUBLE;
                        }
                        else if ((GetHandValue(hand).Item2 == 15 || GetHandValue(hand).Item2 == 16)
                            && (dealer.hand[0].GetCardValue() == 4 || dealer.hand[0].GetCardValue() == 5 || dealer.hand[0].GetCardValue() == 6)
                            && chips >= 1)
                        {
                            if ((Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH) && isVisible)
                            {
                                Console.WriteLine(Name + ": double");
                            }
                            return CHOICE_DOUBLE;
                        }
                        else if ((GetHandValue(hand).Item2 == 17 || GetHandValue(hand).Item2 == 18)
                            && (dealer.hand[0].GetCardValue() == 3 || dealer.hand[0].GetCardValue() == 4 || dealer.hand[0].GetCardValue() == 5 || dealer.hand[0].GetCardValue() == 6)
                            && chips >= 1)
                        {
                            if ((Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH) && isVisible)
                            {
                                Console.WriteLine(Name + ": double");
                            }
                            return CHOICE_DOUBLE;
                        }
                        else if ((GetHandValue(hand).Item2 == 18)
                            && (dealer.hand[0].GetCardValue() == 2)
                            && chips >= 1)
                        {
                            if ((Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH) && isVisible)
                            {
                                Console.WriteLine(Name + ": double");
                            }
                            return CHOICE_DOUBLE;
                        }
                        else if ((GetHandValue(hand).Item2 == 19)
                            && (dealer.hand[0].GetCardValue() == 6)
                            && trueCount >= 1 //deviation
                            && chips >= 1)
                        {
                            if ((Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH) && isVisible)
                            {
                                Console.WriteLine(Name + ": double");
                            }
                            return CHOICE_DOUBLE;
                        }
                        else if ((GetHandValue(hand).Item2 == 17)//deviation
                            && (dealer.hand[0].GetCardValue() == 2)
                            && trueCount >= 1
                            && chips >= 1)
                        {
                            if ((Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH) && isVisible)
                            {
                                Console.WriteLine(Name + ": double");
                            }
                            return CHOICE_DOUBLE;
                        }
                        else if ((GetHandValue(hand).Item2 == 19)//deviation
                            && (dealer.hand[0].GetCardValue() == 5)
                            && trueCount >= 1
                            && chips >= 1)
                        {
                            if ((Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH) && isVisible)
                            {
                                Console.WriteLine(Name + ": double");
                            }
                            return CHOICE_DOUBLE;
                        }
                        else if ((GetHandValue(hand).Item2 == 19)//deviation
                            && (dealer.hand[0].GetCardValue() == 4)
                            && trueCount >= 3
                            && chips >= 1)
                        {
                            if ((Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH) && isVisible)
                            {
                                Console.WriteLine(Name + ": double");
                            }
                            return CHOICE_DOUBLE;
                        }

                        //soft total - other
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
                        //check surrender first
                        if (isSurrenderAllowed && hand.Count == 2 && Array.IndexOf(hands, hand) == 0
                            && hands[1] == null && hands[2] == null && hands[3] == null) //in order to surrender: surrender must be allowed and it MUST be your first move - before doubling down or splitting
                        {
                            if (GetHandValue(hand).Item2 == 16 &&
                                ((dealer.hand[0] is CardAce || dealer.hand[0].GetCardValue() == 10)
                                || (dealer.hand[0].GetCardValue() == 9 && trueCount >= -1)
                                || (dealer.hand[0].GetCardValue() == 8 && trueCount >= 4)))
                            {
                                if (hand[0].GetCardValue() == hand[1].GetCardValue() && IsSplittingValid(hand) != -1) //pair of 8s
                                {
                                    return CHOICE_SPLIT;
                                }
                                else
                                {
                                    return CHOICE_SURRENDER;
                                }
                            }

                            if (GetHandValue(hand).Item2 == 15 &&
                                ((dealer.hand[0] is CardAce && trueCount >= 2)
                                || (dealer.hand[0].GetCardValue() == 10 && trueCount >= 0) //deviation
                                || (dealer.hand[0].GetCardValue() == 9 && trueCount >= 2))) //deviation
                            {
                                return CHOICE_SURRENDER;
                            }
                        }

                        //check for split opportunity
                        if (IsSplittingValid(hand) != -1)
                        {
                            switch (hand[0].GetCardValue())
                            {
                                case 2:
                                case 3:
                                    if ((dealer.hand[0].GetCardValue() >= 4 && dealer.hand[0].GetCardValue() <= 7)
                                        && CheckBet(bets[Array.IndexOf(hands, hand)], limits))
                                    {
                                        return CHOICE_SPLIT;
                                    }
                                    else if ((dealer.hand[0].GetCardValue() >= 2 && dealer.hand[0].GetCardValue() <= 3)
                                        && CheckBet(bets[Array.IndexOf(hands, hand)], limits)
                                        && isDASAllowed)
                                    {
                                        return CHOICE_SPLIT;
                                    }
                                    else
                                    {
                                        break;
                                    }
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
                                        && CheckBet(bets[Array.IndexOf(hands, hand)], limits)
                                        && isDASAllowed)
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
                                    if (dealer.hand[0].GetCardValue() >= 3 && dealer.hand[0].GetCardValue() <= 6
                                        && CheckBet(bets[Array.IndexOf(hands, hand)], limits))
                                    {
                                        return CHOICE_SPLIT;
                                    }
                                    else if (dealer.hand[0].GetCardValue() == 2 && isDASAllowed
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
                                        || (trueCount >= 5 && dealer.hand[0].GetCardCountValue() == 5)
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

                        //check for double opportunity
                        if (hand.Count == 2 && ((hands[1] == null && hands[2] == null && hands[3] == null) || isDASAllowed))
                        {
                            if ((GetHandValue(hand).Item2 == 9)
                                && (dealer.hand[0].GetCardValue() >= 3 && dealer.hand[0].GetCardValue() <= 6)
                                && chips >= 1)
                            {
                                if ((Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH) && isVisible)
                                {
                                    Console.WriteLine(Name + ": double");
                                }
                                return CHOICE_DOUBLE;
                            }
                            else if ((GetHandValue(hand).Item2 == 10)
                                && (dealer.hand[0].GetCardValue() >= 2 && dealer.hand[0].GetCardValue() <= 9)
                                && chips >= 1)
                            {
                                if ((Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH) && isVisible)
                                {
                                    Console.WriteLine(Name + ": double");
                                }
                                return CHOICE_DOUBLE;
                            }
                            else if ((GetHandValue(hand).Item2 == 11)
                                && !(dealer.hand[0] is CardAce)
                                && chips >= 1)
                            {
                                if ((Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH) && isVisible)
                                {
                                    Console.WriteLine(Name + ": double");
                                }
                                return CHOICE_DOUBLE;
                            }
                            else if ((GetHandValue(hand).Item2 == 11)
                                && (dealer.hand[0] is CardAce)
                                && trueCount >= 1
                                && chips >= 1)
                            {
                                
                            }
                            else if ((GetHandValue(hand).Item2 == 10) //deviation
                                && dealer.hand[0].GetCardValue() == 10
                                && trueCount >= 4
                                && chips >= 1)
                            {
                                if ((Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH) && isVisible)
                                {
                                    Console.WriteLine(Name + ": double");
                                }
                                return CHOICE_DOUBLE;
                            }
                            else if ((GetHandValue(hand).Item2 == 10) //deviation
                                && dealer.hand[0] is CardAce
                                && trueCount >= 3
                                && chips >= 1)
                            {
                                if ((Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH) && isVisible)
                                {
                                    Console.WriteLine(Name + ": double");
                                }
                                return CHOICE_DOUBLE;
                            }
                            else if ((GetHandValue(hand).Item2 == 9) //deviation
                                && dealer.hand[0].GetCardValue() == 2
                                && trueCount >= 1
                                && chips >= 1)
                            {
                                if ((Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH) && isVisible)
                                {
                                    Console.WriteLine(Name + ": double");
                                }
                                return CHOICE_DOUBLE;
                            }
                            else if ((GetHandValue(hand).Item2 == 9) //deviation
                                && dealer.hand[0].GetCardValue() == 7
                                && trueCount >= 3
                                && chips >= 1)
                            {
                                if ((Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH) && isVisible)
                                {
                                    Console.WriteLine(Name + ": double");
                                }
                                return CHOICE_DOUBLE;
                            }
                            else if ((GetHandValue(hand).Item2 == 8) //deviation
                                && dealer.hand[0].GetCardValue() == 6
                                && trueCount >= 2
                                && chips >= 1)
                            {
                                if ((Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH) && isVisible)
                                {
                                    Console.WriteLine(Name + ": double");
                                }
                                return CHOICE_DOUBLE;
                            }
                        }

                        //if split and double are unavailable
                        if (GetHandValue(hand).Item2 <= 11)
                        {
                            return CHOICE_HIT;
                        }
                        else if ((GetHandValue(hand).Item2 == 12)
                               && !(dealer.hand[0].GetCardValue() == 5 || dealer.hand[0].GetCardValue() == 6))
                        {
                            if (GetHandValue(hand).Item2 == 12 && dealer.hand[0].GetCardValue() == 2 && trueCount >= 3) //deviation
                            {
                                return CHOICE_STAND;
                            }

                            if (GetHandValue(hand).Item2 == 12 && dealer.hand[0].GetCardValue() == 3 && trueCount >= 2) //deviation
                            {
                                return CHOICE_STAND;
                            }

                            if (GetHandValue(hand).Item2 == 12 && dealer.hand[0].GetCardValue() == 4 && trueCount >= 0) //deviation
                            {
                                return CHOICE_STAND;
                            }

                            return CHOICE_HIT;
                        }
                        else if (GetHandValue(hand).Item2 == 13 && !(dealer.hand[0].GetCardValue() >= 3 && dealer.hand[0].GetCardValue() <= 6))
                        {
                            if (GetHandValue(hand).Item2 == 13 && dealer.hand[0].GetCardValue() == 2 && trueCount >= -1)
                            {
                                return CHOICE_STAND;
                            }

                            return CHOICE_HIT;
                        }
                        else if ((GetHandValue(hand).Item2 >= 14 && GetHandValue(hand).Item2 <= 16)
                               && !(dealer.hand[0].GetCardValue() >= 2 && dealer.hand[0].GetCardValue() <= 6))
                        {
                            if (GetHandValue(hand).Item2 == 16 && dealer.hand[0].GetCardValue() == 10 && GetCurrentRunningCount(players, dealer.hand) > 0) //deviation
                            {
                                return CHOICE_STAND;
                            }

                            if (GetHandValue(hand).Item2 == 16 && dealer.hand[0].GetCardValue() == 9 && trueCount >= 4) //deviation
                            {
                                return CHOICE_STAND;
                            }

                            if (GetHandValue(hand).Item2 == 15 && dealer.hand[0].GetCardValue() == 10 && trueCount >= 3) //deviation
                            {
                                return CHOICE_STAND;
                            }

                            return CHOICE_HIT;
                        }
                        else
                        {
                            return CHOICE_STAND;
                        }
                    }
                }
                else
                {
                    return CHOICE_STAND;
                }
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
                if ((Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH) && isVisible)
                {
                    Console.WriteLine(Name + ": split");
                }
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


        //splitting is valid if and only if a hand contains exactly 2 cards of the same VALUE (not rank!!) and the hand has not been splitted twice.
        //returns index of the new hand if splitting is valid. Otherwise returns -1
        private int IsSplittingValid(List<Card> hand)
        {
            switch (Array.IndexOf(hands, hand))
            {
                case 0:
                    if (hand.Count == 2
                        && ((hand[0].GetCardValue() == hand[1].GetCardValue()) || (hand[0].GetType() == hand[1].GetType())) //GetType for AA hands
                        && (hands[1] == null || hands[2] == null)
                        && CheckBet(bets[0], tableLimits)
                        && (hands[1] == null || isResplitAllowed))
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
                        && CheckBet(bets[1], tableLimits)
                        && isResplitAllowed)
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


        public void SetSurrender()
        {
            surrender = true;
        }


        //CARD COUNTING METHODS
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


        //counts cards on the table and adds the count to the runningCount
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


        //BETTING METHODS
        //BETTING SYSTEM JE UPRAVENOU VERZI SYSTEMU DOSTUPNEHO NA TOMTO ODKAZU: https://www.blackjack-trainer.net/blackjack-betting-strategy/
        public override void Bet(List<Card> hand, Tuple<int, int> limits)
        {
            if (hand != null)
            {
                if (chips >= limits.Item1)
                {
                    int betMultiplier;
                    for (betMultiplier = Math.Min((int)(trueCount - 1)*betSpreadMultiplier, 6*betSpreadMultiplier); betMultiplier > 0; betMultiplier--) //https://www.blackjack-trainer.net/blackjack-betting-strategy/
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
            if (isVisible)
            {
                if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                {
                    betterUI.DisplayTurn(Name);
                }
                else
                {
                    Console.WriteLine("It's {0}'s turn.", Name);
                }
            }

            double bet;

            if (trueCount >= 3 && CheckBet(bets[0] * 0.5, new Tuple<int, int>(0, tableLimits.Item2)))
            {
                if (isVisible)
                {
                    if (Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH)
                    {
                        Console.WriteLine(Name + ": insurance");
                    }
                }

                bet = bets[0] * 0.5;
            }
            else
            {
                if (isVisible)
                {
                    if (Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH)
                    {
                        Console.WriteLine(Name + ": no insurance");
                    }
                }

                bet = 0;
            }

            chips -= bet;
            insurance = bet;
        }


        //betting on pairs isn't worth it so AI turns that option off if it's turned on.
        public override void BetPair(Tuple<int, int> limits)
        {
            if (allowPairBets)
            {
                allowPairBets = false;
            }
        }


        //"HAND" METHODS
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
                        if (c!=null)
                        {
                            Console.Write("{0} ", c.Name);
                        }
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
            surrender = false;
        }
    }
}
