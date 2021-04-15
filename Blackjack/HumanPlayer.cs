using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;

namespace Blackjack
{
    class HumanPlayer:Player
    {
        private bool practice;

        public HumanPlayer(string name, List<Card> hand, BetterUI betterUI, int originalChips, Tuple<int, int> tableLimits,
            bool isSurrenderAllowed, bool isDASAllowed, bool isResplitAllowed, bool isResplitAcesAllowed, bool practice, int runningCount = 0, int trueCount = 0) :
            base(name, hand, betterUI, originalChips, tableLimits, isSurrenderAllowed, isDASAllowed, isResplitAllowed, isResplitAcesAllowed)
        {
            this.practice = practice;
            this.runningCount = runningCount;
            this.trueCount = trueCount;
        }        

        public override void TakeTurn(Player[] players, Dealer dealer)
        {
            string choice;
            bool isDouble = false;

            if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
            {
                betterUI.DisplayTurn(Name);
            }
            else
            {
                Console.WriteLine("It's {0}'s turn", Name);
            }

            for (int i = 0; i<hands.Length; i++)
            {
                if (hands[i]==null)
                {
                    continue;
                }

                do
                {
                    if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                    {
                        betterUI.DisplayPlayersStatus(players);
                    }
                    else
                    {
                        DisplayHands();
                    }

                    if (GetHandValue(hands[i]).Item1 >= 21 || GetHandValue(hands[i]).Item2 >= 21 || hasBlackjack) //obvious choice; you always stand on 21 and you lose after going over 21.
                    {
                        choice = CHOICE_STAND;
                    }
                    else if (isDouble) //after doubling you only get 1 card. The same rule apply if you split AA.
                    {
                        Hit(dealer, i);

                        if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                        {
                            betterUI.DisplayPlayersStatus(players);
                            betterUI.DisplayMessage(String.Format("Last card on hand {0}", i));
                        }
                        else
                        {
                            DisplayHands();
                            Console.WriteLine("Last card on hand {0}", i);
                        }

                        choice = CHOICE_STAND;
                        isDouble = false;
                    }
                    else if (hands[i].Count == 1 && hands[i][0] is CardAce)
                    {
                        Hit(dealer, i);

                        if (IsSplittingValid(hands[i])!=-1 && isResplitAcesAllowed)
                        {
                            choice = betterUI.GetStringChoice("Do you want to split again?", new string[] { CHOICE_STAND, CHOICE_SPLIT});
                        }
                        else
                        {
                            choice = CHOICE_STAND;
                        }
                    }
                    else if (hands[i].Count == 1) //automatic choice, you are always dealt a card do BOTH hands after splitting.
                    {
                        choice = CHOICE_HIT;
                    }
                    else
                    {
                        if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                        {
                            if (isSurrenderAllowed)
                            {
                                choice = betterUI.GetStringChoice(GetHandValue(hands[i]).Item1 != GetHandValue(hands[i]).Item2 ?
                                    String.Format("hand {0}: {1} or {2}", i + 1, GetHandValue(hands[i]).Item1, GetHandValue(hands[i]).Item2)
                                    : String.Format("hand {0}: {1}", i + 1, GetHandValue(hands[i]).Item1),
                                    new string[] { CHOICE_HIT, CHOICE_STAND, CHOICE_SPLIT, CHOICE_DOUBLE, CHOICE_SURRENDER });
                            }
                            else
                            {
                                choice = betterUI.GetStringChoice(GetHandValue(hands[i]).Item1 != GetHandValue(hands[i]).Item2 ?
                                    String.Format("hand {0}: {1} or {2}", i + 1, GetHandValue(hands[i]).Item1, GetHandValue(hands[i]).Item2)
                                    : String.Format("hand {0}: {1}", i + 1, GetHandValue(hands[i]).Item1),
                                    new string[] { CHOICE_HIT, CHOICE_STAND, CHOICE_SPLIT, CHOICE_DOUBLE });
                            }
                        }
                        else
                        {
                            Console.WriteLine(GetHandValue(hands[i]).Item1 != GetHandValue(hands[i]).Item2 ?
                           String.Format("hand {0}: {1} or {2}", i + 1, GetHandValue(hands[i]).Item1, GetHandValue(hands[i]).Item2)
                           : String.Format("hand {0}: {1}", i + 1, GetHandValue(hands[i]).Item1));
                            choice = GetChoice();
                        }

                        if (practice)
                        {
                            CountDealt(players, dealer.hand, dealer.DeckAmount - dealer.GetDecksInDiscard());
                            string correctChoice = GetCorrectChoice(hands[i], tableLimits, players, dealer);
                            if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                            {
                                Console.SetCursorPosition(0,2);
                                Console.Write(new String(' ', 50));

                                Console.SetCursorPosition(0,2);
                                if (choice == correctChoice)
                                {
                                    Console.Write("Your choice is correct");
                                }
                                else
                                {
                                    Console.Write("Correct choice was {0}, (RC was {1}, TC was {2})",
                                        correctChoice, GetCurrentRunningCount(players, dealer.hand), trueCount);
                                }
                            }
                            else
                            {
                                if (choice == correctChoice)
                                {
                                    Console.WriteLine("Your choice is correct");
                                }
                                else
                                {
                                    Console.WriteLine("Correct choice was {0}, (RC was {1}, TC was {2})",
                                        correctChoice, GetCurrentRunningCount(players, dealer.hand), trueCount);
                                }
                            }
                        }
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
                            if (hands[i].Count == 2 && chips >= 0 && (isDASAllowed || (hands[1] == null && hands[2] == null && hands[3] == null)))
                            {
                                Double(i, new Tuple<int, int>(0, bets[i]));
                                isDouble = true;
                            }
                            else
                            {
                                if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                                {
                                    betterUI.DisplayMessage("Cannot double");
                                }
                                else
                                {
                                    Console.WriteLine("Cannot double");
                                }
                            }
                            break;
                        case CHOICE_STAND:
                            Stand();
                            break;
                        case CHOICE_SURRENDER:
                            if (i == 0 && hands[i].Count == 2 && hands[1] == null && hands[2] == null && hands[3] == null)
                            {
                                SetSurrender();
                            }
                            else
                            {
                                if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                                {
                                    betterUI.DisplayMessage("Cannot surrender");
                                }
                                else
                                {
                                    Console.WriteLine("Cannot surrender");
                                }

                                choice = "";
                            }
                            break;
                        default:
                            break;
                    }
                } while (!(choice == CHOICE_STAND || choice == CHOICE_SURRENDER));

                if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                {
                    betterUI.ClearMessages();
                }
            }
        }

