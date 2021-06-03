using System;
using System.Collections.Generic;
using System.Text;

namespace Blackjack
{
    //<after>

    class Simulation : GameStyleModsParent
    {
        //Simulation setup
        protected int AIsAmount;
        protected int repetitions;
        protected bool isVisible; //makes the simulation visible
        protected bool AILeaves;  //If trueCount <= -1, all AIs leave the table and find another one.
                                  //In this program, AI won't go away from the table; instead, the dealer will shuffle the cards.
                                  //It will have the same effect.

        //AI
        protected string[] name;
        protected int[] chips;
        protected bool isSurrenderAllowed;
        protected bool isDASAllowed;
        protected bool isResplitAllowed;
        protected bool isResplitAcesAllowed;
        protected int[] betUnit;
        protected int[] betSpreadMultiplier;
        protected bool wait;
        protected int runningCount;
        protected double trueCount;


        public Simulation(Dealer dealer, Tuple<int, int> tableLimits, Player[] players, Random random, BetterUI betterUI,
            int AIsAmount, int repetitions, bool isVisible, bool AILeaves,
            string[] name, int[] chips, bool isSurrenderAllowed, bool isDASAllowed, bool isResplitAllowed, bool isResplitAcesAllowed, int[] betUnit, int[] betSpreadMultiplier, bool wait,
            int runningCount = 0, double trueCount = 0)
            :base(dealer, tableLimits, players, random, betterUI)
        {
            //Simulation
            this.AIsAmount = AIsAmount; //user is allowed to enter a number 1-7
            this.repetitions = repetitions;
            this.isVisible = isVisible;
            this.AILeaves = AILeaves;

            //AI
            this.name = name;
            this.chips = chips;
            this.isSurrenderAllowed = isSurrenderAllowed;
            this.isDASAllowed = isDASAllowed;
            this.isResplitAllowed = isResplitAllowed;
            this.isResplitAcesAllowed = isResplitAcesAllowed;
            this.betUnit = betUnit;
            this.betSpreadMultiplier = betSpreadMultiplier;
            this.wait = wait;
            this.runningCount = runningCount;
            this.trueCount = trueCount;
        }


        protected void DoInitializeAIAlgorithm()
        {
            for (int i = 0; i < AIsAmount; i++)
            {
                players[i] = new CardCountingAI(name[i], new List<Card>(), betterUI, chips[i], tableLimits,
                    isSurrenderAllowed, isDASAllowed, isResplitAllowed, isResplitAcesAllowed,
                    betUnit[i], betSpreadMultiplier[i], wait,
                    runningCount, trueCount, isVisible);
            }
        }


        protected void DisplayRepetition(int repetition)
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("repetition: " + repetition);
        }


        protected void DoInvisibleBonusPaymentAlgorithm()
        {
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
                            p.Chips += p.PairBet * 26;
                        }
                        else if (p.hand[0].GetType() == p.hand[1].GetType()
                            && p.hand[0].Color == p.hand[1].Color)
                        {
                            p.Chips += p.PairBet * 13;
                        }
                        else if (p.hand[0].GetType() == p.hand[1].GetType())
                        {
                            p.Chips += p.PairBet * 7;
                        }
                        else
                        {
                        }
                    }
                }
            }
        }


        protected void DoInvisibleOfferInsuranceAlgorithm()
        {
            if (dealer.hand[0] is CardAce)
            {
                foreach (Player p in players)
                {
                    if (!(p == null || p.IsRuined || p.IsGone))
                    {
                        p.CountDealt(players, dealer.hand, dealer.DeckAmount - dealer.GetDecksInDiscard());
                        p.BetInsurance();
                    }
                }
            }
        }


        protected void DoInvisiblePlayersTurnsAlgorithm()
        {
            if (!dealer.HasBlackjack)
            {
                foreach (Player p in players)
                {
                    if (!(p == null || p.IsRuined || p.IsGone))
                    {
                        p.CountDealt(players, dealer.hand, dealer.DeckAmount - dealer.GetDecksInDiscard());

                        p.TakeTurn(players, dealer);
                    }
                }
            }
        }


        protected void DoInvisibleDealersTurnAlgorithm(bool dealerSkips)
        {
            if (!dealerSkips)
            {
                dealer.TakeTurn();
            }
        }


        protected virtual bool DoVisibleUpdateCountsAlgorithm(bool isTrueCountUnderD1)
        {
            foreach (Player p in players)
            {
                if (p != null)
                {
                    p.UpdateRunningCount(players, dealer.hand);
                    p.UpdateTrueCount(dealer.DeckAmount - dealer.GetDecksInDiscard());

                    Console.SetCursorPosition(0, 0);
                    Console.WriteLine("Counts: RC{0}, TC{1}", p.RunningCount, p.TrueCount);

                    isTrueCountUnderD1 = isTrueCountUnderD1 || p.TrueCount <= -1;
                }
            }

            return isTrueCountUnderD1;
        }


        protected virtual bool DoInvisibleUpdateCountsAlgorithm(bool isTrueCountUnderD1)
        {
            foreach (Player p in players)
            {
                if (p != null)
                {
                    p.UpdateRunningCount(players, dealer.hand);
                    p.UpdateTrueCount(dealer.DeckAmount - dealer.GetDecksInDiscard());

                    isTrueCountUnderD1 = isTrueCountUnderD1 || p.TrueCount <= -1;
                }
            }

            return isTrueCountUnderD1;
        }


        protected void DoShuffleAlgorithm(bool isTrueCountUnderD1)
        {
            if (dealer.CardToDeal >= dealer.DeckPenetration || (AILeaves && isTrueCountUnderD1))
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
        }


        //HAND OUTCOMES
        protected override void Surrender(Player player, int handIndex)
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


        protected override void Win(Player player, int handIndex)
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


        protected override void Lose(Player player, int handIndex)
        {
            if (isVisible)
            {
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


        protected override void Push(Player player, int handIndex)
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


        protected override void WinInsurance(Player player, double insurance)
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
    }

    //</After>
}
