using System;
using System.Collections.Generic;
using System.Text;

namespace Blackjack
{
    //part of a <Practice Factory pattern>
    //STRUKTURU FACTORY PATTERNU JSEM PREVZAL Z https://www.dofactory.com/net/factory-method-design-pattern

    class PracticeTrueCountConversionCreator : PracticeCreator
    {
        public PracticeTrueCountConversionCreator(BetterUI betterUI, Random random)
            : base(betterUI, random)
        {
        }


        public override IPlayable CreateGameMode()
        {
            string[] stringOptions = new string[] {
                "True counts are only integers: "
            };

            string[] onlyIntegers = new string[] { "yes", "no" };

            ConsoleKey k;
            int optionSelected = 0;
            int indexOnlyIntegers = 0;

            //initial display
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            betterUI.ClearAll();
            Console.WriteLine("Use arrows to customize your game or press enter to move on");
            for (int i = 0; i < stringOptions.Length; i++)
            {
                if (i == optionSelected)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                }

                switch (i)
                {
                    case 0:
                        Console.WriteLine(stringOptions[i] + "<{0}>", onlyIntegers[indexOnlyIntegers]);
                        break;
                    default:
                        Console.WriteLine(stringOptions[i]);
                        break;
                }
            }

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;

            //reads user's input
            do
            {
                k = Console.ReadKey(true).Key;
                switch (k)
                {
                    case ConsoleKey.LeftArrow:
                        switch (optionSelected)
                        {
                            case 0:
                                indexOnlyIntegers--;
                                indexOnlyIntegers = indexOnlyIntegers < 0 ? indexOnlyIntegers + onlyIntegers.Length : indexOnlyIntegers;
                                break;
                            default:
                                break;
                        }
                        break;
                    case ConsoleKey.UpArrow:
                        optionSelected--;
                        optionSelected = optionSelected < 0 ? optionSelected += stringOptions.Length : optionSelected;
                        break;
                    case ConsoleKey.RightArrow:
                        switch (optionSelected)
                        {
                            case 0:
                                indexOnlyIntegers++;
                                indexOnlyIntegers %= onlyIntegers.Length;
                                break;
                            default:
                                break;
                        }
                        break;
                    case ConsoleKey.DownArrow:
                        optionSelected++;
                        optionSelected %= stringOptions.Length;
                        break;
                    default:
                        break;
                }

                //Display
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
                betterUI.ClearAll();
                Console.WriteLine("Use arrows to customize your game or press enter to move on");
                for (int i = 0; i < stringOptions.Length; i++)
                {
                    if (i == optionSelected)
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    switch (i)
                    {
                        case 0:
                            Console.WriteLine(stringOptions[i] + "<{0}>", onlyIntegers[indexOnlyIntegers]);
                            break;
                        default:
                            Console.WriteLine(stringOptions[i]);
                            break;
                    }
                }
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
            } while (k != ConsoleKey.Enter);

            return new PracticeTrueCountConversion(betterUI, random, onlyIntegers[indexOnlyIntegers] == "yes");
        }
    }

    //</Practice Factory pattern>
}
