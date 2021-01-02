using System;
using System.Collections.Generic;
using System.Text;

namespace Blackjack
{
    class SimulationCreator : GameModeCreator
    {
        string[] stringOptions = new string[] {
        "Number of decks: ",
        "Dealer hits soft 17: ",
        "Allow surrender: ",
        "Allow double after split: ",
        "Allow resplit: ",
        "Number of AIs: ",
        "Make dealer and AIs visible: "};

        int[] numberOfDecks = new int[] { 4, 5, 6, 7, 8 };
        string[] dealerHitsSoft17 = new string[] { "yes", "no" };
        string[] allowSurrender = new string[] { "yes", "no" };
        string[] allowDAS = new string[] { "yes", "no" };
        string[] allowResplit = new string[] { "yes", "no" };
        int[] numberOfAIs = new int[] { 1, 2, 3, 4, 5, 6, 7 };
        string[] isVisible = new string[] { "yes", "no" };

        public SimulationCreator(BetterUI betterUI, Random random)
            : base(betterUI, random)
        { }

        public override IPlayable CreateGameMode()
        {
            ConsoleKey k;
            int optionSelected = 0;
            int indexNumberOfDecks = 0;
            int indexDealerHits = 0;
            int indexSurrender = 0;
            int indexDAS = 0;
            int indexResplit = 0;
            int indexNumberOfAIs = 0;
            int indexIsVisible = 0;
            int tableMin;
            int tableMax;
            int handsPerCycle;
            int repetitions;
            int betSpreadMultiplier;

            //initial display
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            betterUI.ClearAll();
            Console.WriteLine("Use arrows to set up your simulation or press enter to move on");
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
                        Console.WriteLine(stringOptions[i] + "<{0}>", numberOfDecks[indexNumberOfDecks]);
                        break;
                    case 1:
                        Console.WriteLine(stringOptions[i] + "<{0}>", dealerHitsSoft17[indexDealerHits]);
                        break;
                    case 2:
                        Console.WriteLine(stringOptions[i] + "<{0}>", allowSurrender[indexSurrender]);
                        break;
                    case 3:
                        Console.WriteLine(stringOptions[i] + "<{0}>", allowDAS[indexDAS]);
                        break;
                    case 4:
                        Console.WriteLine(stringOptions[i] + "<{0}>", allowResplit[indexResplit]);
                        break;
                    case 5:
                        Console.WriteLine(stringOptions[i] + "<{0}>", numberOfAIs[indexNumberOfAIs]);
                        break;
                    case 6:
                        Console.WriteLine(stringOptions[i] + "<{0}>", isVisible[indexIsVisible]);
                        break;
                    default:
                        Console.WriteLine(stringOptions[i]);
                        break;
                }
            }
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(new String('=', 10));
            Console.WriteLine("Table minimum: ");
            Console.WriteLine();
            Console.WriteLine("Table maximum: ");
            Console.WriteLine();
            Console.WriteLine("Hands per cycle (# hands played during 1 repetition): ");
            Console.WriteLine();
            Console.WriteLine("Repetitions: ");
            Console.WriteLine();
            Console.WriteLine("Bet spread multiplier (widens the bet spread): ");
            Console.WriteLine();
            Console.WriteLine(new String('=', 10));

            do
            {
                k = Console.ReadKey(true).Key;
                switch (k)
                {
                    case ConsoleKey.LeftArrow:
                        switch (optionSelected)
                        {
                            case 0:
                                indexNumberOfDecks--;
                                indexNumberOfDecks = indexNumberOfDecks < 0 ? indexNumberOfDecks + numberOfDecks.Length : indexNumberOfDecks;
                                break;
                            case 1:
                                indexDealerHits--;
                                indexDealerHits = indexDealerHits < 0 ? indexDealerHits + dealerHitsSoft17.Length : indexDealerHits;
                                break;
                            case 2:
                                indexSurrender--;
                                indexSurrender = indexSurrender < 0 ? indexSurrender + allowSurrender.Length : indexSurrender;
                                break;
                            case 3:
                                indexDAS--;
                                indexDAS = indexDAS < 0 ? indexDAS + allowDAS.Length : indexDAS;
                                break;
                            case 4:
                                indexResplit--;
                                indexResplit = indexResplit < 0 ? indexResplit + allowResplit.Length : indexResplit;
                                break;
                            case 5:
                                indexNumberOfAIs--;
                                indexNumberOfAIs = indexNumberOfAIs < 0 ? indexNumberOfAIs + numberOfAIs.Length : indexNumberOfAIs;
                                break;
                            case 6:
                                indexIsVisible--;
                                indexIsVisible = indexIsVisible < 0 ? indexIsVisible + isVisible.Length : indexIsVisible;
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
                                indexNumberOfDecks++;
                                indexNumberOfDecks %= numberOfDecks.Length;
                                break;
                            case 1:
                                indexDealerHits++;
                                indexDealerHits %= dealerHitsSoft17.Length;
                                break;
                            case 2:
                                indexSurrender++;
                                indexSurrender %= dealerHitsSoft17.Length;
                                break;
                            case 3:
                                indexDAS++;
                                indexDAS %= dealerHitsSoft17.Length;
                                break;
                            case 4:
                                indexResplit++;
                                indexResplit %= dealerHitsSoft17.Length;
                                break;
                            case 5:
                                indexNumberOfAIs++;
                                indexNumberOfAIs %= numberOfAIs.Length;
                                break;
                            case 6:
                                indexIsVisible++;
                                indexIsVisible %= isVisible.Length;
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
                            Console.WriteLine(stringOptions[i] + "<{0}>", numberOfDecks[indexNumberOfDecks]);
                            break;
                        case 1:
                            Console.WriteLine(stringOptions[i] + "<{0}>", dealerHitsSoft17[indexDealerHits]);
                            break;
                        case 2:
                            Console.WriteLine(stringOptions[i] + "<{0}>", allowSurrender[indexSurrender]);
                            break;
                        case 3:
                            Console.WriteLine(stringOptions[i] + "<{0}>", allowDAS[indexDAS]);
                            break;
                        case 4:
                            Console.WriteLine(stringOptions[i] + "<{0}>", allowResplit[indexResplit]);
                            break;
                        case 5:
                            Console.WriteLine(stringOptions[i] + "<{0}>", numberOfAIs[indexNumberOfAIs]);
                            break;
                        case 6:
                            Console.WriteLine(stringOptions[i] + "<{0}>", isVisible[indexIsVisible]);
                            break;
                        default:
                            Console.WriteLine(stringOptions[i]);
                            break;
                    }
                }
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(new String('=', 10));
                Console.WriteLine("Table minimum: ");
                Console.WriteLine();
                Console.WriteLine("Table maximum: ");
                Console.WriteLine();
                Console.WriteLine("Hands per cycle (# hands played during 1 repetition): ");
                Console.WriteLine();
                Console.WriteLine("Repetitions: ");
                Console.WriteLine();
                Console.WriteLine("Bet spread multiplier (widens the bet spread): ");
                Console.WriteLine();
                Console.WriteLine(new String('=', 10));
            } while (k != ConsoleKey.Enter);

