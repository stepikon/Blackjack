using System;
using System.Collections.Generic;
using System.Text;

namespace Blackjack
{
    //part of a <Factory pattern>
    //STRUKTURU FACTORY PATTERNU JSEM PREVZAL Z https://www.dofactory.com/net/factory-method-design-pattern

    class EVSimulationCreator : GameModeCreator
    {
        string[] stringOptions = new string[] {
        "Number of decks: ",
        "Dealer hits soft 17: ",
        "Allow surrender: ",
        "Allow double after split: ",
        "Allow resplit: ",
        "--Allow resplit aces: ",
        "Number of AIs: ",
        "Make dealer and AIs visible: ",
        "AI leaves when true count is -1 or lower: "};

        int[] numberOfDecks = new int[] { 4, 5, 6, 7, 8 };
        string[] dealerHitsSoft17 = new string[] { "yes", "no" };
        string[] allowSurrender = new string[] { "yes", "no" };
        string[] allowDAS = new string[] { "yes", "no" };
        string[] allowResplit = new string[] { "yes", "no" };
        string[] allowResplitAces = new string[] { "yes", "no" };
        int[] numberOfAIs = new int[] { 1, 2, 3, 4, 5, 6, 7 };
        string[] isVisible = new string[] { "yes", "no" };
        string[] AILeaves = new string[] { "yes", "no" };

        public EVSimulationCreator(BetterUI betterUI, Random random)
            : base(betterUI, random)
        {
        }


