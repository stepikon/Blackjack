﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Blackjack
{
    //part of a <Factory pattern>
    //STRUKTURU FACTORY PATTERNU JSEM PREVZAL Z https://www.dofactory.com/net/factory-method-design-pattern

    class GameCreator : GameModeCreator
    {
        string[] stringOptions = new string[] {
        "Number of decks: ",
        "Dealer hits soft 17: ",
        "Allow surrender: ",
        "Allow double after split: ",
        "Allow resplit: ",
        "--Allow resplit aces: ",
        "Dealer and AIs wait when drawing: ",
        "Number of human players: ",
        "Number of AIs: "};

        /*<before>
        int[] numberOfDecks = new int[] { 4, 5, 6, 7, 8 };
        string[] dealerHitsSoft17 = new string[] { "yes", "no" };
        string[] allowSurrender = new string[] { "yes", "no" };
        string[] allowDAS = new string[] { "yes", "no" };
        string[] allowResplit = new string[] { "yes", "no" };
        string[] allowResplitAces = new string[] { "yes", "no" };
        string[] waiting = new string[] { "yes", "no" };
        int[] numberOfPlayers = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };
        int[] numberOfAIs = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };
        </before>*/

        //<after>
        string[] numberOfDecks = new string[] { "4", "5", "6", "7", "8" };
        string[] dealerHitsSoft17 = new string[] { "yes", "no" };
        string[] allowSurrender = new string[] { "yes", "no" };
        string[] allowDAS = new string[] { "yes", "no" };
        string[] allowResplit = new string[] { "yes", "no" };
        string[] allowResplitAces = new string[] { "yes", "no" };
        string[] waiting = new string[] { "yes", "no" };
        string[] numberOfPlayers = new string[] { "0", "1", "2", "3", "4", "5", "6", "7" };
        string[] numberOfAIs = new string[] { "0", "1", "2", "3", "4", "5", "6", "7" };
        //</after>

        bool practice; //sets up a normal or a practice game

        public GameCreator(BetterUI betterUI, Random random, bool practice)
            : base (betterUI, random)
        {
            this.practice = practice;
        }


        public override IPlayable CreateGameMode()
        {
            ConsoleKey k;
            int optionSelected = 0;
            int tableMin;
            int tableMax;

            /*<before>
            int indexNumberOfDecks = 0;
            int indexDealerHits = 0;
            int indexSurrender = 0;
            int indexDAS = 0;
            int indexResplit = 0;
            int indexResplitAces = 0;
            int indexWaiting = 0;
            int indexNumberOfPlayers = 0;
            int indexNumberOfAIs = 0;
            </before>*/

            //<after>
            string[][] displayedOptions = new string[][] { numberOfDecks, dealerHitsSoft17, allowSurrender, allowDAS, allowResplit, allowResplitAces, waiting, numberOfPlayers, numberOfAIs };
            int[] indexes = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
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
                        Console.WriteLine(stringOptions[i] + "<{0}>", allowResplitAces[Math.Max(indexResplit, indexResplitAces)]); //if resplit is not allowed, you cannot resplit aces.
                        break;
                    case 6:
                        Console.WriteLine(stringOptions[i] + "<{0}>", waiting[indexWaiting]);
                        break;
                    case 7:
                        Console.WriteLine(stringOptions[i] + "<{0}>", numberOfPlayers[indexNumberOfPlayers]);
                        break;
                    case 8:
                        Console.WriteLine(stringOptions[i] + "<{0}>", numberOfAIs[indexNumberOfAIs]);
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
            Console.WriteLine(new String('=', 10));
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            </before>*/
            //<after>
            betterUI.DisplayMenu("Use arrows to customize your game or press enter to move on", stringOptions, displayedOptions, indexes, new string[] { "Table minimum: ", "Table maximum: " }, optionSelected, 5, ConsoleColor.DarkGray, true);
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
                                indexWaiting--;
                                indexWaiting = indexWaiting < 0 ? indexWaiting + waiting.Length : indexWaiting;
                                break;
                            case 7:
                                indexNumberOfPlayers--;
                                indexNumberOfPlayers = indexNumberOfPlayers < 0 ? indexNumberOfPlayers + numberOfPlayers.Length : indexNumberOfPlayers;
                                break;
                            case 8:
                                indexNumberOfAIs--;
                                indexNumberOfAIs = indexNumberOfAIs < 0 ? indexNumberOfAIs + numberOfAIs.Length : indexNumberOfAIs;
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
                                indexWaiting++;
                                indexWaiting %= waiting.Length;
                                break;
                            case 7:
                                indexNumberOfPlayers++;
                                indexNumberOfPlayers %= numberOfPlayers.Length;
                                break;
                            case 8:
                                indexNumberOfAIs++;
                                indexNumberOfAIs %= numberOfAIs.Length;
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
                            Console.WriteLine(stringOptions[i] + "<{0}>", allowResplitAces[Math.Max(indexResplit, indexResplitAces)]);
                            break;
                        case 6:
                            Console.WriteLine(stringOptions[i] + "<{0}>", waiting[indexWaiting]);
                            break;
                        case 7:
                            Console.WriteLine(stringOptions[i] + "<{0}>", numberOfPlayers[indexNumberOfPlayers]);
                            break;
                        case 8:
                            Console.WriteLine(stringOptions[i] + "<{0}>", numberOfAIs[indexNumberOfAIs]);
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
                Console.WriteLine(new String('=', 10));
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
                </before>*/
                //<after>
                indexes = betterUI.GetUserIntArrayInput("Use arrows to customize your game or press enter to move on", stringOptions, displayedOptions, indexes, new string[] { "Table minimum: ", "Table maximum: " }, k, optionSelected, 5, ConsoleColor.DarkGray);
                
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
            } while (k != ConsoleKey.Enter || int.Parse(numberOfPlayers[indexes[7]]) + int.Parse(numberOfAIs[indexes[8]]) == 0 || int.Parse(numberOfPlayers[indexes[7]]) + int.Parse(numberOfAIs[indexes[8]]) > 7);

            //Final Display
            /*<before>
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
                        Console.WriteLine(stringOptions[i] + "<{0}>", waiting[indexWaiting]);
                        break;
                    case 7:
                        Console.WriteLine(stringOptions[i] + "<{0}>", numberOfPlayers[indexNumberOfPlayers]);
                        break;
                    case 8:
                        Console.WriteLine(stringOptions[i] + "<{0}>", numberOfAIs[indexNumberOfAIs]);
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
            Console.WriteLine(new String('=', 10));
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            </before>*/
            //<after>
            betterUI.DisplayMenu("Use arrows to customize your game or press enter to move on", stringOptions, displayedOptions, indexes, new string[] { "Table minimum: ", "Table maximum: " }, optionSelected, 5, ConsoleColor.White, false);
            //</after>

            /*<before>
            //table minimum
            do
            {
                Console.SetCursorPosition(0,12);
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
            </before>*/

            //<after>
            //table minimum
            tableMin = betterUI.GetIntInput(12, 1);
            //table maximum
            tableMax = betterUI.GetIntInput(14, tableMin);
            //</after>

             /*<before>
            //players and AIs construction
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            Player[] players = new Player[7];
            string[] names = new string[7];
            int[] chips = new int[7];
            Tuple<int, int> tableLimits = new Tuple<int, int>(tableMin, tableMax);

            for (int i = 0; i < (numberOfPlayers[indexNumberOfPlayers]); i++)
            {                
                do
                {
                    Console.WriteLine("Enter player {0}'s name (1-25 characters)", i + 1);
                    names[i] = Console.ReadLine();
                } while (names[i].Length > 25|| names[i].Length < 1);

                do
                {
                    Console.WriteLine("Enter player {0}'s chips", i + 1);
                } while (!(int.TryParse(Console.ReadLine(), out chips[i]) && chips[i] >= 0));

                betterUI.ClearAll();
            }

            for (int i = numberOfPlayers[indexNumberOfPlayers]; i < numberOfPlayers[indexNumberOfPlayers] + numberOfAIs[indexNumberOfAIs]; i++)
            {
                do
                {
                    Console.WriteLine("Enter AI{0}'s name (1-25 characters)", i + 1 - numberOfPlayers[indexNumberOfPlayers]);
                    names[i] = Console.ReadLine();
                } while (names[i].Length > 25 || names[i].Length < 1);

                do
                {
                    Console.WriteLine("Enter AI{0}'s chips", i + 1 - numberOfPlayers[indexNumberOfPlayers]);
                } while (!( int.TryParse(Console.ReadLine(), out chips[i]) && chips[i] >= 0));

                betterUI.ClearAll();
            }

            for (int i = 0; i < 7; i++)
            {
                if (i < numberOfPlayers[indexNumberOfPlayers])
                {
                    players[i] = new HumanPlayer(names[i], new List<Card>(), betterUI, chips[i], tableLimits,
                        allowSurrender[indexSurrender] == "yes", allowDAS[indexDAS] == "yes", allowResplit[indexResplit] == "yes", allowResplit[indexResplit] == "yes" && allowResplitAces[indexResplitAces] == "yes", practice);
                }
                else if (i < numberOfPlayers[indexNumberOfPlayers] + numberOfAIs[indexNumberOfAIs])
                {
                    players[i] = new CardCountingAI(names[i], new List<Card>(), betterUI, chips[i], tableLimits,
                        allowSurrender[indexSurrender] == "yes", allowDAS[indexDAS] == "yes", allowResplit[indexResplit] == "yes", allowResplit[indexResplit] == "yes" && allowResplitAces[indexResplitAces] == "yes",
                        Math.Max(tableMin, chips[i]/1000), 2, waiting[indexWaiting] == "yes");
                }
            }

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;

            return new Game(
                new Dealer("Dealer", new List<Card>(), betterUI, numberOfDecks[indexNumberOfDecks], random, dealerHitsSoft17[indexDealerHits] == "yes", waiting[indexWaiting] == "yes"),
                tableLimits,
                players,
                random,
                betterUI,
                practice);
            </before>*/

            //<after>
            //players and AIs construction
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            Player[] players = new Player[7];
            string[] names = new string[7];
            int[] chips = new int[7];
            Tuple<int, int> tableLimits = new Tuple<int, int>(tableMin, tableMax);

            for (int i = 0; i < int.Parse(numberOfPlayers[indexes[7]]); i++)
            {
                do
                {
                    Console.WriteLine("Enter player {0}'s name (1-25 characters)", i + 1);
                    names[i] = Console.ReadLine();
                } while (names[i].Length > 25 || names[i].Length < 1);

                do
                {
                    Console.WriteLine("Enter player {0}'s chips", i + 1);
                } while (!(int.TryParse(Console.ReadLine(), out chips[i]) && chips[i] >= 0));

                betterUI.ClearAll();
            }

            for (int i = int.Parse(numberOfPlayers[indexes[7]]); i < int.Parse(numberOfPlayers[indexes[7]]) + int.Parse(numberOfAIs[indexes[8]]); i++)
            {
                do
                {
                    Console.WriteLine("Enter AI{0}'s name (1-25 characters)", i + 1 - int.Parse(numberOfPlayers[indexes[7]]));
                    names[i] = Console.ReadLine();
                } while (names[i].Length > 25 || names[i].Length < 1);

                do
                {
                    Console.WriteLine("Enter AI{0}'s chips", i + 1 - int.Parse(numberOfPlayers[indexes[7]]));
                } while (!(int.TryParse(Console.ReadLine(), out chips[i]) && chips[i] >= 0));

                betterUI.ClearAll();
            }

            for (int i = 0; i < 7; i++)
            {
                if (i < int.Parse(numberOfPlayers[indexes[7]]))
                {
                    players[i] = new HumanPlayer(names[i], new List<Card>(), betterUI, chips[i], tableLimits,
                        allowSurrender[indexes[2]] == "yes", allowDAS[indexes[3]] == "yes", allowResplit[indexes[4]] == "yes", allowResplit[indexes[4]] == "yes" && allowResplitAces[indexes[5]] == "yes", practice);
                }
                else if (i < int.Parse(numberOfPlayers[indexes[7]]) + int.Parse(numberOfAIs[indexes[8]]))
                {
                    players[i] = new CardCountingAI(names[i], new List<Card>(), betterUI, chips[i], tableLimits,
                        allowSurrender[indexes[2]] == "yes", allowDAS[indexes[3]] == "yes", allowResplit[indexes[4]] == "yes", allowResplit[indexes[4]] == "yes" && allowResplitAces[indexes[5]] == "yes",
                        Math.Max(tableMin, chips[i] / 1000), 2, waiting[indexes[6]] == "yes");
                }
            }

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;

            return new Game(
                new Dealer("Dealer", new List<Card>(), betterUI, int.Parse(numberOfDecks[indexes[0]]), random, dealerHitsSoft17[indexes[1]] == "yes", waiting[indexes[6]] == "yes"),
                tableLimits,
                players,
                random,
                betterUI,
                practice);
            //</after>
        }
    }

    //</Factory pattern>
}
