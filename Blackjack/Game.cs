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
            bool allPlayersHaveBlackJack;
            do
            {
                allPlayersHaveBlackJack = true;

                Console.WriteLine("newRound");
                foreach (Player p in players)
                {
                    p.Bet(p.hands[0], tableLimits);
                }
                Console.WriteLine("-----");
                //Deal
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < players.Length; j++)
                    {
                        dealer.Deal(players[j], 0);
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

                foreach (Player p in players)
                {
                    p.SetHasBlackjack();
                }
                dealer.SetHasBlackjack();

                foreach (Player p in players)
                {
                    p.DisplayHands();
                }
                Console.WriteLine();
                dealer.DisplayHand();

                Console.WriteLine("-----");

                if (dealer.hand[0] is CardAce)
                {
                    foreach (Player p in players)
                    {
                        p.BetInsurance();
                    }
                }

                Console.WriteLine("-----");

                if (!dealer.HasBlackjack)
                {
                    if (dealer.hand[0] is CardAce)
                    {
                        Console.WriteLine("Nobody home");
                    }

                    foreach (Player p in players)
                    {
                        p.TakeTurn(dealer);
                    }

                    Console.WriteLine("-----");
                }

                Console.WriteLine();
                dealer.RevealHidden();
                dealer.DisplayHand();

                Console.WriteLine("-----");

                foreach (Player p in players)
                {
                    allPlayersHaveBlackJack = allPlayersHaveBlackJack && p.HasBlackjack;
                }

                if (!allPlayersHaveBlackJack)
                {
                    dealer.TakeTurn();
                }
                dealer.DisplayHand();

                Console.WriteLine("----");

                foreach (Player p in players)
                {
                    for(int i = 0; i < p.GetHandValues().Count; i++)
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

                Console.WriteLine(dealer.GetHandValue(dealer.hand).Item2);
                Console.WriteLine("----");
                //outcomes

                foreach (Player p in players)
                {
                    p.ResetHands();
                }

                dealer.ResetHand();
            } while (Console.ReadKey().KeyChar!='q');
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
    }
}
