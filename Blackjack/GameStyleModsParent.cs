using System;
using System.Collections.Generic;
using System.Text;

namespace Blackjack
{
    class GameStyleModsParent : IPlayable
    {
        //to display everything in a fancier way
        protected const int MINIMUM_WINDIW_WIDTH = 7 * 25;
        protected const int MINIMUM_WINDIW_HEIGHT = 39;

        //Game setup
        protected Dealer dealer;
        protected Tuple<int, int> tableLimits;
        protected Player[] players;
        protected BetterUI betterUI;
        protected Random random;


        public GameStyleModsParent(Dealer dealer, Tuple<int, int> tableLimits, Player[] players, Random random, BetterUI betterUI)
        {
            this.dealer = dealer;
            this.tableLimits = tableLimits;
            this.players = players;
            this.random = random;
            this.betterUI = betterUI;
        }


        public virtual void Run() 
        { 
        }


        protected void DoInitialCheckAlgorithm()
        {
            foreach (Player p in players)
            {
                if (p != null)
                {
                    p.IsRuined = p.Chips < tableLimits.Item1;
                }
            }
        }


        protected void DoBettingAlgorithm() 
        {
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
        }


        protected void DoDealingAlgorithm()
        {
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
        }


        protected void DoDisplayStatusesAlgorithm()
        {
            if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
            {
                betterUI.DisplayPlayersStatus(players);
                betterUI.DisplayDealerStatus(dealer);
            }
        }


        protected void DoVisibleBonusPaymentAlgorithm()
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
        }


        protected void DoSetBlackjackAlgorithm()
        {
            foreach (Player p in players)
            {
                if (!(p == null || p.IsRuined || p.IsGone))
                {
                    p.SetHasBlackjack();
                }
            }
            dealer.SetHasBlackjack();
        }


        protected void DoDisplayHandsAlgorithm()
        {
            if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
            {
                betterUI.DisplayPlayersStatus(players);
                betterUI.DisplayDealerStatus(dealer);
            }
            else
            {
                foreach (Player p in players)
                {
                    if (!(p == null || p.IsRuined || p.IsGone))
                    {
                        Console.WriteLine(p.Name);
                        p.DisplayHands();
                    }
                }

                Console.WriteLine();
                Console.WriteLine(dealer.Name);
                dealer.DisplayHand();
                Console.WriteLine("-----");
            }
        }


        protected void DoVisibleOfferInsuranceAlgorithm()
        {
            if (dealer.hand[0] is CardAce)
            {
                foreach (Player p in players)
                {
                    if (!(p == null || p.IsRuined || p.IsGone))
                    {
                        p.CountDealt(players, dealer.hand, dealer.DeckAmount - dealer.GetDecksInDiscard());
                        p.BetInsurance();

                        if (Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH)
                        {
                            Console.WriteLine("-----");
                        }
                    }
                }
            }
        }


        protected void DoDisplayBlackjacksAlgorithm()
        {
            if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
            {
                betterUI.DisplayPlayersBlackjack(players);
                betterUI.DisplayDealerBlackjack(dealer);
            }
        }


