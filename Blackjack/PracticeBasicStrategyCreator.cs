using System;
using System.Collections.Generic;
using System.Text;

namespace Blackjack
{
    //part of a <Practice Factory pattern>
    //STRUKTURU FACTORY PATTERNU JSEM PREVZAL Z https://www.dofactory.com/net/factory-method-design-pattern

    class PracticeBasicStrategyCreator : PracticeCreator
    {
        string[] stringOptions = new string[] {
        "Dealer hits soft 17: ",
        "Allow surrender: ",
        "Allow double: ",
        "--Allow double after split: "
        };

        string[] dealerHitsSoft17 = new string[] { "yes", "no" };
        string[] allowSurrender = new string[] { "yes", "no" };
        string[] allowDouble = new string[] { "yes", "no" };
        string[] allowDAS = new string[] { "yes", "no" };

        public PracticeBasicStrategyCreator(BetterUI betterUI, Random random)
        : base(betterUI, random)
        {
        }


        public override IPlayable CreateGameMode()
        {
            ConsoleKey k;
            int optionSelected = 0;

            /*<before>
            int indexDealerHits = 0;
            int indexSurrender = 0;
            int indexDouble = 0;
            int indexDAS = 0;
            </before>*/

            //<after>
            string[][] displayedOptions = new string[][] { dealerHitsSoft17, allowSurrender, allowDouble, allowDAS };
            int[] indexes = { 0, 0, 0, 0 };
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
                        Console.WriteLine(stringOptions[i] + "<{0}>", dealerHitsSoft17[indexDealerHits]);
                        break;
                    case 1:
                        Console.WriteLine(stringOptions[i] + "<{0}>", allowSurrender[indexSurrender]);
                        break;
                    case 2:
                        Console.WriteLine(stringOptions[i] + "<{0}>", allowDouble[indexDouble]);
                        break;
                    case 3:
                        Console.WriteLine(stringOptions[i] + "<{0}>", allowDAS[Math.Max(indexDouble, indexDAS)]); //if double is not allowed, you cannot double after split.
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
            betterUI.DisplayMenu("Use arrows to customize your practice or press enter to move on", stringOptions, displayedOptions, indexes, null, optionSelected, 3, ConsoleColor.DarkGray, true);
            //</after>

            //reads user's inputs
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
                                indexDealerHits--;
                                indexDealerHits = indexDealerHits < 0 ? indexDealerHits + dealerHitsSoft17.Length : indexDealerHits;
                                break;
                            case 1:
                                indexSurrender--;
                                indexSurrender = indexSurrender < 0 ? indexSurrender + allowSurrender.Length : indexSurrender;
                                break;
                            case 2:
                                indexDouble--;
                                indexDouble = indexDouble < 0 ? indexDouble + allowDouble.Length : indexDouble;
                                break;
                            case 3:
                                indexDAS--;
                                indexDAS = indexDAS < 0 ? indexDAS + allowDAS.Length : indexDAS;
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
                                indexDealerHits++;
                                indexDealerHits %= dealerHitsSoft17.Length;
                                break;
                            case 1:
                                indexSurrender++;
                                indexSurrender %= allowSurrender.Length;
                                break;
                            case 2:
                                indexDouble++;
                                indexDouble %= allowDouble.Length;
                                break;
                            case 3:
                                indexDAS++;
                                indexDAS %= allowDAS.Length;
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
                            Console.WriteLine(stringOptions[i] + "<{0}>", dealerHitsSoft17[indexDealerHits]);
                            break;
                        case 1:
                            Console.WriteLine(stringOptions[i] + "<{0}>", allowSurrender[indexSurrender]);
                            break;
                        case 2:
                            Console.WriteLine(stringOptions[i] + "<{0}>", allowDouble[indexDouble]);
                            break;
                        case 3:
                            Console.WriteLine(stringOptions[i] + "<{0}>", allowDAS[Math.Max(indexDouble, indexDAS)]); //if double is not allowed, you cannot double after split.
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
                indexes = betterUI.GetUserIntArrayInput("Use arrows to customize your practice or press enter to move on", stringOptions, displayedOptions, indexes, null, k, optionSelected, 3, ConsoleColor.DarkGray);

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
        
            return new PracticeBasicStrategy(betterUI, random,
                dealerHitsSoft17[indexes[0]] == "yes",
                allowSurrender[indexes[1]] == "yes",
                allowDouble[indexes[2]] == "yes",
                allowDAS[indexes[3]] == "yes" && allowDouble[indexes[2]] == "yes");
        }
    }
}
