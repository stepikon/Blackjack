using System;
using System.Collections.Generic;
using System.Text;

namespace Blackjack
{
    //part of a <Strategy pattern>
    //STRUKTURU STRATEGY PATTERNU JSEM PREVZAL Z https://refactoring.guru/design-patterns/strategy/csharp/example

    class EVSimulation : Simulation
    {
        int handsPerCycle;
        List<double>[] finalChips;

        public EVSimulation(Dealer dealer, Tuple<int, int> tableLimits, Player[] players, Random random, BetterUI betterUI,
            int AIsAmount, int handsPerCycle, int repetitions, List<double>[] finalChips, bool isVisible, bool AILeaves,
            string[] name, int[] chips, bool isSurrenderAllowed, bool isDASAllowed, bool isResplitAllowed, bool isResplitAcesAllowed, int[] betUnit, int[] betSpreadMultiplier, bool wait,
            int runningCount = 0, double trueCount = 0)
            : base(dealer, tableLimits, players, random, betterUI,
            AIsAmount, repetitions, isVisible, AILeaves, name, chips, isSurrenderAllowed, isDASAllowed, isResplitAllowed, isResplitAcesAllowed, betUnit, betSpreadMultiplier, wait,
            runningCount = 0, trueCount = 0)
        {
            this.handsPerCycle = handsPerCycle;
            this.finalChips = finalChips;
        }