        public string GetChoice()
        {
            if (isSurrenderAllowed)
            {
                Console.WriteLine("Enter (h)it, (s)tand, (sp)lit, (d)ouble or (su)rrender");
            }
            else
            {
                Console.WriteLine("Enter (h)it, (s)tand, (sp)lit, or (d)ouble");
            }

            switch (Console.ReadLine().ToLower())
            {
                case "h":
                case "hit":
                    return CHOICE_HIT;
                case "s":
                case "stand":
                    return CHOICE_STAND;
                case "sp":
                case "split":
                    return CHOICE_SPLIT;
                case "d":
                case "double":
                    return CHOICE_DOUBLE;
                case "su":
                case "surrender":
                    if (isSurrenderAllowed)
                    {
                        return CHOICE_SURRENDER;
                    }
                    else
                    {
                        Console.WriteLine("Invalid option! Enter (h)it, (s)tand, (sp)lit or (d)ouble");
                        return "";
                    }
                default:
                    if (isSurrenderAllowed)
                    {
                        Console.WriteLine("Invalid option! Enter (h)it, (s)tand, (sp)lit, (d)ouble or (su)rrender");
                    }
                    else
                    {
                        Console.WriteLine("Invalid option! Enter (h)it, (s)tand, (sp)lit or (d)ouble");
                    }

                    return "";
            }
        }

        public override void CountDealt(Player[] players,List<Card> dealerHand, double remainingDecks)
        {
            if (practice)
            {
                UpdateTrueCount(GetCurrentRunningCount(players, dealerHand), remainingDecks);
            }
            else
            {
                //Console.WriteLine("Human does this on his own");
            }
        }

