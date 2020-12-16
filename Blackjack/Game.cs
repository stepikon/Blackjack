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
            for (int i = 0; i < 200; i++)
            {
                dealer.Deal(players[0]);

                players[0].hand.Display();
            }
            
        }
    }
}
