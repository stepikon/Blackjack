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
            Console.WriteLine("Press any key to start. Press q to quit");

            char quit;
            quit = Console.ReadKey().KeyChar;
            if (quit == 'q' || quit == 'Q')
            {
                return;
            }

            Random random = new Random();
            BetterUI betterUI = new BetterUI();
            GameMode gm = new GameMode();

            Console.Clear();

            do
            {
                //Strategy pattern https://refactoring.guru/design-patterns/strategy/csharp/example
                gm.SetGamemode(ChooseGamemode(betterUI, random));
                gm.Run();

                Console.WriteLine("Game over.\n" +
                    "Press any key to play a new game. Press q to quit");
                quit = Console.ReadKey().KeyChar;
            } while (quit != 'q' && quit != 'Q');
        }

        public static IPlayable ChooseGamemode(BetterUI betterUI, Random random)
        {
            //Factory pattern https://www.dofactory.com/net/factory-method-design-pattern
            GameModeCreator[] creators = new GameModeCreator[] { 
                new GameCreator(betterUI, random, false),
                new PracticeCreator(betterUI, random), 
                new EVSimulationCreator(betterUI, random),
                new RORSimulationCreator(betterUI, random)
            };

            string[] options = new string[] { "game", "practice", "EV simulation", "Risk of ruin simulation"};

            return creators[Array.IndexOf(options, betterUI.GetStringChoiceTopRight("Choose gamemode:", options))].CreateGameMode();
        }
    }
}
