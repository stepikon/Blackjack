﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Blackjack
{
    class Game
    {
        //to display everything in a more fancy way
        private const int MINIMUM_WINDIW_WIDTH = 7 * 25;
        private const int MINIMUM_WINDIW_HEIGHT = 36;

        Dealer dealer;
        Tuple<int, int> tableLimits;
        Player[] players;
        BetterUI betterUI;
        Random random;

        public Game(Dealer dealer, Tuple<int, int> tableLimits, Player[] players, Random random, BetterUI betterUI)
        {
            this.dealer = dealer;
            this.tableLimits = tableLimits;
            this.players = players;
            this.random = random;
            this.betterUI = betterUI;
        }

        public void Run()
        {
            dealer.Reset();
            bool allPlayersHaveBlackJack;
            bool allBusted;

            //initial check
            foreach (Player p in players)
            {
                if (p!=null)
                {
                    p.IsRuined = p.Chips < tableLimits.Item1;
                }
            }

            do
            {
                allPlayersHaveBlackJack = true;
                allBusted = true;
                betterUI.ClearAll();

                if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                {
                    betterUI.DisplayTableRules(dealer.HitSoft17);
                    betterUI.DisplayLimits(tableLimits);
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
                                    betterUI.DisplayMessage("Perfect pair.");
                                }
                                else
                                {
                                    Console.WriteLine("Perfect pair.");
                                }
                                p.Chips += p.PairBet * 26;
                            }
                            else if (p.hand[0].GetType() == p.hand[1].GetType()
                                && p.hand[0].Color == p.hand[1].Color)
                            {
                                if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                                {
                                    betterUI.DisplayMessage("Color pair.");
                                }
                                else
                                {
                                    Console.WriteLine("Color pair.");
                                }
                                p.Chips += p.PairBet * 13;
                            }
                            else if (p.hand[0].GetType() == p.hand[1].GetType())
                            {
                                if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                                {
                                    betterUI.DisplayMessage("Pair.");
                                }
                                else
                                {
                                    Console.WriteLine("Pair.");
                                }
                                p.Chips += p.PairBet * 7;
                            }
                            else
                            {
                                if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                                {
                                    betterUI.DisplayMessage("Not a pair.");
                                }
                                else
                                {
                                    Console.WriteLine("Not a pair.");
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
                            p.CountDealt(players,dealer.hand, dealer.DeckAmount - dealer.GetDecksInDiscard());
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

                //checks if everyone has BJ
                foreach (Player p in players)
                {
                    if (!(p == null || p.IsRuined || p.IsGone))
                    {
                        allPlayersHaveBlackJack = allPlayersHaveBlackJack && p.HasBlackjack;
                    }
                }

                //checks if everyone has busted
                foreach (Player p in players)
                {
                    if (!(p == null || p.IsRuined || p.IsGone))
                    {
                        foreach (int handValue in p.GetHandValues())
                        {
                            allBusted = allBusted && handValue > 21;
                        }
                    }
                }

                //Dealers turn
                if (!(allPlayersHaveBlackJack||allBusted))
                {
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
                            if ((p.GetHandValues()[i] > dealer.GetHandValue(dealer.hand).Item2
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
                    if (p!=null)
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
                    if (p!=null)
                    {
                        p.ResetHands();
                    }
                }
                dealer.ResetHand();

                //checks if anyone is ruined
                foreach (Player p in players)
                {
                    if (p!=null)
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
                        if (p!=null)
                        {
                            p.ResetCounts();
                        }
                    }
                }

                if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT || Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.SetCursorPosition(0,38);
                    Console.WriteLine("Press any key to continue. Press q to quit");
                }
                else
                {
                    Console.WriteLine("Press any key to continue. Press q to quit");
                }

            } while (ExistActivePlayers()&&Console.ReadKey().KeyChar!='q');
        }

        private void Win(Player player, int handIndex)
        {
            player.Chips += player.HasBlackjack ? player.GetBet(handIndex) * 2.5 : player.GetBet(handIndex) * 2;

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

        private void Lose(Player player, int handIndex)
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

        private void Push(Player player, int handIndex)
        {
            player.Chips += player.GetBet(handIndex);

            if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
            {
                betterUI.DisplayOutcomes("PUSH", handIndex, player.GetHandValues()[handIndex], players, player);
            }
            else
            {
                Console.WriteLine("Push, {0} chips returned, you now have {1} chips.", player.GetBet(handIndex), player.Chips);
            }        
        }

        private void WinInsurance(Player player, double insurance)
        {            
            player.Chips += insurance * 3;

            if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
            {
                betterUI.DisplayOutcomes("INSURANCE WIN", 0, player.GetHandValues()[0], players, player);
            }
            else
            {
                Console.WriteLine("You get {0} chips from insurance", insurance * 3);
            }
        }

        private bool ExistActivePlayers()
        {
            bool existActivePlayers = false;

            foreach (Player p in players)
            {
                if (p!=null)
                {
                    existActivePlayers = existActivePlayers || !(p.IsRuined || p.IsGone);
                }
            }

            return existActivePlayers;
        }
    }
}
