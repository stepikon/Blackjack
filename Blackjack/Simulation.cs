﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Blackjack
{
    class Simulation : IPlayable
    {
        //to display everything in a more fancy way
        private const int MINIMUM_WINDIW_WIDTH = 7 * 25;
        private const int MINIMUM_WINDIW_HEIGHT = 38;

        //Game setup
        Dealer dealer;
        Tuple<int, int> tableLimits;
        Player[] players;
        BetterUI betterUI;
        Random random;

        //Simulation setup
        int AIsAmount;
        int handsPerCycle;
        int repetions;
        List<double>[] finalChips;
        bool isVisible; //makes the simulation visible

        //AI
        string[] name;
        int[] chips;
        bool isSurrenderAllowed;
        bool isDASAllowed;
        bool isResplitAllowed;
        int betSpreadMultiplier;
        bool wait;
        int runningCount;
        double trueCount;

        public Simulation(Dealer dealer, Tuple<int, int> tableLimits, Player[] players, Random random, BetterUI betterUI,
/*simulation*/int AIsAmount, int handsPerCycle, int repetions, List<double>[] finalChips, bool isVisible,
     /*AI:*/string[] name, int[] chips, bool isSurrenderAllowed, bool isDASAllowed, bool isResplitAllowed, int betSpreadMultiplier, bool wait,
            int runningCount = 0, double trueCount = 0)
        {
            this.dealer = dealer;
            this.tableLimits = tableLimits;
            this.players = players; //will be set to 7 during creation
            this.random = random;
            this.betterUI = betterUI;

            //Simulation
            this.AIsAmount = AIsAmount; //user is allowed to enter a number 1-7
            this.handsPerCycle = handsPerCycle;
            this.repetions = repetions;
            this.finalChips = finalChips;
            this.isVisible = isVisible;

            //AI
            this.name = name;
            this.chips = chips;
            this.isSurrenderAllowed = isSurrenderAllowed;
            this.isDASAllowed = isDASAllowed;
            this.isResplitAllowed = isResplitAllowed;
            this.betSpreadMultiplier = betSpreadMultiplier;
            this.wait = wait;
            this.runningCount = runningCount;
            this.trueCount = trueCount;
        }

        public void Run()
        {
            for (int i = 0; i < AIsAmount; i++)
            {
                finalChips[i] = new List<double>();
            }

            //simulation with display. If isVisible = false, then the same simulation will be run, 
            //but it will be invisible. This adds so many lines of code, but for performance reasons 
            //I only want this expression to be evaluated once.
            if (isVisible) 
            {
                for (int repetion = 0; repetion < repetions; repetion++)
                {
                    dealer.CreateShoe();
                    dealer.Shuffle();
                    dealer.Reset();
                    bool dealerSkips; //skips dealer turn. Dealer skips his turn if all players are over 21, all have blackjack, all surrendered or some combination of those situations

                    for (int i = 0; i < AIsAmount; i++)
                    {
                        players[i] = new CardCountingAI(name[i], new List<Card>(), betterUI, chips[i], tableLimits,
                            isSurrenderAllowed, isDASAllowed, isResplitAllowed,
                            Math.Max(tableLimits.Item1, chips[i] / 1000), betSpreadMultiplier, wait,
                            runningCount, trueCount, isVisible);
                    }

                    //initial check
                    foreach (Player p in players)
                    {
                        if (p != null)
                        {
                            p.IsRuined = p.Chips < tableLimits.Item1;
                        }
                    }

                    for (int currentHand = 0; currentHand < handsPerCycle; currentHand++)
                    {
                        dealerSkips = true;
                        betterUI.ClearAll();

                        if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                        {
                            betterUI.DisplayTableRules(dealer.HitSoft17);
                            betterUI.DisplayLimits(tableLimits);
                            Console.SetCursorPosition(0, 0);
                            Console.WriteLine("repetition: " + repetion);
                        }

                        //Betting
                        foreach (Player p in players)
                        {
                            if (!(p == null || p.IsRuined || p.IsGone))
                            {
                                p.Bet(p.hands[0], tableLimits);

                                if (!p.IsGone)
                                {
                                    p.BetPair(tableLimits);
                                }
                            }
                        }
                        if (Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH)
                        {
                            Console.WriteLine("-----");
                        }

                        //Dealing
                        for (int i = 0; i < 2; i++)
                        {
                            for (int j = 0; j < players.Length; j++)
                            {
                                if (!(players[j] == null || players[j].IsRuined || players[j].IsGone))
                                {
                                    dealer.Deal(players[j], 0);
                                }
                            }

                            if (i == 0)
                            {
                                dealer.Deal(dealer, 0);
                            }
                            else
                            {
                                dealer.DealHidden();
                            }
                        }

                        //displays statuses
                        if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                        {
                            betterUI.DisplayPlayersStatus(players);
                            betterUI.DisplayDealerStatus(dealer);
                        }

                        //Pair bonus gets paid always first
                        foreach (Player p in players)
                        {
                            if (!(p == null || p.IsRuined || p.IsGone))
                            {
                                if (p.PairBet != 0)
                                {
                                    if (p.hand[0].GetType() == p.hand[1].GetType()
                                        && p.hand[0].Color == p.hand[1].Color
                                        && p.hand[0].Suit == p.hand[1].Suit)
                                    {
                                        if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                                        {
                                            betterUI.DisplayPairs(players, p, "Perfect pair.");
                                        }
                                        else
                                        {
                                            Console.WriteLine("{0}: Perfect pair.", p.Name);
                                        }
                                        p.Chips += p.PairBet * 26;
                                    }
                                    else if (p.hand[0].GetType() == p.hand[1].GetType()
                                        && p.hand[0].Color == p.hand[1].Color)
                                    {
                                        if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                                        {
                                            betterUI.DisplayPairs(players, p, "Color pair.");
                                        }
                                        else
                                        {
                                            Console.WriteLine("{0}: Color pair.", p.Name);
                                        }
                                        p.Chips += p.PairBet * 13;
                                    }
                                    else if (p.hand[0].GetType() == p.hand[1].GetType())
                                    {
                                        if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                                        {
                                            betterUI.DisplayPairs(players, p, "Pair.");
                                        }
                                        else
                                        {
                                            Console.WriteLine("{0}: Pair.", p.Name);
                                        }
                                        p.Chips += p.PairBet * 7;
                                    }
                                    else
                                    {
                                        if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                                        {
                                            betterUI.DisplayPairs(players, p, "Not a pair.");
                                        }
                                        else
                                        {
                                            Console.WriteLine("{0}: Not a pair.", p.Name);
                                        }
                                    }
                                }
                            }
                        }

                        //checks blackjacks
                        foreach (Player p in players)
                        {
                            if (!(p == null || p.IsRuined || p.IsGone))
                            {
                                p.SetHasBlackjack();
                            }
                        }
                        dealer.SetHasBlackjack();

                        //Displays hands
                        foreach (Player p in players)
                        {
                            if (!(p == null || p.IsRuined || p.IsGone))
                            {
                                if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                                {
                                    betterUI.DisplayPlayersStatus(players);
                                }
                                else
                                {
                                    p.DisplayHands();
                                }
                            }
                        }
                        if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                        {
                            betterUI.DisplayDealerStatus(dealer);
                        }
                        else
                        {
                            Console.WriteLine();
                            dealer.DisplayHand();
                            Console.WriteLine("-----");
                        }

                        //offers insurance if dealer shows an Ace
                        if (dealer.hand[0] is CardAce)
                        {
                            foreach (Player p in players)
                            {
                                if (!(p == null || p.IsRuined || p.IsGone))
                                {
                                    p.CountDealt(players, dealer.hand, dealer.DeckAmount - dealer.GetDecksInDiscard());
                                    p.BetInsurance();
                                }

                                if (Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH)
                                {
                                    Console.WriteLine("-----");
                                }
                            }
                        }

                        //displays blackjacks
                        if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                        {
                            betterUI.DisplayPlayersBlackjack(players);
                            betterUI.DisplayDealerBlackjack(dealer);
                        }

                        //players turns
                        if (!dealer.HasBlackjack)
                        {
                            if (dealer.hand[0] is CardAce)
                            {
                                //displays blackjacks
                                if (Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH)
                                {
                                    Console.WriteLine("Nobody home");
                                }
                            }

                            foreach (Player p in players)
                            {
                                if (!(p == null || p.IsRuined || p.IsGone))
                                {
                                    p.CountDealt(players, dealer.hand, dealer.DeckAmount - dealer.GetDecksInDiscard());

                                    if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                                    {
                                        betterUI.ClearMessages();
                                    }

                                    p.TakeTurn(players, dealer);
                                }

                                if (Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH)
                                {
                                    Console.WriteLine("-----");
                                }
                            }
                        }

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
                        foreach (Player p in players)
                        {
                            if (!(p == null || p.IsRuined || p.IsGone))
                            {
                                foreach (int handValue in p.GetHandValues())
                                {
                                    dealerSkips = dealerSkips && (p.HasBlackjack || handValue > 21 || p.Surrender);
                                }
                            }
                        }

                        //Dealers turn
                        if (!dealerSkips)
                        {
                            if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                            {
                                betterUI.DisplayTurn(dealer.Name);
                            }
                            else
                            {
                                Console.WriteLine("It's {0}'s turn.", dealer.Name);
                            }

                            dealer.TakeTurn();
                        }

                        if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                        {
                            betterUI.DisplayDealerStatus(dealer);
                            betterUI.ClearTurn();
                            betterUI.ClearOptionsSpace();
                        }
                        else
                        {
                            dealer.DisplayHand();
                            Console.WriteLine("-----");
                        }

                        //outcomes
                        if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                        {
                            betterUI.ClearMessages();
                        }

                        foreach (Player p in players)
                        {
                            if (!(p == null || p.IsRuined || p.IsGone))
                            {
                                for (int i = 0; i < p.GetHandValues().Count; i++)
                                {
                                    if (p.Surrender)
                                    {
                                        Surrender(p, i);
                                    }
                                    else if ((p.GetHandValues()[i] > dealer.GetHandValue(dealer.hand).Item2
                                        || dealer.GetHandValue(dealer.hand).Item2 > 21
                                        || (p.HasBlackjack && !dealer.HasBlackjack))
                                        && p.GetHandValues()[i] <= 21)
                                    {
                                        Win(p, i);
                                    }
                                    else if (p.GetHandValues()[i] < dealer.GetHandValue(dealer.hand).Item2
                                        || p.GetHandValues()[i] > 21
                                        || (!p.HasBlackjack && dealer.HasBlackjack))
                                    {
                                        Lose(p, i);
                                    }
                                    else
                                    {
                                        Push(p, i);
                                    }
                                }

                                if (dealer.HasBlackjack && p.Insurance != 0)
                                {
                                    WinInsurance(p, p.Insurance);
                                }
                            }
                        }

                        if (!dealer.HasBlackjack)
                        {
                            if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT || Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                            {
                                //betterUI.DisplayMessage(String.Format("Dealer has {0}", dealer.GetHandValue(dealer.hand)));
                            }
                            else
                            {
                                Console.WriteLine();
                                Console.WriteLine(dealer.GetHandValue(dealer.hand).Item2);
                                Console.WriteLine("----");
                            }
                        }

                        if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                        {
                            betterUI.DisplayDealerStatus(dealer);
                            betterUI.DisplayPlayersStatus(players);
                        }


                        //updates counts
                        foreach (Player p in players)
                        {
                            if (p != null)
                            {
                                p.UpdateRunningCount(players, dealer.hand);
                                p.UpdateTrueCount(dealer.DeckAmount - dealer.GetDecksInDiscard());

                                Console.SetCursorPosition(0, 0);
                                Console.WriteLine("DEBUG: RC{0}, TC{1}", p.RunningCount, p.TrueCount);
                            }
                        }

                        //Reset
                        foreach (Player p in players)
                        {
                            if (p != null)
                            {
                                p.ResetHands();
                            }
                        }
                        dealer.ResetHand();

                        //checks if anyone is ruined
                        foreach (Player p in players)
                        {
                            if (p != null)
                            {
                                if (p.Chips < tableLimits.Item1)
                                {
                                    p.IsRuined = true;
                                }
                            }
                        }

                        //Shuffle if necessary
                        if (dealer.CardToDeal >= dealer.DeckPenetration)
                        {
                            dealer.Reset();

                            foreach (Player p in players)
                            {
                                if (p != null)
                                {
                                    p.ResetCounts();
                                }
                            }
                        }

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
                for (int repetion = 0; repetion < repetions; repetion++)
                {
                    dealer.CreateShoe();
                    dealer.Shuffle();
                    dealer.Reset();
                    bool dealerSkips; //skips dealer turn. Dealer skips his turn if all players are over 21, all have blackjack, all surrendered or some combination of those situations

                    for (int i = 0; i < AIsAmount; i++)
                    {
                        players[i] = new CardCountingAI(name[i], new List<Card>(), betterUI, chips[i], tableLimits,
                            isSurrenderAllowed, isDASAllowed, isResplitAllowed,
                            Math.Max(tableLimits.Item1, chips[i] / 1000), betSpreadMultiplier, wait,
                            runningCount, trueCount, isVisible);
                    }

                    //initial check
                    foreach (Player p in players)
                    {
                        if (p != null)
                        {
                            p.IsRuined = p.Chips < tableLimits.Item1;
                        }
                    }

                    for (int currentHand = 0; currentHand < handsPerCycle; currentHand++)
                    {
                        dealerSkips = true;
                        /*betterUI.ClearAll();*/

                        if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                        {
                            /*betterUI.DisplayTableRules(dealer.HitSoft17);
                            betterUI.DisplayLimits(tableLimits);*/
                            Console.SetCursorPosition(0, 0);
                            Console.WriteLine("repetition: " + repetion);
                        }

                        //Betting
                        foreach (Player p in players)
                        {
                            if (!(p == null || p.IsRuined || p.IsGone))
                            {
                                p.Bet(p.hands[0], tableLimits);

                                if (!p.IsGone)
                                {
                                    p.BetPair(tableLimits);
                                }
                            }
                        }
                        if (Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH)
                        {
                            Console.WriteLine("-----");
                        }

                        //Dealing
                        for (int i = 0; i < 2; i++)
                        {
                            for (int j = 0; j < players.Length; j++)
                            {
                                if (!(players[j] == null || players[j].IsRuined || players[j].IsGone))
                                {
                                    dealer.Deal(players[j], 0);
                                }
                            }

                            if (i == 0)
                            {
                                dealer.Deal(dealer, 0);
                            }
                            else
                            {
                                dealer.DealHidden();
                            }
                        }

                        //displays statuses
                        /*if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                        {
                            betterUI.DisplayPlayersStatus(players);
                            betterUI.DisplayDealerStatus(dealer);
                        }*/

                        //Pair bonus gets paid always first
                        foreach (Player p in players)
                        {
                            if (!(p == null || p.IsRuined || p.IsGone))
                            {
                                if (p.PairBet != 0)
                                {
                                    if (p.hand[0].GetType() == p.hand[1].GetType()
                                        && p.hand[0].Color == p.hand[1].Color
                                        && p.hand[0].Suit == p.hand[1].Suit)
                                    {
                                        /*if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                                        {
                                            betterUI.DisplayPairs(players, p, "Perfect pair.");
                                        }
                                        else
                                        {
                                            Console.WriteLine("{0}: Perfect pair.", p.Name);
                                        }*/
                                        p.Chips += p.PairBet * 26;
                                    }
                                    else if (p.hand[0].GetType() == p.hand[1].GetType()
                                        && p.hand[0].Color == p.hand[1].Color)
                                    {
                                        /*if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                                        {
                                            betterUI.DisplayPairs(players, p, "Color pair.");
                                        }
                                        else
                                        {
                                            Console.WriteLine("{0}: Color pair.", p.Name);
                                        }*/
                                        p.Chips += p.PairBet * 13;
                                    }
                                    else if (p.hand[0].GetType() == p.hand[1].GetType())
                                    {
                                        /*if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                                        {
                                            betterUI.DisplayPairs(players, p, "Pair.");
                                        }
                                        else
                                        {
                                            Console.WriteLine("{0}: Pair.", p.Name);
                                        }*/
                                        p.Chips += p.PairBet * 7;
                                    }
                                    else
                                    {
                                        /*if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                                        {
                                            betterUI.DisplayPairs(players, p, "Not a pair.");
                                        }
                                        else
                                        {
                                            Console.WriteLine("{0}: Not a pair.", p.Name);
                                        }*/
                                    }
                                }
                            }
                        }

                        //checks blackjacks
                        foreach (Player p in players)
                        {
                            if (!(p == null || p.IsRuined || p.IsGone))
                            {
                                p.SetHasBlackjack();
                            }
                        }
                        dealer.SetHasBlackjack();

                        //Displays hands
                        /*foreach (Player p in players)
                        {
                            if (!(p == null || p.IsRuined || p.IsGone))
                            {
                                if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                                {
                                    betterUI.DisplayPlayersStatus(players);
                                }
                                else
                                {
                                    p.DisplayHands();
                                }
                            }
                        }
                        if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                        {
                            betterUI.DisplayDealerStatus(dealer);
                        }
                        else
                        {
                            Console.WriteLine();
                            dealer.DisplayHand();
                            Console.WriteLine("-----");
                        }*/

                        //offers insurance if dealer shows an Ace
                        if (dealer.hand[0] is CardAce)
                        {
                            foreach (Player p in players)
                            {
                                if (!(p == null || p.IsRuined || p.IsGone))
                                {
                                    p.CountDealt(players, dealer.hand, dealer.DeckAmount - dealer.GetDecksInDiscard());
                                    p.BetInsurance();
                                }

                                if (Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH)
                                {
                                    Console.WriteLine("-----");
                                }
                            }
                        }

                        //displays blackjacks
                        /*if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                        {
                            betterUI.DisplayPlayersBlackjack(players);
                            betterUI.DisplayDealerBlackjack(dealer);
                        }*/

                        //players turns
                        if (!dealer.HasBlackjack)
                        {
                            /*if (dealer.hand[0] is CardAce)
                            {
                                //displays blackjacks
                                if (Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH)
                                {
                                    Console.WriteLine("Nobody home");
                                }
                            }*/

                            foreach (Player p in players)
                            {
                                if (!(p == null || p.IsRuined || p.IsGone))
                                {
                                    p.CountDealt(players, dealer.hand, dealer.DeckAmount - dealer.GetDecksInDiscard());

                                    /*if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                                    {
                                        betterUI.ClearMessages();
                                    }*/

                                    p.TakeTurn(players, dealer);
                                }

                                /*if (Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH)
                                {
                                    Console.WriteLine("-----");
                                }*/
                            }
                        }

                        //Dealer reveals his cards
                        dealer.RevealHidden();
                        /*if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                        {
                            betterUI.DisplayDealerStatus(dealer);
                        }
                        else
                        {
                            dealer.DisplayHand();
                            Console.WriteLine("-----");
                        }*/

                        //checks if dealer may take his turn
                        foreach (Player p in players)
                        {
                            if (!(p == null || p.IsRuined || p.IsGone))
                            {
                                foreach (int handValue in p.GetHandValues())
                                {
                                    dealerSkips = dealerSkips && (p.HasBlackjack || handValue > 21 || p.Surrender);
                                }
                            }
                        }

                        //Dealers turn
                        if (!dealerSkips)
                        {
                            /*if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                            {
                                betterUI.DisplayTurn(dealer.Name);
                            }
                            else
                            {
                                Console.WriteLine("It's {0}'s turn.", dealer.Name);
                            }*/

                            dealer.TakeTurn();
                        }

                        /*if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                        {
                            betterUI.DisplayDealerStatus(dealer);
                            betterUI.ClearTurn();
                            betterUI.ClearOptionsSpace();
                        }
                        else
                        {
                            dealer.DisplayHand();
                            Console.WriteLine("-----");
                        }*/

                        //outcomes
                        /*if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                        {
                            betterUI.ClearMessages();
                        }*/

                        foreach (Player p in players)
                        {
                            if (!(p == null || p.IsRuined || p.IsGone))
                            {
                                for (int i = 0; i < p.GetHandValues().Count; i++)
                                {
                                    if (p.Surrender)
                                    {
                                        Surrender(p, i);
                                    }
                                    else if ((p.GetHandValues()[i] > dealer.GetHandValue(dealer.hand).Item2
                                        || dealer.GetHandValue(dealer.hand).Item2 > 21
                                        || (p.HasBlackjack && !dealer.HasBlackjack))
                                        && p.GetHandValues()[i] <= 21)
                                    {
                                        Win(p, i);
                                    }
                                    else if (p.GetHandValues()[i] < dealer.GetHandValue(dealer.hand).Item2
                                        || p.GetHandValues()[i] > 21
                                        || (!p.HasBlackjack && dealer.HasBlackjack))
                                    {
                                        Lose(p, i);
                                    }
                                    else
                                    {
                                        Push(p, i);
                                    }
                                }

                                if (dealer.HasBlackjack && p.Insurance != 0)
                                {
                                    WinInsurance(p, p.Insurance);
                                }
                            }
                        }

                        /*if (!dealer.HasBlackjack)
                        {
                            if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT || Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                            {
                                //betterUI.DisplayMessage(String.Format("Dealer has {0}", dealer.GetHandValue(dealer.hand)));
                            }
                            else
                            {
                                Console.WriteLine();
                                Console.WriteLine(dealer.GetHandValue(dealer.hand).Item2);
                                Console.WriteLine("----");
                            }
                        }*/

                        /*if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                        {
                            betterUI.DisplayDealerStatus(dealer);
                            betterUI.DisplayPlayersStatus(players);
                        }*/


                        //updates counts
                        foreach (Player p in players)
                        {
                            if (p != null)
                            {
                                p.UpdateRunningCount(players, dealer.hand);
                                p.UpdateTrueCount(dealer.DeckAmount - dealer.GetDecksInDiscard());

                                Console.SetCursorPosition(0, 0);
                                //Console.WriteLine("DEBUG: RC{0}, TC{1}", p.RunningCount, p.TrueCount);
                            }
                        }

                        //Reset
                        foreach (Player p in players)
                        {
                            if (p != null)
                            {
                                p.ResetHands();
                            }
                        }
                        dealer.ResetHand();

                        //checks if anyone is ruined
                        foreach (Player p in players)
                        {
                            if (p != null)
                            {
                                if (p.Chips < tableLimits.Item1)
                                {
                                    p.IsRuined = true;
                                }
                            }
                        }

                        //Shuffle if necessary
                        if (dealer.CardToDeal >= dealer.DeckPenetration)
                        {
                            dealer.Reset();

                            foreach (Player p in players)
                            {
                                if (p != null)
                                {
                                    p.ResetCounts();
                                }
                            }
                        }

                        if (!ExistActivePlayers())
                        {
                            break;
                        }
                    }

                    //betterUI.ClearAll();

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

            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }

        private void Surrender(Player player, int handIndex)
        {
            player.Chips += 0.5 * player.GetBet(handIndex);

            if (isVisible)
            {
                if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                {
                    betterUI.DisplayOutcomes("SURRENDER", handIndex, player.GetHandValues()[handIndex], players, player);
                }
                else
                {
                    Console.WriteLine("You surrender, returning {0} chips, you now have {1} chips.",
                    0.5 * player.GetBet(handIndex), player.Chips);
                }
            }
        }

        private void Win(Player player, int handIndex)
        {
            player.Chips += player.HasBlackjack ? player.GetBet(handIndex) * 2.5 : player.GetBet(handIndex) * 2;

            if (isVisible)
            {
                if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                {
                    betterUI.DisplayOutcomes("WIN", handIndex, player.GetHandValues()[handIndex], players, player);
                }
                else
                {
                    Console.WriteLine("You won {0} chips, you now have {1} chips.",
                    player.HasBlackjack ? player.GetBet(handIndex) * 2.5 : player.GetBet(handIndex) * 2,
                    player.Chips);
                }
            }
        }

        private void Lose(Player player, int handIndex)
        {
            if (isVisible)
            {
                if (dealer.HasBlackjack)
                {
                    if (Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH)
                    {
                        Console.WriteLine("Dealer has blackjack");
                    }
                }

                if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                {
                    betterUI.DisplayOutcomes("LOSS", handIndex, player.GetHandValues()[handIndex], players, player);
                }
                else
                {
                    Console.WriteLine("You lost, you now have {0} chips.", player.Chips);
                }
            }
        }

        private void Push(Player player, int handIndex)
        {
            player.Chips += player.GetBet(handIndex);

            if (isVisible)
            {
                if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                {
                    betterUI.DisplayOutcomes("PUSH", handIndex, player.GetHandValues()[handIndex], players, player);
                }
                else
                {
                    Console.WriteLine("Push, {0} chips returned, you now have {1} chips.", player.GetBet(handIndex), player.Chips);
                }
            }
        }

        private void WinInsurance(Player player, double insurance)
        {
            player.Chips += insurance * 3;

            if (isVisible)
            {
                if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                {
                    betterUI.DisplayOutcomes("INSURANCE WIN", 0, player.GetHandValues()[0], players, player);
                }
                else
                {
                    Console.WriteLine("You get {0} chips from insurance", insurance * 3);
                }
            }
        }

        private bool ExistActivePlayers()
        {
            bool existActivePlayers = false;

            foreach (Player p in players)
            {
                if (p != null)
                {
                    existActivePlayers = existActivePlayers || !(p.IsRuined || p.IsGone);
                }
            }

            return existActivePlayers;
        }
    }
}