            //Final Display
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            betterUI.ClearAll();
            Console.WriteLine("Use arrows to customize your game or press enter to move on");
            for (int i = 0; i < stringOptions.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        Console.WriteLine(stringOptions[i] + "<{0}>", numberOfDecks[indexNumberOfDecks]);
                        break;
                    case 1:
                        Console.WriteLine(stringOptions[i] + "<{0}>", dealerHitsSoft17[indexDealerHits]);
                        break;
                    case 2:
                        Console.WriteLine(stringOptions[i] + "<{0}>", allowSurrender[indexSurrender]);
                        break;
                    case 3:
                        Console.WriteLine(stringOptions[i] + "<{0}>", allowDAS[indexDAS]);
                        break;
                    case 4:
                        Console.WriteLine(stringOptions[i] + "<{0}>", allowResplit[indexResplit]);
                        break;
                    case 5:
                        Console.WriteLine(stringOptions[i] + "<{0}>", numberOfAIs[indexNumberOfAIs]);
                        break;
                    case 6:
                        Console.WriteLine(stringOptions[i] + "<{0}>", isVisible[indexIsVisible]);
                        break;
                    default:
                        Console.WriteLine(stringOptions[i]);
                        break;
                }
            }
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(new String('=', 10));
            Console.WriteLine("Table minimum: ");
            Console.WriteLine();
            Console.WriteLine("Table maximum: ");
            Console.WriteLine();
            Console.WriteLine("Hands per cycle (# hands played during 1 repetition): ");
            Console.WriteLine();
            Console.WriteLine("Repetitions: ");
            Console.WriteLine();
            Console.WriteLine("Bet spread multiplier (widens the bet spread): ");
            Console.WriteLine();
            Console.WriteLine(new String('=', 10));

            //table minimum
            do
            {
                Console.SetCursorPosition(0, 10);
                Console.Write(new String(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, 10);
            } while (!(int.TryParse(Console.ReadLine(), out tableMin) && tableMin > 0));

            //table maximum
            do
            {
                Console.SetCursorPosition(0, 12);
                Console.Write(new String(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, 12);
            } while (!(int.TryParse(Console.ReadLine(), out tableMax) && tableMax >= tableMin));

            //hands per cycle
            do
            {
                Console.SetCursorPosition(0, 14);
                Console.Write(new String(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, 14);
            } while (!(int.TryParse(Console.ReadLine(), out handsPerCycle) && handsPerCycle > 0));

            //repetitions
            do
            {
                Console.SetCursorPosition(0, 16);
                Console.Write(new String(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, 16);
            } while (!(int.TryParse(Console.ReadLine(), out repetitions) && repetitions > 0));

            //bet spread multiplier
            do
            {
                Console.SetCursorPosition(0, 18);
                Console.Write(new String(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, 18);
            } while (!(int.TryParse(Console.ReadLine(), out betSpreadMultiplier) && betSpreadMultiplier > 0));

            //players and AIs construction
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            Player[] players = new Player[7];
            string[] names = new string[7];
            int[] chips = new int[7];
            Tuple<int, int> tableLimits = new Tuple<int, int>(tableMin, tableMax);

            for (int i = 0; i < numberOfAIs[indexNumberOfAIs]; i++)
            {
                do
                {
                    Console.WriteLine("Enter AI{0}'s name (1-25 characters)", i + 1);
                    names[i] = Console.ReadLine();
                } while (names[i].Length > 25 || names[i].Length < 1);

                do
                {
                    Console.WriteLine("Enter player AI{0}'s chips", i + 1);
                } while (!int.TryParse(Console.ReadLine(), out chips[i]));

                betterUI.ClearAll();
            }

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;

            return new Simulation(
                new Dealer("Dealer", new List<Card>(), betterUI, numberOfDecks[indexNumberOfDecks], random, dealerHitsSoft17[indexDealerHits] == "yes", false, isVisible[indexIsVisible] == "yes"),
                tableLimits,
                players,
                random,
                betterUI,
                numberOfAIs[indexNumberOfAIs], handsPerCycle, repetitions, new List<double>[numberOfAIs[indexNumberOfAIs]], isVisible[indexIsVisible] == "yes",
                names, chips, allowSurrender[indexSurrender] == "yes", allowDAS[indexDAS] == "yes",
                allowResplit[indexResplit] == "yes", betSpreadMultiplier, false
                );
        }
    }
}