        protected void DoVisiblePlayersTurnsAlgorithm()
        {
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

                        if (Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH)
                        {
                            Console.WriteLine("-----");
                        }
                    }
                }
            }
        }


        protected bool DoCheckDealersTurnAlgorithm(bool dealerSkips)
        {
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

            return dealerSkips;
        }


        protected void DoVisibleDealersTurnAlgorithm(bool dealerSkips)
        {
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

                if (Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH)
                {
                    Console.WriteLine("-----");
                }
            }

            if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
            {
                betterUI.DisplayDealerStatus(dealer);
                betterUI.ClearTurn();
                betterUI.ClearOptionsSpace();
            }
            else
            {
                Console.Write("FINAL: ");
                dealer.DisplayHand();

                if (dealer.HasBlackjack)
                {
                    Console.WriteLine("Dealer has blackjack");
                }
                else
                {
                    Console.WriteLine("Dealer has " + dealer.GetHandValue(dealer.hand).Item2);
                }

                Console.WriteLine("-----");
            }
        }


        protected void DoOutcomesAlgorithm()
        {
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
        }


        protected void DoUpdateCountsAlgorithm(bool practice)
        {
            foreach (Player p in players)
            {
                if (p != null)
                {
                    p.UpdateRunningCount(players, dealer.hand);
                    p.UpdateTrueCount(dealer.DeckAmount - dealer.GetDecksInDiscard());

                    if (practice)
                    {
                        if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                        {
                            Console.SetCursorPosition(0, 0);
                            Console.WriteLine("PRACTICE: RC: {0}, TC: {1}", p.RunningCount, p.TrueCount);
                        }
                        else
                        {
                            Console.WriteLine("PRACTICE: RC: {0}, TC: {1}", p.RunningCount, p.TrueCount);
                        }
                    }
                }
            }
        }


        protected void DoResetAlgorithm()
        {
            foreach (Player p in players)
            {
                if (p != null)
                {
                    p.ResetHands();
                }
            }
            dealer.ResetHand();
        }


        protected void DoCheckRuinAlgorithm()
        {
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
        }


        protected void DoShuffleAlgorithm()
        {
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

                if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.SetCursorPosition(0, 37);
                    Console.WriteLine("Shuffling.");
                }
                else
                {
                    Console.WriteLine("Shuffling");
                }
            }
        }


        //HAND OUTCOMES
        protected virtual void Surrender(Player player, int handIndex)
        {
            player.Chips += 0.5 * player.GetBet(handIndex);

            if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
            {
                betterUI.DisplayOutcomes("SURRENDER", handIndex, player.GetHandValues()[handIndex], players, player);
            }
            else
            {
                Console.WriteLine("Player {0}: You surrendered, returning {1} chips, you now have {2} chips.",
                player.Name, 0.5 * player.GetBet(handIndex), player.Chips);
            }
        }


        protected virtual void Win(Player player, int handIndex)
        {
            player.Chips += player.HasBlackjack ? player.GetBet(handIndex) * 2.5 : player.GetBet(handIndex) * 2;

            if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
            {
                betterUI.DisplayOutcomes("WIN", handIndex, player.GetHandValues()[handIndex], players, player);
            }
            else
            {
                Console.WriteLine("Player {0}, hand {1}: You won {2} chips, you now have {3} chips.",
                player.Name, handIndex + 1, player.HasBlackjack ? player.GetBet(handIndex) * 1.5 : player.GetBet(handIndex) * 1,
                player.Chips);
            }
        }


        protected virtual void Lose(Player player, int handIndex)
        {
            if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
            {
                betterUI.DisplayOutcomes("LOSS", handIndex, player.GetHandValues()[handIndex], players, player);
            }
            else
            {
                Console.WriteLine("Player {0}, hand {1}: You lost, you now have {2} chips.", player.Name, handIndex + 1, player.Chips);
            }
        }


        protected virtual void Push(Player player, int handIndex)
        {
            player.Chips += player.GetBet(handIndex);

            if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
            {
                betterUI.DisplayOutcomes("PUSH", handIndex, player.GetHandValues()[handIndex], players, player);
            }
            else
            {
                Console.WriteLine("Player {0}, hand {1}: Push, {2} chips returned, you now have {3} chips.",
                    player.Name, handIndex + 1, player.GetBet(handIndex), player.Chips);
            }
        }


        protected virtual void WinInsurance(Player player, double insurance)
        {
            player.Chips += insurance * 3;

            if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
            {
                betterUI.DisplayOutcomes("INSURANCE WIN", 0, player.GetHandValues()[0], players, player);
            }
            else
            {
                Console.WriteLine("Player {0}: You get {1} chips from insurance", player.Name, insurance * 2);
            }
        }


        protected bool ExistActivePlayers()
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
