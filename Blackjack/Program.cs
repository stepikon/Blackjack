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
            Console.WriteLine("Press any key to start.");

            Console.ReadKey();

            Random random = new Random();
            BetterUI betterUI = new BetterUI();
            GameMode gm = new GameMode();

            Console.Clear();

            //part of a <Strategy pattern>
            //STRUKTURU STRATEGY PATTERNU JSEM PREVZAL Z https://refactoring.guru/design-patterns/strategy/csharp/example
            
            do
            {           
                gm.SetGamemode(ChooseGamemode(betterUI, random));
                gm.Run();
            } while (!(gm.GetGamemode() is Quit));

            //</Strategy pattern>
        }

        public static IPlayable ChooseGamemode(BetterUI betterUI, Random random)
        {
            //part of a <Factory pattern>
            //STRUKTURU FACTORY PATTERNU JSEM PREVZAL Z https://www.dofactory.com/net/factory-method-design-pattern
            
            GameModeCreator[] creators = new GameModeCreator[] { 
                new GameCreator(betterUI, random, false),
                new PracticeCreator(betterUI, random), 
                new EVSimulationCreator(betterUI, random),
                new RORSimulationCreator(betterUI, random),
                new QuitCreator(betterUI, random)
            };

            string[] options = new string[] { "Game", "Practice", "EV simulation", "Risk of ruin simulation", "Quit"};

            return creators[Array.IndexOf(options, betterUI.GetStringChoiceTopRight("Choose gamemode:", options))].CreateGameMode();
            
            //</Factory pattern>
        }
    }
}
