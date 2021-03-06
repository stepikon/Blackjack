﻿using System;
using System.Collections.Generic;
using System.Resources;
using System.Text;

namespace Blackjack
{
    class BetterUI
    {
        private const int MAX_PLAYERS = 7;

        private const string BLACKJACK_PAYOUT = "Blackjack pays 3:2";
        private const string DEALER_RULES_STAND_SOFT17 = "Dealer must draw to 16 and stand on all 17's.";
        private const string DEALER_RULES_HIT_SOFT17 = "Dealer must hit soft 17.";
        private const string INSURANCE_PAYOUT = "Insurance pays 2:1";
        
        public BetterUI()
        {
        }


        //DISPLAY METHODS
        public void DisplayDealerStatus(Dealer dealer)
        {
            ClearDealerStatus(dealer);

            Console.SetCursorPosition((int)((Console.WindowWidth - dealer.Name.Length)/2), 0);
            Console.WriteLine(dealer.Name);

            Console.SetCursorPosition((int)((Console.WindowWidth - dealer.hand.Count * 2 - "Hand: ".Length) / 2), 1);
            Console.Write("Hand: ");

            foreach (Card c in dealer.hand)
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = c.ColorOfConsole;
                Console.Write(c.Name);

                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" ");
            }

            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine(dealer.GetHiddenCardName());

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition((int)((Console.WindowWidth - "Hand total: dd".Length) / 2), 2);
            Console.WriteLine("Hand total: {0}", dealer.GetHandValue(dealer.hand).Item2);

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;

