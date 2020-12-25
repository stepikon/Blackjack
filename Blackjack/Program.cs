using System;
using System.Collections.Generic;

namespace Blackjack
{
    class Program
    {
        static void Main(string[] args)
        {
            Dealer dealer = new Dealer("Dealer", new List<Card>(), 6, new Random(), true);
            dealer.BuildShoe();
            dealer.Shuffle();

            Tuple<int, int> tableLimits = new Tuple<int, int>(5,500);

            Player[] players = new Player[2];
            players[0] = new CardCountingAI("CC AI", new List<Card>(), 300, tableLimits, 10);

            Game game = new Game(dealer, tableLimits, players, new Random());

            game.Run();
        }
    }
}
