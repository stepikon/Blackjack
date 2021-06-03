using System;
using System.Collections.Generic;
using System.Text;

namespace Blackjack
{
    //part of a <Strategy pattern>
    //STRUKTURU STRATEGY PATTERNU JSEM PREVZAL Z https://refactoring.guru/design-patterns/strategy/csharp/example

    class RORSimulation : Simulation
    {
        List<int>[] roundsToDouble;
        List<double>[] finalChips;

        public RORSimulation(Dealer dealer, Tuple<int, int> tableLimits, Player[] players, Random random, BetterUI betterUI,
            int AIsAmount, int repetitions, List<int>[] roundsToDouble, List<double>[] finalChips, bool isVisible, bool AILeaves,
            string[] name, int[] chips, bool isSurrenderAllowed, bool isDASAllowed, bool isResplitAllowed, bool isResplitAcesAllowed, int[] betUnit, int[] betSpreadMultiplier, bool wait,
            int runningCount = 0, double trueCount = 0)
            : base(dealer, tableLimits, players, random, betterUI,
            AIsAmount, repetitions, isVisible, AILeaves, name, chips, isSurrenderAllowed, isDASAllowed, isResplitAllowed, isResplitAcesAllowed, betUnit, betSpreadMultiplier, wait,
            runningCount = 0, trueCount = 0)
        {
            this.roundsToDouble = roundsToDouble;
            this.finalChips = finalChips;
        }


        public override void Run()
        {
            for (int i = 0; i < AIsAmount; i++)
            {
                roundsToDouble[i] = new List<int>();
                finalChips[i] = new List<double>();
            }

            dealer.CreateShoe();

            //simulation with display. If isVisible = false, then the same simulation will be run, 
            //but it will be invisible. This adds so many lines of code, but for performance reasons 
            //I only want this expression to be evaluated once.
            if (isVisible)
            {
                bool isTrueCountUnderD1;

                for (int repetition = 0; repetition < repetitions; repetition++)
                {
                    int roundsPlayed = 0;
                    dealer.Shuffle();
                    dealer.Reset();
                    bool dealerSkips; //skips dealer's turn. Dealer skips his turn if all players are over 21, all have blackjack, all surrendered or some combination of those situations

                    //initializes AI
                    DoInitializeAIAlgorithm();

                    //initial check
                    DoInitialCheckAlgorithm();

                    do
                    {
                        isTrueCountUnderD1 = false;
                        roundsPlayed++;
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

                        //checks if anyone has doubled their chips
                        for (int i = 0; i < players.Length; i++)
                        {
                            if (players[i] != null && !players[i].IsGone)
                            {
                                if (players[i].Chips >= chips[i]*2)
                                {
                                    players[i].IsGone = true;
                                    roundsToDouble[i].Add(roundsPlayed);
                                }
                            }
                        }

                        //Shuffle if necessary
                        DoShuffleAlgorithm(isTrueCountUnderD1);
                    } while (ExistActivePlayers());

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
                    int roundsPlayed = 0;
                    dealer.Shuffle();
                    dealer.Reset();
                    bool dealerSkips; //skips dealer's turn. Dealer skips his turn if all players are over 21, all have blackjack, all surrendered or some combination of those situations

                    //initializes AI
                    DoInitializeAIAlgorithm();

                    //initial check
                    DoInitialCheckAlgorithm();


                    //displays current repetition
                    DisplayRepetition(repetition);

                    do
                    {
                        isTrueCountUnderD1 = false;
                        roundsPlayed++;
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

                        //checks if anyone has doubled their chips
                        for (int i = 0; i < players.Length; i++)
                        {
                            if (players[i] != null && !players[i].IsGone)
                            {
                                if (players[i].Chips >= chips[i] * 2)
                                {
                                    players[i].IsGone = true;
                                    roundsToDouble[i].Add(roundsPlayed);
                                }
                            }
                        }
                    } while (ExistActivePlayers());

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
            for (int i = 0; i < players.Length; i++)
            {
                int ruinCounter = 0;

                if (players[i] != null && roundsToDouble[i] != null)
                {
                    for (int j = 0; j < finalChips[i].Count; j++)
                    {
                        if (finalChips[i][j] < tableLimits.Item1)
                        {
                            ruinCounter++;
                        }
                    }

                    double sum = 0;
                    double average;

                    for (int j = 0; j < roundsToDouble[i].Count; j++)
                    {
                        sum += roundsToDouble[i][j];
                    }

                    average = sum / roundsToDouble[i].Count;

                    Console.WriteLine("AI {0} \n Risk of ruin: {1}% \n rounds to double: {2}", players[i].Name, (float)100*ruinCounter/repetitions, average);
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

