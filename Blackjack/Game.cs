using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Blackjack
{
    class Game
    {
        Dealer dealer;
        Tuple<int, int> tableLimits;
        Player[] players;
        Random random;

        public Game(Dealer dealer, Tuple<int, int> tableLimits, Player[] players, Random random)
        {
            this.dealer = dealer;
            this.tableLimits = tableLimits;
            this.players = players;
            this.random = random;
        }

        public void Run()
        {
            dealer.SetDeckPenetration();
            bool allPlayersHaveBlackJack;

            //initial check
            foreach (Player p in players)
            {
                p.IsRuined = p.Chips < tableLimits.Item1;
            }

            do
            {
                allPlayersHaveBlackJack = true;

                //Betting
                Console.WriteLine("newRound");
                foreach (Player p in players)
                {
                    if (!(p.IsRuined || p.IsGone))
                    {
                        p.Bet(p.hands[0], tableLimits);

                        if (!p.IsGone)
                        {
                            p.BetPair(tableLimits);
                        }
                    }
                }
                Console.WriteLine("-----");

                //Dealing
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < players.Length; j++)
                    {
                        if (!(players[j].IsRuined || players[j].IsGone))
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

                //Pair bonus gets paid always first
                foreach (Player p in players)
                {
                    if (!(p.IsRuined || p.IsGone))
                    {
                        if (p.PairBet != 0)
                        {
                            if (p.hand[0].GetType() == p.hand[1].GetType()
                                && p.hand[0].Color == p.hand[1].Color
                                && p.hand[0].Suit == p.hand[1].Suit)
                            {
                                Console.WriteLine("Perfect pair.");
                                p.Chips += p.PairBet * 26;
                            }
                            else if (p.hand[0].GetType() == p.hand[1].GetType()
                                && p.hand[0].Color == p.hand[1].Color)
                            {
                                Console.WriteLine("Color pair.");
                                p.Chips += p.PairBet * 13;
                            }
                            else if (p.hand[0].GetType() == p.hand[1].GetType())
                            {
                                Console.WriteLine("Pair.");
                                p.Chips += p.PairBet * 7;
                            }
                            else
                            {
                                Console.WriteLine("Not a pair.");
                            }
                        }
                    }                   
                }

                //checks blackjacks
                foreach (Player p in players)
                {
                    if (!(p.IsRuined || p.IsGone))
                    {
                        p.SetHasBlackjack();
                    }
                }
                dealer.SetHasBlackjack();

                //Displays hands
                foreach (Player p in players)
                {
                    if (!(p.IsRuined || p.IsGone))
                    {
                        p.DisplayHands();
                    }
                }
                Console.WriteLine();
                dealer.DisplayHand();

                Console.WriteLine("-----");

                //offers insurance if dealer shows an Ace
                if (dealer.hand[0] is CardAce)
                {
                    foreach (Player p in players)
                    {
                        if (!(p.IsRuined || p.IsGone))
                        {
                            p.BetInsurance();
                        }
                    }
                }

                Console.WriteLine("-----");

                //players turns
                if (!dealer.HasBlackjack)
                {
                    if (dealer.hand[0] is CardAce)
                    {
                        Console.WriteLine("Nobody home");
                    }

                    foreach (Player p in players)
                    {
                        if (!(p.IsRuined || p.IsGone))
                        {
                            p.TakeTurn(dealer);
                        }
                    }

                    Console.WriteLine("-----");
                }

                //Dealer reveals his cards
                dealer.RevealHidden();
                dealer.DisplayHand();

                Console.WriteLine("-----");

                //checks if everyone has BJ
                foreach (Player p in players)
                {
                    if (!(p.IsRuined || p.IsGone))
                    {
                        allPlayersHaveBlackJack = allPlayersHaveBlackJack && p.HasBlackjack;
                    }
                }

                //Dealers turn
                if (!allPlayersHaveBlackJack)
                {
                    dealer.TakeTurn();
                }
                dealer.DisplayHand();

                Console.WriteLine("----");

                //outcomes
                foreach (Player p in players)
                {
                    if (!(p.IsRuined || p.IsGone))
                    {
                        for (int i = 0; i < p.GetHandValues().Count; i++)
                        {
                            Console.WriteLine("player: {0} ", p.GetHandValues()[i]);
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

                        Console.WriteLine();
                    }                   
                }

                Console.WriteLine(dealer.GetHandValue(dealer.hand).Item2);
                Console.WriteLine("----");

                //Reset
                foreach (Player p in players)
                {
                    p.ResetHands();
                }
                dealer.ResetHand();

                //checks if anyone is ruined
                foreach (Player p in players)
                {
                    if (p.Chips < tableLimits.Item1)
                    {
                        p.IsRuined = true;
                    }
                }

                //Shuffle if necessary
                if (dealer.CardToDeal >= dealer.DeckPenetration)
                {
                    Console.WriteLine("Shuffling" + dealer.CardToDeal + "  " + dealer.DeckPenetration);
                    dealer.Shuffle();
                    dealer.SetDeckPenetration();
                }
            } while (ExistActivePlayers() && Console.ReadKey().KeyChar!='q');
        }

        private void Win(Player player, int handIndex)
        {
            player.Chips += player.HasBlackjack ? player.GetBet(handIndex) * 2.5 : player.GetBet(handIndex) * 2;
            Console.WriteLine("You won {0} chips, you now have {1} chips.",
                player.HasBlackjack ? player.GetBet(handIndex) * 2.5 : player.GetBet(handIndex) * 2,
                player.Chips);
        }

        private void Lose(Player player, int handIndex)
        {
            if (dealer.HasBlackjack)
            {
                Console.WriteLine("DEBUG: Dealer has blackjack");
            }

            Console.WriteLine("You lost, you now have {0} chips.", player.Chips);
        }

        private void Push(Player player, int handIndex)
        {
            player.Chips += player.GetBet(handIndex);
            Console.WriteLine("Push, {0} chips returned, you now have {1} chips.", player.GetBet(handIndex), player.Chips);
        }

        private void WinInsurance(Player player, double insurance)
        {
            Console.WriteLine("You get {0} chips from insurance", insurance * 3);
            player.Chips += insurance * 3;
        }

        private bool ExistActivePlayers()
        {
            bool existActivePlayers = false;

            foreach (Player p in players)
            {
                existActivePlayers = existActivePlayers || !(p.IsRuined || p.IsGone);
            }

            return existActivePlayers;
        }
    }
}