        public override IPlayable CreateGameMode()
        {
            ConsoleKey k;
            int optionSelected = 0;
            int indexNumberOfDecks = 0;
            int indexDealerHits = 0;
            int indexSurrender = 0;
            int indexDAS = 0;
            int indexResplit = 0;
            int indexResplitAces = 0;
            int indexNumberOfAIs = 0;
            int indexIsVisible = 0;
            int indexAILeaves = 0;
            int tableMin;
            int tableMax;
            int handsPerCycle;
            int repetitions;

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
                        Console.WriteLine(stringOptions[i] + "<{0}>", allowResplitAces[Math.Max(indexResplit, indexResplitAces)]); //resplit must be allowed in order to resplit aces
                        break;
                    case 6:
                        Console.WriteLine(stringOptions[i] + "<{0}>", numberOfAIs[indexNumberOfAIs]);
                        break;
                    case 7:
                        Console.WriteLine(stringOptions[i] + "<{0}>", isVisible[indexIsVisible]);
                        break;
                    default:
                        Console.WriteLine(stringOptions[8] + "<{0}>", AILeaves[indexAILeaves]);
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
            Console.WriteLine("Hands per cycle (# hands played per 1 repetition): ");
            Console.WriteLine();
            Console.WriteLine("Repetitions: ");
            Console.WriteLine();
            Console.WriteLine(new String('=', 10));
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;

            //reads user's inputs
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
                                indexResplitAces--;
                                indexResplitAces = indexResplitAces < 0 ? indexResplitAces + allowResplitAces.Length : indexResplitAces;
                                break;
                            case 6:
                                indexNumberOfAIs--;
                                indexNumberOfAIs = indexNumberOfAIs < 0 ? indexNumberOfAIs + numberOfAIs.Length : indexNumberOfAIs;
                                break;
                            case 7:
                                indexIsVisible--;
                                indexIsVisible = indexIsVisible < 0 ? indexIsVisible + isVisible.Length : indexIsVisible;
                                break;
                            default:
                                indexAILeaves--;
                                indexAILeaves = indexAILeaves < 0 ? indexAILeaves + AILeaves.Length : indexAILeaves;
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
                                indexSurrender %= allowSurrender.Length;
                                break;
                            case 3:
                                indexDAS++;
                                indexDAS %= allowDAS.Length;
                                break;
                            case 4:
                                indexResplit++;
                                indexResplit %= allowResplit.Length;
                                break;
                            case 5:
                                indexResplitAces++;
                                indexResplitAces %= allowResplitAces.Length;
                                break;
                            case 6:
                                indexNumberOfAIs++;
                                indexNumberOfAIs %= numberOfAIs.Length;
                                break;
                            case 7:
                                indexIsVisible++;
                                indexIsVisible %= isVisible.Length;
                                break;
                            default:
                                indexAILeaves++;
                                indexAILeaves %= AILeaves.Length;
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
                            Console.WriteLine(stringOptions[i] + "<{0}>", allowResplitAces[Math.Max(indexResplit, indexResplitAces)]);
                            break;
                        case 6:
                            Console.WriteLine(stringOptions[i] + "<{0}>", numberOfAIs[indexNumberOfAIs]);
                            break;
                        case 7:
                            Console.WriteLine(stringOptions[i] + "<{0}>", isVisible[indexIsVisible]);
                            break;
                        default:
                            Console.WriteLine(stringOptions[8] + "<{0}>", AILeaves[indexAILeaves]);
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
                Console.WriteLine("Hands per cycle (# hands played per 1 repetition): ");
                Console.WriteLine();
                Console.WriteLine("Repetitions: ");
                Console.WriteLine();
                Console.WriteLine(new String('=', 10));
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
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
                        Console.WriteLine(stringOptions[i] + "<{0}>", allowResplitAces[Math.Max(indexResplit, indexResplitAces)]);
                        break;
                    case 6:
                        Console.WriteLine(stringOptions[i] + "<{0}>", numberOfAIs[indexNumberOfAIs]);
                        break;
                    case 7:
                        Console.WriteLine(stringOptions[i] + "<{0}>", isVisible[indexIsVisible]);
                        break;
                    default:
                        Console.WriteLine(stringOptions[8] + "<{0}>", AILeaves[indexAILeaves]);
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
            Console.WriteLine("Hands per cycle (# hands played per 1 repetition): ");
            Console.WriteLine();
            Console.WriteLine("Repetitions: ");
            Console.WriteLine();
            Console.WriteLine(new String('=', 10));
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;

            //table minimum
            do
            {
                Console.SetCursorPosition(0, 12);
                Console.Write(new String(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, 12);
            } while (!(int.TryParse(Console.ReadLine(), out tableMin) && tableMin > 0));

            //table maximum
            do
            {
                Console.SetCursorPosition(0, 14);
                Console.Write(new String(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, 14);
            } while (!(int.TryParse(Console.ReadLine(), out tableMax) && tableMax >= tableMin));

            //hands per cycle
            do
            {
                Console.SetCursorPosition(0, 16);
                Console.Write(new String(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, 16);
            } while (!(int.TryParse(Console.ReadLine(), out handsPerCycle) && handsPerCycle > 0));

            //repetitions
            do
            {
                Console.SetCursorPosition(0, 18);
                Console.Write(new String(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, 18);
            } while (!(int.TryParse(Console.ReadLine(), out repetitions) && repetitions > 0));

            //players and AIs construction
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            Player[] players = new Player[7];
            string[] names = new string[7];
            int[] chips = new int[7];
            int[] betUnits = new int[7];
            int[] betSpreadMultipliers = new int[7];
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
                    Console.WriteLine("Enter AI{0}'s chips", i + 1);
                } while (!(int.TryParse(Console.ReadLine(), out chips[i]) && chips[i] >= 0));

                do
                {
                    Console.WriteLine("Enter AI{0}'s bet unit. Value must fall within the table limits.", i + 1);
                } while (!(int.TryParse(Console.ReadLine(), out betUnits[i]) && betUnits[i] >= tableLimits.Item1 && betUnits[i] <= tableLimits.Item2));

                do
                {
                    Console.WriteLine("Enter AI{0}'s bet spread multiplier \n" +
                        "(bet spread will be 6*multiplier). Expecting integer from <1;5>", i + 1);
                } while (!(int.TryParse(Console.ReadLine(), out betSpreadMultipliers[i]) && betSpreadMultipliers[i] >= 1 && betSpreadMultipliers[i] <= 5));

                betterUI.ClearAll();
            }

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;

            return new EVSimulation(
                new Dealer("Dealer", new List<Card>(), betterUI, numberOfDecks[indexNumberOfDecks], random, dealerHitsSoft17[indexDealerHits] == "yes", false, isVisible[indexIsVisible] == "yes"),
                tableLimits,
                players,
                random,
                betterUI,
                numberOfAIs[indexNumberOfAIs], handsPerCycle, repetitions, new List<double>[numberOfAIs[indexNumberOfAIs]], isVisible[indexIsVisible] == "yes", AILeaves[indexAILeaves] == "yes",
                names, chips, allowSurrender[indexSurrender] == "yes", allowDAS[indexDAS] == "yes",
                allowResplit[indexResplit] == "yes", allowResplit[indexResplit] == "yes" && allowResplitAces[indexResplitAces] == "yes", betUnits, betSpreadMultipliers, false
                );
        }
    }

    //</Factory pattern>
}
