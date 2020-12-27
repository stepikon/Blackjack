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

            BetterUI betterUI = new BetterUI();
            Dealer dealer = new Dealer("Dealer", new List<Card>(), betterUI, 6, new Random(), true);
            dealer.BuildShoe();
            dealer.Shuffle();

            Tuple<int, int> tableLimits = new Tuple<int, int>(5,500);

            Player[] players = new Player[7];
            players[2] = new CardCountingAI("CC AI1", new List<Card>(), betterUI, 2000, tableLimits, 10);
            players[3] = new CardCountingAI("CC AI2", new List<Card>(), betterUI, 2000, tableLimits, 10);

            Game game = new Game(dealer, tableLimits, players, new Random(), new BetterUI());

            Console.WriteLine("Press any key to start.");
            Console.ReadKey();
            Console.Clear();

            game.Run();
        }
    }
}
