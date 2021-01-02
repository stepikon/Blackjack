using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;

namespace Blackjack
{
    class HumanPlayer:Player
    {

        public HumanPlayer(string name, List<Card> hand, BetterUI betterUI, int chips, Tuple<int,int> tableLimits,
            bool isSurrenderAllowed, bool isDASAllowed, bool isResplitAllowed) :
            base(name, hand, betterUI, chips, tableLimits, isSurrenderAllowed, isDASAllowed, isResplitAllowed)
        {
        }

        public string GetChoice()
        {
            if (isSurrenderAllowed)
            {
                Console.WriteLine("Enter(h)it, (s)tand, (sp)lit, (d)ouble or (su)rrender");
            }
            else
            {
                Console.WriteLine("Enter(h)it, (s)tand, (sp)lit, or (d)ouble");
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
                    else if (isDouble || (hands[i].Count == 1 && hands[i][0] is CardAce)) //after doubling you only get 1 card. The same rule apply if you split AA.
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

        public override void CountDealt(Player[] players,List<Card> dealerHand, double remainingDecks)
        {
            //Console.WriteLine("Human does this on his own");
        }

        public override void UpdateRunningCount(Player[] players, List<Card> dealerHand)
        {
            //Console.WriteLine("Human does this on his own");
        }

        public override void UpdateTrueCount(int runningCount, double remainingDecks)
        {
            //Console.WriteLine("Human does this on his own");
        }

        public override void UpdateTrueCount(double remainingDecks)
        {
            //Console.WriteLine("Human does this on his own");
        }

        public override void ResetCounts()
        {
            //Console.WriteLine("Human does this on his own");
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
    }
}