        public override void Run()
        {
            for (int i = 0; i < AIsAmount; i++)
            {
                finalChips[i] = new List<double>();
            }

            dealer.CreateShoe();

            //simulation with display. If isVisible = false, then the same simulation will be run, 
            //but it will be invisible. This adds so many lines of code, but for performance reasons 
            //I want this expression to be evaluated only once.
            if (isVisible) 
            {
                bool isTrueCountUnderD1;

                for (int repetition = 0; repetition < repetitions; repetition++)
                {
                    dealer.Shuffle();
                    dealer.Reset();
                    bool dealerSkips; //skips dealer's turn. Dealer skips his turn if all players are over 21, all have blackjack, all surrendered or some combination of those situations

                    //initializes AI
                    DoInitializeAIAlgorithm();

                    //initial check
                    DoInitialCheckAlgorithm();

                    for (int currentHand = 0; currentHand < handsPerCycle; currentHand++)
                    {
                        isTrueCountUnderD1 = false;
                        dealerSkips = true;

                        if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                        {
                            betterUI.ClearAll();
                            betterUI.DisplayTableRules(dealer.HitSoft17);
                            betterUI.DisplayLimits(tableLimits);
                            //displays current repetition
                            DisplayRepetition(repetition);
                        }
                        else
                        {
                            Console.Clear();
                            //displays current repetition
                            DisplayRepetition(repetition);
                        }

                        //Betting
                        DoBettingAlgorithm();

                        if (Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH)
                        {
                            Console.WriteLine("-----");
                        }

                        //Dealing
                        DoDealingAlgorithm();

                        //displays statuses
                        DoDisplayStatusesAlgorithm();

                        //Pair bonus gets paid always first
                        DoVisibleBonusPaymentAlgorithm();

                        //checks blackjacks
                        DoSetBlackjackAlgorithm();

                        //Displays hands
                        DoDisplayHandsAlgorithm();

                        //offers insurance if dealer shows an Ace
                        DoVisibleOfferInsuranceAlgorithm();

                        //displays blackjacks
                        DoDisplayBlackjacksAlgorithm();

                        //players turns
                        DoVisiblePlayersTurnsAlgorithm();

                        //Dealer reveals his cards
                        dealer.RevealHidden();
                        if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                        {
                            betterUI.DisplayDealerStatus(dealer);
                        }
                        else
                        {
                            dealer.DisplayHand();
                            Console.WriteLine("-----");
                        }

                        //checks if dealer may take his turn
                        dealerSkips = DoCheckDealersTurnAlgorithm(dealerSkips);

                        //Dealers turn
                        DoVisibleDealersTurnAlgorithm(dealerSkips);

                        //outcomes
                        if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                        {
                            betterUI.ClearMessages();
                        }

                        DoOutcomesAlgorithm();

                        if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                        {
                            betterUI.DisplayDealerStatus(dealer);
                            betterUI.DisplayPlayersStatus(players);
                        }
                        else
                        {
                            Console.WriteLine("-----");
                        }


                        //updates counts
                        isTrueCountUnderD1 = DoVisibleUpdateCountsAlgorithm(isTrueCountUnderD1);

                        //Reset
                        DoResetAlgorithm();

                        //checks if anyone is ruined
                        DoCheckRuinAlgorithm();

                        //Shuffle if necessary
                        DoShuffleAlgorithm(isTrueCountUnderD1);

                        if (!ExistActivePlayers())
                        {
                            break;
                        }
                    }

                    for (int i = 0; i < AIsAmount; i++)
                    {
                        if (finalChips[i] != null && players[i] != null)
                        {
                            finalChips[i].Add(players[i].Chips);
                        }
                    }
                }
            }
            else //the same simulation, but invisible.
            {
                bool isTrueCountUnderD1;

                for (int repetition = 0; repetition < repetitions; repetition++)
                {
                    dealer.Shuffle();
                    dealer.Reset();
                    bool dealerSkips; //skips dealer's turn. Dealer skips his turn if all players are over 21, all have blackjack, all surrendered or some combination of those situations

                    //initializes AI
                    DoInitializeAIAlgorithm();

                    //initial check
                    DoInitialCheckAlgorithm();

                    //displays current repetition
                    DisplayRepetition(repetition);

                    for (int currentHand = 0; currentHand < handsPerCycle; currentHand++)
                    {
                        isTrueCountUnderD1 = false;
                        dealerSkips = true;

                        //Betting
                        DoBettingAlgorithm();

                        //Dealing
                        DoDealingAlgorithm();

                        //Pair bonus gets paid always first
                        DoInvisibleBonusPaymentAlgorithm();

                        //checks blackjacks
                        DoSetBlackjackAlgorithm();

                        //offers insurance if dealer shows an Ace
                        DoInvisibleOfferInsuranceAlgorithm();

                        //players turns
                        DoInvisiblePlayersTurnsAlgorithm();

                        //Dealer reveals his cards
                        dealer.RevealHidden();

                        //checks if dealer may take his turn
                        dealerSkips = DoCheckDealersTurnAlgorithm(dealerSkips);

                        //Dealers turn
                        DoInvisibleDealersTurnAlgorithm(dealerSkips);

                        //outcomes
                        DoOutcomesAlgorithm();

                        //updates counts
                        isTrueCountUnderD1 = DoInvisibleUpdateCountsAlgorithm(isTrueCountUnderD1);

                        //Reset
                        DoResetAlgorithm();

                        //checks if anyone is ruined
                        DoCheckRuinAlgorithm();

                        //Shuffle if necessary
                        DoShuffleAlgorithm(isTrueCountUnderD1);

                        if (!ExistActivePlayers())
                        {
                            break;
                        }
                    }

                    for (int i = 0; i < AIsAmount; i++)
                    {
                        if (finalChips[i] != null && players[i] != null)
                        {
                            finalChips[i].Add(players[i].Chips);
                        }
                    }
                }
            }

            betterUI.ClearAll();

            //displays simulation outcomes
            for (int i = 0; i < 7; i++)
            {
                int brokeCounter = 0;

                if (players[i] != null && finalChips[i] != null)
                {
                    double sum = 0;
                    double average;

                    for (int j = 0; j < finalChips[i].Count; j++)
                    {
                        sum += finalChips[i][j];

                        if (finalChips[i][j]<tableLimits.Item1)
                        {
                            brokeCounter++;
                        }
                    }

                    average = sum / finalChips[i].Count;

                    Console.WriteLine("AI {0} was starting on {1} chips and ended on {2} chips in average. Gone broke {3} times", players[i].Name, chips[i], average, brokeCounter);
                }
            }

            Console.WriteLine();
            Console.WriteLine("Press any key...");
            Console.ReadKey(true);
            Console.Clear();
        }
    }

    //</Strategy pattern>
}

