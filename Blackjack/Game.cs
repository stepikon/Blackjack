using System;
using System.Collections.Generic;
using System.Text;

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
            //Deal
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < players.Length; j++)
                {
                    dealer.Deal(players[j]);
                }

                if (i==0)
                {
                    dealer.Deal(dealer);
                }
                else
                {
                    dealer.DealHidden();
                }
            }

            foreach (Player p in players)
            {
                p.hand.Display();
            }
            Console.WriteLine();
            dealer.hand.Display();

            //getChoices...

            Console.WriteLine();
            dealer.RevealHidden();
            dealer.hand.Display();

            //Dealers turn...

            //outcomes
        }
    }
}
