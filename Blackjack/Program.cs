using System;
using System.Collections.Generic;

namespace Blackjack
{
    class Program
    {
        static void Main(string[] args)
        {
            Dealer dealer = new Dealer("Dealer", new List<Card>(), 20, new Random());
            dealer.BuildShoe();
            dealer.Shuffle();

            Tuple<int, int> tableLimits = new Tuple<int, int>(5,500);

            Player[] players = new Player[1];
            players[0] = new HumanPlayer("human", new List<Card>(), 300, tableLimits);

            Game game = new Game(dealer, tableLimits, players, new Random());

            game.Run();
        }
    }
}