        public override void UpdateRunningCount(Player[] players, List<Card> dealerHand)
        {
            if (practice)
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
            else
            {
                //Console.WriteLine("Human does this on his own");
            }
        }

        public override void UpdateTrueCount(int runningCount, double remainingDecks)
        {
            if (practice)
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
            else
            {
                //Console.WriteLine("Human does this on his own");
            }
        }

        public override void UpdateTrueCount(double remainingDecks)
        {
            if (practice)
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
            else
            {
                //Console.WriteLine("Human does this on his own");
            }
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
            if (practice)
            {
                runningCount = 0;
                trueCount = 0;
            }
            else
            {
                //Console.WriteLine("Human does this on his own");
            }
        }

        public override void Bet(List<Card> hand, Tuple<int, int> limits)
        {
            int bet;

            do
            {
                if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                {
                    betterUI.DisplayTurn(Name);

                    bet = betterUI.GetIntInput(new string[] { 
                    "How much do you want to bet?",
                    String.Format("You have {0}", chips),
                    String.Format("Bet limits are {0}-{1}", limits.Item1, limits.Item2),
                    bets[0] == 0 ? "(Bet 0 to quit)" : ""
                    });
                }
                else
                {
                    do
                    {
                        Console.WriteLine("{0}, how much do you want to bet?\n" +
                            "You have {1} chips, bet limits are {2}-{3}", Name, chips, limits.Item1, limits.Item2);

                        if (bets[0] == 0)
                        {
                            Console.WriteLine("(Bet 0 to quit)");
                        }
                    } while (!(int.TryParse(Console.ReadLine(), out bet)));
                }               
            } while (!((bet == 0 && bets[0] == 0) || (bet >= limits.Item1 && bet <= limits.Item2 && CheckBet(bet, limits))));

            if (bet == 0 && bets[0] == 0)
            {
                string choice;

                if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                {
                    choice = betterUI.GetStringChoice("Are you sure you want to quit?", new string[] { "yes", "no" });
                }
                else
                {
                    do
                    {
                        Console.WriteLine("Are you sure you want to quit? (Y/N)");
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
                }
                
                if (choice.ToLower() == "yes")
                {
                    IsGone = true;
                }
                else
                {
                    Bet(hand, limits);
                }
            }
            else
            {
                bets[Array.IndexOf(hands, hand)] += bet;
                chips -= bet;
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
            if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
            {
                betterUI.DisplayTurn(Name);
            }
            else
            {
                Console.WriteLine("It's {0}'s turn.", Name);
            }

            string choice;

            if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
            {
                choice = betterUI.GetStringChoice("Insurance is open. Do you want to buy insurance?", new string[] { "yes", "no" });
            }
            else
            {
                do
                {
                    Console.WriteLine("Insurance is open. Do you want to buy insurance? (Y/N)");
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
            }
            
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
                    if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                    {
                        betterUI.DisplayTurn(Name);

                        bet = betterUI.GetIntInput(new string[] {
                    "How much do you want to bet on pairs?",
                    String.Format("You have {0}", chips),
                    String.Format("Bet limits are {0}-{1}", limits.Item1, limits.Item2),
                    "Bet 0 to skip this bet."
                    });
                    }
                    else
                    {
                        do
                        {
                            Console.WriteLine("{0}, how much do you want to bet on pairs?\n" +
                                "You have {1} chips, bet limits are {2}-{3}",Name, chips, limits.Item1, limits.Item2);
                            Console.WriteLine("Bet 0 to skip this bet.");
                        } while (!int.TryParse(Console.ReadLine(), out bet));
                    }                   
                } while (!((bet >= limits.Item1 && bet <= limits.Item2 && CheckBet(bet, limits)) || bet == 0));

                if (bet == 0)
                {
                    string choice;

                    if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                    {
                        choice = betterUI.GetStringChoice("Allow this side bet for future rounds?", new string[] { "yes", "no" });
                    }
                    else
                    {
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
                    }                    

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
            else
            {
                if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                {
                    betterUI.DisplayMessage("Cannot split");
                }
                else
                {
                    Console.WriteLine("Cannot split");
                }
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
                if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                {
                    betterUI.DisplayMessage("Cannot split");
                }
                else
                {
                    Console.WriteLine("Cannot split");
                }
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
                        &&CheckBet(bets[0], tableLimits)
                        && (isResplitAllowed || hands[1] == null))
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
                    if (bets[i]>=1000000000)
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
                        if (c != null)
                        {
                            Console.Write("{0} ", c.Name);
                        }
                    }
                    Console.WriteLine();
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
            surrender = false;
        }
        //Basic strategy data: https://wizardofodds.com/games/blackjack/strategy/4-decks/
        //Deviations data: https://www.reddit.com/r/blackjack/comments/5fgf1a/deviations/, https://quizlet.com/18561678/blackjack-h17-deviations-flash-cards/, https://www.888casino.com/blog/advanced-card-counting-blackjack-strategy-deviations, 
        //best deviations data https://digitalcommons.usu.edu/cgi/viewcontent.cgi?article=1528&context=gradreports, https://www.blackjacktheforum.com/showthread.php?17600-H17-Deviations-Correct-Expert-OpinionS, https://wizardofodds.com/games/blackjack/card-counting/high-low/
        public string GetCorrectChoice(List<Card> hand, Tuple<int, int> limits, Player[] players, Dealer dealer)
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
                            return CHOICE_DOUBLE;
                        }
                        else if ((GetHandValue(hand).Item2 == 15 || GetHandValue(hand).Item2 == 16)
                            && (dealer.hand[0].GetCardValue() == 4 || dealer.hand[0].GetCardValue() == 5 || dealer.hand[0].GetCardValue() == 6)
                            && chips >= 1)
                        {
                            return CHOICE_DOUBLE;
                        }
                        else if ((GetHandValue(hand).Item2 == 17 || GetHandValue(hand).Item2 == 18)
                            && (dealer.hand[0].GetCardValue() == 3 || dealer.hand[0].GetCardValue() == 4 || dealer.hand[0].GetCardValue() == 5 || dealer.hand[0].GetCardValue() == 6)
                            && chips >= 1)
                        {
                            return CHOICE_DOUBLE;
                        }
                        else if ((GetHandValue(hand).Item2 == 18)
                            && (dealer.hand[0].GetCardValue() == 2)
                            && chips >= 1)
                        {
                            return CHOICE_DOUBLE;
                        }
                        else if ((GetHandValue(hand).Item2 == 19)
                            && (dealer.hand[0].GetCardValue() == 6)
                            && trueCount >= 0 //deviation
                            && chips >= 1)
                        {
                            return CHOICE_DOUBLE;
                        }
                        else if ((GetHandValue(hand).Item2 == 17)//deviation
                            && (dealer.hand[0].GetCardValue() == 2)
                            && trueCount >= 1
                            && chips >= 1)
                        {
                            return CHOICE_DOUBLE;
                        }
                        else if ((GetHandValue(hand).Item2 == 19)//deviation
                            && (dealer.hand[0].GetCardValue() == 5)
                            && trueCount >= 1
                            && chips >= 1)
                        {
                            return CHOICE_DOUBLE;
                        }
                        else if ((GetHandValue(hand).Item2 == 19)//deviation
                            && (dealer.hand[0].GetCardValue() == 4)
                            && trueCount >= 3
                            && chips >= 1)
                        {
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
                                return CHOICE_DOUBLE;
                            }
                            else if ((GetHandValue(hand).Item2 == 10)
                                && (dealer.hand[0].GetCardValue() >= 2 && dealer.hand[0].GetCardValue() <= 9)
                                && chips >= 1)
                            {
                                return CHOICE_DOUBLE;
                            }
                            else if ((GetHandValue(hand).Item2 == 11)
                                && chips >= 1)
                            {
                                return CHOICE_DOUBLE;
                            }
                            else if ((GetHandValue(hand).Item2 == 10) //deviation
                                && dealer.hand[0].GetCardValue() == 10
                                && trueCount >= 4
                                && chips >= 1)
                            {
                                return CHOICE_DOUBLE;
                            }
                            else if ((GetHandValue(hand).Item2 == 10) //deviation
                                && dealer.hand[0] is CardAce
                                && trueCount >= 3
                                && chips >= 1)
                            {
                                return CHOICE_DOUBLE;
                            }
                            else if ((GetHandValue(hand).Item2 == 9) //deviation
                                && dealer.hand[0].GetCardValue() == 2
                                && trueCount >= 1
                                && chips >= 1)
                            {
                                return CHOICE_DOUBLE;
                            }
                            else if ((GetHandValue(hand).Item2 == 9) //deviation
                                && dealer.hand[0].GetCardValue() == 7
                                && trueCount >= 3
                                && chips >= 1)
                            {
                                return CHOICE_DOUBLE;
                            }
                            else if ((GetHandValue(hand).Item2 == 8) //deviation
                                && dealer.hand[0].GetCardValue() == 6
                                && trueCount >= 2
                                && chips >= 1)
                            {
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
                            return CHOICE_DOUBLE;
                        }
                        else if ((GetHandValue(hand).Item2 == 15 || GetHandValue(hand).Item2 == 16)
                            && (dealer.hand[0].GetCardValue() == 4 || dealer.hand[0].GetCardValue() == 5 || dealer.hand[0].GetCardValue() == 6)
                            && chips >= 1)
                        {
                            return CHOICE_DOUBLE;
                        }
                        else if ((GetHandValue(hand).Item2 == 17 || GetHandValue(hand).Item2 == 18)
                            && (dealer.hand[0].GetCardValue() == 3 || dealer.hand[0].GetCardValue() == 4 || dealer.hand[0].GetCardValue() == 5 || dealer.hand[0].GetCardValue() == 6)
                            && chips >= 1)
                        {
                            return CHOICE_DOUBLE;
                        }
                        else if ((GetHandValue(hand).Item2 == 18)
                            && (dealer.hand[0].GetCardValue() == 2)
                            && chips >= 1)
                        {
                            return CHOICE_DOUBLE;
                        }
                        else if ((GetHandValue(hand).Item2 == 19)
                            && (dealer.hand[0].GetCardValue() == 6)
                            && trueCount >= 1 //deviation
                            && chips >= 1)
                        {
                            return CHOICE_DOUBLE;
                        }
                        else if ((GetHandValue(hand).Item2 == 17)//deviation
                            && (dealer.hand[0].GetCardValue() == 2)
                            && trueCount >= 1
                            && chips >= 1)
                        {
                            return CHOICE_DOUBLE;
                        }
                        else if ((GetHandValue(hand).Item2 == 19)//deviation
                            && (dealer.hand[0].GetCardValue() == 5)
                            && trueCount >= 1
                            && chips >= 1)
                        {
                            return CHOICE_DOUBLE;
                        }
                        else if ((GetHandValue(hand).Item2 == 19)//deviation
                            && (dealer.hand[0].GetCardValue() == 4)
                            && trueCount >= 3
                            && chips >= 1)
                        {
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
                                return CHOICE_DOUBLE;
                            }
                            else if ((GetHandValue(hand).Item2 == 10)
                                && (dealer.hand[0].GetCardValue() >= 2 && dealer.hand[0].GetCardValue() <= 9)
                                && chips >= 1)
                            {
                                return CHOICE_DOUBLE;
                            }
                            else if ((GetHandValue(hand).Item2 == 11)
                                && !(dealer.hand[0] is CardAce)
                                && chips >= 1)
                            {
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
                                return CHOICE_DOUBLE;
                            }
                            else if ((GetHandValue(hand).Item2 == 10) //deviation
                                && dealer.hand[0] is CardAce
                                && trueCount >= 3
                                && chips >= 1)
                            {
                                return CHOICE_DOUBLE;
                            }
                            else if ((GetHandValue(hand).Item2 == 9) //deviation
                                && dealer.hand[0].GetCardValue() == 2
                                && trueCount >= 1
                                && chips >= 1)
                            {
                                return CHOICE_DOUBLE;
                            }
                            else if ((GetHandValue(hand).Item2 == 9) //deviation
                                && dealer.hand[0].GetCardValue() == 7
                                && trueCount >= 3
                                && chips >= 1)
                            {
                                return CHOICE_DOUBLE;
                            }
                            else if ((GetHandValue(hand).Item2 == 8) //deviation
                                && dealer.hand[0].GetCardValue() == 6
                                && trueCount >= 2
                                && chips >= 1)
                            {
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
    
    }
}