            DisplayShoe(dealer);
        }


        public void DisplayShoe(Dealer dealer)
        {
            try
            {
                Console.SetCursorPosition(Console.WindowWidth - 25, 0);
                Console.Write("Shoe: ");
                for (int i = 0; i < dealer.DeckAmount * 2; i++)
                {
                    Console.SetCursorPosition(Console.WindowWidth - 25 + i, 1);
                    if (i == dealer.DeckAmount * 2 - Math.Ceiling((double)dealer.DeckPenetration * 2 / 52))
                    {
                        Console.BackgroundColor = ConsoleColor.Yellow;
                    }
                    else if (i < dealer.DeckAmount * 2 - dealer.GetDecksInDiscard() * 2)
                    {
                        Console.BackgroundColor = ConsoleColor.Green;
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.Red;
                    }
                    Console.Write(' ');
                }
                Console.BackgroundColor = ConsoleColor.Black;

                Console.SetCursorPosition(Console.WindowWidth - 25, 2);
                Console.Write("Discard tray: ");
                for (int i = 0; i < dealer.DeckAmount * 2; i++)
                {
                    Console.SetCursorPosition(Console.WindowWidth - 25 + i, 3);
                    if (i < dealer.GetDecksInDiscard() * 2)
                    {
                        Console.BackgroundColor = ConsoleColor.Green;
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.Red;
                    }
                    Console.Write(' ');
                }
                Console.BackgroundColor = ConsoleColor.Black;
            }
            catch (ArgumentException e)
            {
                Console.Clear();
                Console.WriteLine(e.Message);
                Console.WriteLine("Fatal error has occured. Please don't change the window size during the round.");
            }
        }


        public void DisplayDealerBlackjack(Dealer dealer)
        {
            ClearDealerBlackjack();

            if (dealer.HasBlackjack)
            {
                Console.SetCursorPosition((int)((Console.WindowWidth - "BLACKJACK".Length) / 2), 3);
                Console.WriteLine("BLACKJACK");
            }
            else
            {
                Console.SetCursorPosition((int)((Console.WindowWidth - "Nobody home".Length) / 2), 3);
                Console.WriteLine("Nobody home");
            }
        }

        
        public void DisplayPlayersStatus(Player[] players)
        {
            ClearPlayersStatus();

            int spacing = (int)(Console.WindowWidth / MAX_PLAYERS);

            for (int i = 0; i < MAX_PLAYERS; i++)
            {
                if (i < players.Length && players[i] != null)
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.SetCursorPosition(Console.WindowWidth - spacing * (i + 1), 19); //player order is from right to left in blackjack; from dealer's POV it's from left to right
                    Console.Write(players[i].Name);
                    Console.SetCursorPosition(Console.WindowWidth - spacing * (i + 1), 20);
                    Console.Write(players[i].IsRuined || players[i].IsGone ? "Gone" : "In game");
                    Console.SetCursorPosition(Console.WindowWidth - spacing * (i + 1), 21);
                    Console.Write("Chips: ${0}", players[i].Chips >= 1000000000 ? "billions" : String.Format("{0:#,#,0.00}", players[i].Chips));

                    for (int j = 0; j < players[i].hands.Length; j++)
                    {
                        if (players[i].hands[j] != null && !players[i].IsGone && !players[i].IsRuined)
                        {
                            if (players[i].Bets[j] >= 1000000000)
                            {
                                Console.SetCursorPosition(Console.WindowWidth - spacing * (i + 1), 23 + j * 2);
                                Console.Write("Bet: billions");
                            }
                            else
                            {
                                Console.SetCursorPosition(Console.WindowWidth - spacing * (i + 1), 23 + j * 2);
                                Console.Write("Bet: {0}", String.Format("${0:#,#,0.##}", players[i].Bets[j]));
                            }

                            Console.SetCursorPosition(Console.WindowWidth - spacing * (i + 1), 23 + j * 2 + 1);
                            Console.Write("hand {0}: ", j + 1);
                            foreach (Card c in players[i].hands[j])
                            {
                                if (c != null)
                                {
                                    Console.BackgroundColor = ConsoleColor.White;
                                    Console.ForegroundColor = c.ColorOfConsole;
                                    Console.Write("{0}", c.Name);

                                    Console.BackgroundColor = ConsoleColor.Black;
                                    Console.ForegroundColor = ConsoleColor.White;
                                    Console.Write(" ");
                                }                                
                            }
                        }
                    }
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.SetCursorPosition(Console.WindowWidth - spacing * (i + 1), 19);
                    Console.Write("Empty spot");
                }
            }
        }


        public void DisplayPlayersBlackjack(Player[] players)
        {
            ClearPlayersBlackjack();

            int spacing = (int)(Console.WindowWidth / MAX_PLAYERS);

            for (int i = 0; i < MAX_PLAYERS; i++)
            {
                if (i < players.Length && players[i] != null)
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.SetCursorPosition(Console.WindowWidth - spacing * (i + 1), 25);
                    Console.Write(players[i].HasBlackjack ? "BLACKJACK" : "");
                }
            }
        }


        public void DisplayMessage(string message)
        {
            ClearMessages();

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;

            if (message.Length > 3 * Console.WindowWidth)
            {
                Console.SetCursorPosition(0, 37);
                Console.WriteLine(message);
            }
            else if (message.Length > 2 * Console.WindowWidth)
            {
                Console.SetCursorPosition(0, 4);
                Console.WriteLine(message);
            }
            else if (message.Length > Console.WindowWidth)
            {
                Console.SetCursorPosition(0, 5);
                Console.WriteLine(message);
            }
            else
            {
                Console.SetCursorPosition((int)((Console.WindowWidth - message.Length) / 2), 5);
                Console.WriteLine(message);
            }
        }


        //displays whose turn it is
        public void DisplayTurn(string name)
        {
            ClearTurn();

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition((int)((Console.WindowWidth - name.Length - "It's 's turn.".Length) / 2),7);
            Console.Write("It's {0}'s turn.", name);
        }


        public void DisplayLimits(Tuple<int, int> tablelimits)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(0,32);
            Console.Write("Table limits: {0}-{1}", tablelimits.Item1, tablelimits.Item2);
        }


        public void DisplayTableRules(bool hit17)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.SetCursorPosition((int)((Console.WindowWidth - BLACKJACK_PAYOUT.Length) / 2), 15);
            Console.Write(BLACKJACK_PAYOUT);

            if (hit17)
            {
                Console.SetCursorPosition((int)((Console.WindowWidth - DEALER_RULES_HIT_SOFT17.Length) / 2), 16);
                Console.Write(DEALER_RULES_HIT_SOFT17);
            }
            else
            {
                Console.SetCursorPosition((int)((Console.WindowWidth - DEALER_RULES_STAND_SOFT17.Length) / 2), 16);
                Console.Write(DEALER_RULES_STAND_SOFT17);
            }

            Console.SetCursorPosition((int)((Console.WindowWidth - INSURANCE_PAYOUT.Length) / 2), 17);
            Console.Write(INSURANCE_PAYOUT);
        }


        //displays whether a player won/lost/pushed/surrendered/won an insurance on a certain hand
        public void DisplayOutcomes(string outcome, int handIndex, int handValue, Player[] players, Player player)
        {
            int spacing = (int)(Console.WindowWidth / MAX_PLAYERS);

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(Console.WindowWidth - (spacing * (Array.IndexOf(players, player) + 1)), 33 + handIndex);
            Console.Write("Hand {0}: {1} ({2})", handIndex + 1, outcome, handValue);
        }


        //displays whether a player won a side bet
        public void DisplayPairs(Player[] players, Player player, string outcome)
        {
            int spacing = (int)(Console.WindowWidth / MAX_PLAYERS);

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(Console.WindowWidth - (spacing * (Array.IndexOf(players, player) + 1)), 31);
            Console.Write(outcome);
        }


        public void DisplayMenu(string prompt, string[] stringOptions, string[][] displayedOptions, int[] indexes, string[] grayPrompts, int optionSelected, int conditionIndex, ConsoleColor promptsColor, bool showSelected)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            ClearAll();
            Console.WriteLine(prompt);
            for (int i = 0; i < stringOptions.Length; i++)
            {
                if (i == optionSelected && showSelected)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                }

                if (conditionIndex > 0 && conditionIndex < stringOptions.Length && i == conditionIndex)
                {
                    Console.WriteLine(stringOptions[i] + "<{0}>", displayedOptions[i][Math.Max(indexes[i - 1], indexes[i])]);
                }
                else
                {
                    Console.WriteLine(stringOptions[i] + "<{0}>", displayedOptions[i][indexes[i]]);
                }

            }
            if (grayPrompts != null)
            {
                DisplayPrompts(grayPrompts, promptsColor);
            }

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }


        public void DisplayPrompts(string[] grayPrompts, ConsoleColor color)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = color;
            Console.WriteLine(new String('=', 10));
            for (int i = 0; i < grayPrompts.Length; i++)
            {
                Console.WriteLine(grayPrompts[i]);
                Console.WriteLine();
            }
            Console.WriteLine(new String('=', 10));
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }


        //CLEAR METHODS
        public void ClearDealerStatus(Dealer dealer)
        {
            Console.SetCursorPosition((int)((Console.WindowWidth - dealer.Name.Length) / 2), 0);
            Console.WriteLine(new String(' ', dealer.Name.Length));


            Console.SetCursorPosition((int)((Console.WindowWidth - dealer.hand.Count * 2 - "Hand: ".Length - 34) / 2), 1); //dealer hand may contain 17 cards at most, that makes 17*2=34 characters (with spaces)
            Console.WriteLine(new String(' ',"Hand: ".Length + 34));

            Console.SetCursorPosition((int)((Console.WindowWidth - "Hand total: dd".Length) / 2), 2);
            Console.WriteLine(new String(' ',"Hand total: dd".Length));
        }


        public void ClearDealerBlackjack()
        {
            Console.SetCursorPosition((int)((Console.WindowWidth - "Nobody home".Length) / 2), 3);
            Console.WriteLine(new String(' ',"Nobody home".Length));
        }


        public void ClearPlayersStatus()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            for (int i = 19; i <= 30; i++)
            {
                if (i==25)
                {
                    continue;
                }

                Console.SetCursorPosition(0, i);
                Console.WriteLine(new String(' ', Console.WindowWidth));
            }
        }


        public void ClearPlayersBlackjack()
        {
            int spacing = (int)(Console.WindowWidth / MAX_PLAYERS);

            for (int i = 0; i < MAX_PLAYERS; i++)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(Console.WindowWidth - spacing * (i + 1), 25);
                Console.Write(new String(' ', "BLACKJACK".Length));
            }
        }


        public void ClearOptionsSpace()
        {
            Console.BackgroundColor = ConsoleColor.Black;

            for (int i = 8; i <= 13; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write(new String(' ', Console.WindowWidth));
            }
        }


        public void ClearMessages()
        {
            Console.BackgroundColor = ConsoleColor.Black;

            for (int i = 4; i <= 6; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write(new String(' ', Console.WindowWidth));
            }

            for (int i = 37; i < Console.WindowHeight; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write(new String(' ', Console.WindowWidth));
            }
        }


        public void ClearTurn()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(0, 7);
            Console.Write(new String(' ', Console.WindowWidth));
        }


        public void ClearLimits()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.SetCursorPosition(0, 32);
            Console.Write(new String(' ', Console.WindowWidth));
        }


        public void ClearAll()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            for (int i = 0; i < Console.LargestWindowHeight; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.WriteLine(new String(' ', Console.WindowWidth));
            }

            Console.SetCursorPosition(0,0);
        }


        //GET INPUT METHODS
        //reads user input, 1 prompt
        public int GetIntInput(string prompt)
        {
            ClearOptionsSpace();

            string input;
            int i;

            Console.SetCursorPosition((int)((Console.WindowWidth - prompt.Length) / 2), 7);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(prompt);

            do
            {
                Console.SetCursorPosition(0, 8);
                Console.Write(new String(' ', Console.WindowWidth));
                Console.SetCursorPosition((int)(Console.WindowWidth / 2 - 2), 8);
                input = Console.ReadLine();
            } while (!int.TryParse(input, out i));

            return i;
        }


        public int GetIntInput(int cursorTop, int minimum)
        {
            int output;

            do
            {
                Console.SetCursorPosition(0, cursorTop);
                Console.Write(new String(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, cursorTop);
            } while (!(int.TryParse(Console.ReadLine(), out output) && output >= minimum));

            return output;
        }


        public int GetIntInput(int cursorTop, int maximum, int minimum)
        {
            int output;

            do
            {
                Console.SetCursorPosition(0, cursorTop);
                Console.Write(new String(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, cursorTop);
            } while (!(int.TryParse(Console.ReadLine(), out output) && output >= minimum && output <= maximum));

            return output;
        }


        //reads user int input, multiple prompts
        public int GetIntInput(string[] prompts)
        {
            ClearOptionsSpace();

            string input;
            int i;

            if (prompts.Length > 4) //shouldn't happen
            {
                prompts = new string[] { "too many prompts" };
            }

            for (int index = 0; index < prompts.Length; index++)
            {
                Console.SetCursorPosition((int)((Console.WindowWidth - prompts[index].Length) / 2), 8 + index);
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(prompts[index]);
            }
            
            do
            {
                Console.SetCursorPosition(0, 8 + prompts.Length);
                Console.Write(new String(' ', Console.WindowWidth));
                Console.SetCursorPosition((int)(Console.WindowWidth / 2 - 2), 8 + prompts.Length);
                input = Console.ReadLine();
            } while (!int.TryParse(input, out i));

            return i;
        }


        public string GetStringInput(int cursorTop, int maxLength, int minLength)
        {
            string output;

            do
            {
                Console.SetCursorPosition(0, cursorTop);
                Console.Write(new String(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, cursorTop);
                output = Console.ReadLine();
            } while (output.Length > maxLength || output.Length < minLength);

            return output;
        }


        //prompt and options will be displayed in the top right corner
        public string GetStringChoiceTopRight(string prompt, string[] options) 
        {
            if (options == null || options[0] == "")
            {
                throw new ArgumentException();
            }

            int chosenOption = 0;
            ConsoleKey k;

            //initial display
            ClearAll();
            Console.WriteLine(prompt);
            for (int i = 0; i < options.Length; i++)
            {
                if (i == chosenOption)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                }

                Console.WriteLine(options[i]);
            }

            //gets the choice; help from https://stackoverflow.com/questions/4351258/c-sharp-arrow-key-input-for-a-console-app
            do
            {
                k = Console.ReadKey(true).Key;

                switch (k)
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                        chosenOption--;
                        chosenOption = chosenOption < 0 ? chosenOption + options.Length : chosenOption;
                        break;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                        chosenOption++;
                        chosenOption %= options.Length;
                        break;
                    case ConsoleKey.Enter:
                        break;
                    default:
                        prompt = prompt.Contains(" (use up and down arrows or W and S keys)") ? prompt : prompt + " (use up and down arrows or W and S keys)";
                        break;
                }

                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;

                //Display current choice
                ClearAll();
                Console.WriteLine(prompt);
                for (int i = 0; i < options.Length; i++)
                {
                    if (i == chosenOption)
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    Console.WriteLine(options[i]);
                }

                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
            } while (k != ConsoleKey.Enter);

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;

            return options[chosenOption];
        }


        //prompt and options will be displayed in the top right corner, prompt will be colorised
        public string GetStringChoiceTopRight(string prompt, string[] options, ConsoleColor color)
        {
            if (options == null || options[0] == "")
            {
                throw new ArgumentException();
            }

            int chosenOption = 0;
            ConsoleKey k;

            //initial display
            ClearAll();
            Console.ForegroundColor = color;
            Console.WriteLine(prompt);
            for (int i = 0; i < options.Length; i++)
            {
                if (i == chosenOption)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                }

                Console.WriteLine(options[i]);
            }

            //gets the choice; help from https://stackoverflow.com/questions/4351258/c-sharp-arrow-key-input-for-a-console-app
            do
            {
                k = Console.ReadKey(true).Key;

                switch (k)
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                        chosenOption--;
                        chosenOption = chosenOption < 0 ? chosenOption + options.Length : chosenOption;
                        break;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                        chosenOption++;
                        chosenOption %= options.Length;
                        break;
                    case ConsoleKey.Enter:
                        break;
                    default:
                        prompt = prompt.Contains(" (use up and down arrows or W and S keys)") ? prompt : prompt + " (use up and down arrows or W and S keys)";
                        break;
                }

                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;

                //Display current choice
                ClearAll();
                Console.ForegroundColor = color;
                Console.WriteLine(prompt);
                for (int i = 0; i < options.Length; i++)
                {
                    if (i == chosenOption)
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    Console.WriteLine(options[i]);
                }

                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
            } while (k != ConsoleKey.Enter);

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;

            return options[chosenOption];
        }


        //prompt and options will be displayed in the middle
        public string GetStringChoice(string prompt, string[] options)
        {
            ClearOptionsSpace();

            if (options == null || options[0] == "")
            {
                throw new ArgumentException();
            }

            int chosenOption = 0;
            ConsoleKey ch;

            try
            {
                //displays default choice
                for (int i = 0; i < options.Length; i++)
                {
                    if (i == chosenOption)
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.SetCursorPosition((int)((Console.WindowWidth - options[i].Length) / 2), 9 + i);
                        Console.WriteLine(options[i]);
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.SetCursorPosition((int)((Console.WindowWidth - options[i].Length) / 2), 9 + i);
                        Console.WriteLine(options[i]);
                    }
                }
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;

                do
                {
                    Console.SetCursorPosition((int)((Console.WindowWidth - prompt.Length) / 2), 8);
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(prompt);

                    //gets the choice; help from https://stackoverflow.com/questions/4351258/c-sharp-arrow-key-input-for-a-console-app
                    ch = Console.ReadKey(true).Key;
                    switch (ch)
                    {
                        case ConsoleKey.DownArrow:
                        case ConsoleKey.S:
                            chosenOption++;
                            chosenOption %= options.Length;
                            break;
                        case ConsoleKey.UpArrow:
                        case ConsoleKey.W:
                            chosenOption--;

                            if (chosenOption < 0)
                            {
                                chosenOption += options.Length;
                            }

                            chosenOption %= options.Length;
                            break;
                        case ConsoleKey.Enter:
                            break;
                        default:
                            prompt = prompt.Contains(" (use up and down arrows or W and S keys)") ? prompt : prompt + " (use up and down arrows or W and S keys)";
                            break;
                    }

                    //displays current choice
                    for (int i = 0; i < options.Length; i++)
                    {
                        if (i == chosenOption)
                        {
                            Console.BackgroundColor = ConsoleColor.White;
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.SetCursorPosition((int)((Console.WindowWidth - options[i].Length) / 2), 9 + i);
                            Console.WriteLine(options[i]);
                        }
                        else
                        {
                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.SetCursorPosition((int)((Console.WindowWidth - options[i].Length) / 2), 9 + i);
                            Console.WriteLine(options[i]);
                        }
                    }
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                } while (ch != ConsoleKey.Enter);

                ClearOptionsSpace();
                return options[chosenOption];
            }
            catch (ArgumentException)
            {
                Console.WriteLine("New window size is too small");
                return "";
            }
        }


        //method for for Yes/No input
        public bool GetBoolChoice(string prompt, string[] options)
        {
            ClearOptionsSpace();

            if (options == null || options[0] == "" || options.Length != 2)
            {
                throw new ArgumentException();
            }

            int chosenOption = 0;
            ConsoleKey ch;

            try
            {
                //displays default choice
                for (int i = 0; i < options.Length; i++)
                {
                    if (i == chosenOption)
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.SetCursorPosition((int)((Console.WindowWidth - options[i].Length) / 2), 9 + i);
                        Console.WriteLine(options[i]);
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.SetCursorPosition((int)((Console.WindowWidth - options[i].Length) / 2), 9 + i);
                        Console.WriteLine(options[i]);
                    }
                }
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;

                do
                {
                    Console.SetCursorPosition((int)((Console.WindowWidth - prompt.Length) / 2), 8);
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(prompt);

                    //gets the choice; help from https://stackoverflow.com/questions/4351258/c-sharp-arrow-key-input-for-a-console-app
                    ch = Console.ReadKey(true).Key;
                    switch (ch)
                    {
                        case ConsoleKey.DownArrow:
                        case ConsoleKey.S:
                            chosenOption++;
                            chosenOption %= options.Length;
                            break;
                        case ConsoleKey.UpArrow:
                        case ConsoleKey.W:
                            chosenOption--;

                            if (chosenOption < 0)
                            {
                                chosenOption += options.Length;
                            }

                            chosenOption %= options.Length;
                            break;
                        case ConsoleKey.Enter:
                            break;
                        default:
                            prompt += " (use up and down arrows or W and S keys)";
                            break;
                    }

                    //displays current choice
                    for (int i = 0; i < options.Length; i++)
                    {
                        if (i == chosenOption)
                        {
                            Console.BackgroundColor = ConsoleColor.White;
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.SetCursorPosition((int)((Console.WindowWidth - options[i].Length) / 2), 9 + i);
                            Console.WriteLine(options[i]);
                        }
                        else
                        {
                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.SetCursorPosition((int)((Console.WindowWidth - options[i].Length) / 2), 9 + i);
                            Console.WriteLine(options[i]);
                        }
                    }
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                } while (ch != ConsoleKey.Enter);

                ClearOptionsSpace();
                return chosenOption == 0; //I want to put options like "yes" or "confirm" on the 0th index
            }
            catch (ArgumentException)
            {
                Console.WriteLine("New window size is too small");
                return false;
            }
        }


        
        public int[] GetUserIntArrayInput(string prompt, string[] stringOptions, string[][] displayedOptions, int[] indexes, string[] grayPrompts,
            ConsoleKey k, int optionSelected, int conditionIndex, ConsoleColor promptsColor)
        {
            optionSelected %= stringOptions.Length;

            switch (k)
            {
                case ConsoleKey.LeftArrow:
                    indexes[optionSelected]--;
                    indexes[optionSelected] = indexes[optionSelected] < 0 ? indexes[optionSelected] + displayedOptions[optionSelected].Length : indexes[optionSelected];
                    break;
                case ConsoleKey.UpArrow:
                    optionSelected--;
                    optionSelected = optionSelected < 0 ? optionSelected += stringOptions.Length : optionSelected;
                    break;
                case ConsoleKey.RightArrow:
                    indexes[optionSelected]++;
                    indexes[optionSelected] %= displayedOptions[optionSelected].Length;
                    break;
                case ConsoleKey.DownArrow:
                    optionSelected++;
                    optionSelected %= stringOptions.Length;
                    break;
                default:
                    break;
            }

            if (conditionIndex > 0 && conditionIndex < stringOptions.Length)
            {
                indexes[conditionIndex] = Math.Max(indexes[conditionIndex - 1], indexes[conditionIndex]);
            }

            //Display
            DisplayMenu(prompt, stringOptions, displayedOptions, indexes, grayPrompts, optionSelected, conditionIndex, promptsColor, true);

            return indexes;
        }
    }
}
