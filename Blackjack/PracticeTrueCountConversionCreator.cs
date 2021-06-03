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

            /*<before>
            int indexOnlyIntegers = 0;
            </before>*/

            //<after>
            string[][] displayedOptions = new string[][] { onlyIntegers };
            int[] indexes = { 0 };
            //</after>

            //initial display
            /*<before>
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
            </before>*/
            //<after>
            betterUI.DisplayMenu("Use arrows to customize your practice or press enter to move on", stringOptions, displayedOptions, indexes, null, optionSelected, -1, ConsoleColor.DarkGray, true);
            //</after>

            //reads user's input
            do
            {
                k = Console.ReadKey(true).Key;

                /*<before>
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
                </before>*/
                //<after>
                indexes = betterUI.GetUserIntArrayInput("Use arrows to customize your practice or press enter to move on", stringOptions, displayedOptions, indexes, null, k, optionSelected, -1, ConsoleColor.DarkGray);

                switch (k)
                {
                    case ConsoleKey.UpArrow:
                        optionSelected--;
                        optionSelected = optionSelected < 0 ? optionSelected += stringOptions.Length : optionSelected;
                        break;
                    case ConsoleKey.DownArrow:
                        optionSelected++;
                        optionSelected %= stringOptions.Length;
                        break;
                    default:
                        break;
                }
                //</after>
            } while (k != ConsoleKey.Enter);

            return new PracticeTrueCountConversion(betterUI, random, onlyIntegers[indexes[0]] == "yes");
        }
    }

    //</Practice Factory pattern>
}
