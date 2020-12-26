using System;
using System.Collections.Generic;

namespace Blackjack
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
            Console.SetWindowPosition(0,0);
            Console.WriteLine("Pro tip: if you can't see the whole console, set it to full screen\n" +
                "or right/click -> Properties -> Layout, uncheck the \"let system position the window\" box and set both values to 0");

            Dealer dealer = new Dealer("Dealer", new List<Card>(), 6, new Random(), true);
            dealer.BuildShoe();
            dealer.Shuffle();

            Tuple<int, int> tableLimits = new Tuple<int, int>(5,500);

            Player[] players = new Player[2];
            players[0] = new CardCountingAI("CC AI", new List<Card>(), 2000, tableLimits, 5);

            Game game = new Game(dealer, tableLimits, players, new Random());

            Console.WriteLine("Press any key to start.");
            Console.ReadKey();

            game.Run();
        }
    }
}
