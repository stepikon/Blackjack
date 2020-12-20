using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Blackjack
{
    class Game
    {
        Dealer dealer;
        Player[] players;
        Random random;

        public Game(Dealer dealer, Player[] players, Random random)
        {
            this.dealer = dealer;
            this.players = players;
            this.random = random;
        }

        public void Run()
        {
            do
            {
                Console.WriteLine("newRound");
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
                    p.DisplayHands();
                }
                Console.WriteLine();
                dealer.DisplayHand();

                Console.WriteLine("-----");

                foreach (Player p in players)
                {
                    p.TakeTurn(dealer);
                }

                Console.WriteLine();
                dealer.RevealHidden();
                dealer.DisplayHand();

                Console.WriteLine("-----");

                dealer.TakeTurn();
                dealer.DisplayHand();

                Console.WriteLine("----");

                foreach (Player p in players)
                {
                    foreach (int i in p.GetHandValues())
                    {
                        Console.Write("player: {0} ", i);
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
    }
}
