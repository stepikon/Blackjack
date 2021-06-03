using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace Blackjack
{
    //part of a <Strategy pattern>
    //STRUKTURU STRATEGY PATTERNU JSEM PREVZAL Z https://refactoring.guru/design-patterns/strategy/csharp/example

    class Game : GameStyleModsParent
    {
        private bool practice;

        public Game(Dealer dealer, Tuple<int, int> tableLimits, Player[] players, Random random, BetterUI betterUI, bool practice)
            :base(dealer, tableLimits, players, random, betterUI)
        {
            this.practice = practice;
        }


        public override void Run()
        {
            dealer.CreateShoe();
            dealer.Shuffle();
            dealer.Reset();
            bool dealerSkips; //skips dealer's turn. Dealer skips his turn if all players are over 21, all have blackjack, all surrendered or some combination of those situations

            //Initial check
            DoInitialCheckAlgorithm();

            do
            {
                dealerSkips = true;

                if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                {
                    betterUI.ClearAll();
                    betterUI.DisplayTableRules(dealer.HitSoft17);
                    betterUI.DisplayLimits(tableLimits);
                }
                else
                {
                    Console.Clear();
                }

                //Betting
                DoBettingAlgorithm();

                if (Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH)
                {
                    Console.WriteLine("-----");
                }
                else
                {
                    betterUI.ClearOptionsSpace();
                }

                //Dealing               
                DoDealingAlgorithm();

                //displays statuses
                DoDisplayStatusesAlgorithm();

                //Pair bonus gets paid always first
                DoVisibleBonusPaymentAlgorithm();

                //checks blackjacks
                DoSetBlackjackAlgorithm();

                //Displays hands
                DoDisplayHandsAlgorithm();

                //offers insurance if dealer shows an Ace
                DoVisibleOfferInsuranceAlgorithm();

                //displays blackjacks
                DoDisplayBlackjacksAlgorithm();

                //players turns
                DoVisiblePlayersTurnsAlgorithm();

                //Dealer reveals his cards
                dealer.RevealHidden();
                if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                {
                    betterUI.DisplayDealerStatus(dealer);
                }
                else
                {
                    dealer.DisplayHand();
                    Console.WriteLine("-----");
                }

                //checks if dealer may take his turn
                dealerSkips = DoCheckDealersTurnAlgorithm(dealerSkips);

                //Dealers turn
                DoVisibleDealersTurnAlgorithm(dealerSkips);

                //outcomes
                if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                {
                    betterUI.ClearMessages();
                }

                DoOutcomesAlgorithm();

                if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                {
                    betterUI.DisplayDealerStatus(dealer);
                    betterUI.DisplayPlayersStatus(players);
                }
                else
                {
                    Console.WriteLine("-----");
                }


                //updates counts
                DoUpdateCountsAlgorithm(practice);

                //Reset
                DoResetAlgorithm();

                //checks if anyone is ruined
                DoCheckRuinAlgorithm();

                //Shuffle if necessary
                DoShuffleAlgorithm();

                if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.SetCursorPosition(0,38);
                    Console.WriteLine("Press any key to continue. Press q to quit");
                }
                else
                {
                    Console.WriteLine("Press any key to continue. Press q to quit");
                }

            } while (ExistActivePlayers()&&Console.ReadKey().KeyChar!='q');

            //scores
            Console.Clear();
            Console.SetCursorPosition(0,0);
            Console.WriteLine("GAME OVER");

            foreach (Player p in players)
            {
                if (p!=null)
                {
                    Console.WriteLine("{0} went from {1} chips and ended with {2} chips (with {3} % of original chips)",
                        p.Name, p.OriginalChips, p.Chips, 
                        p.OriginalChips == 0 ? 0 : (double)100 * p.Chips / p.OriginalChips);
                }               
            }

            //highscores
            if (practice)
            {
                Console.WriteLine("No highscore (practice mode was on)");
            }
            else
            {
                foreach (Player p in players)
                {
                    if (p != null)
                    {
                        if (p is HumanPlayer)
                        {
                            AddToHighscores(Directory.GetCurrentDirectory() + @"\Highscores\AbsoluteChipAmount.txt", p.Name, p.Chips);
                            AddToHighscores(Directory.GetCurrentDirectory() + @"\Highscores\RelativeChipAmount.txt", p.Name, p.OriginalChips == 0 ? 0 : (double)p.Chips / p.OriginalChips);
                        }
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine("Press any key...");
            Console.ReadKey(true);
            Console.Clear();
        }


        public void AddToHighscores(string path, string name, double score)
        {
            if (!Directory.Exists(Directory.GetCurrentDirectory() + @"\Highscores"))
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\Highscores");
            }

            //if the file with highscores does not exist: create one and add the score into it
            //(it will be the highscore since the file is empty)
            if (!File.Exists(path))
            {
                using (File.Create(path)) { }
                using (StreamWriter streamWriter = new StreamWriter(path, false))
                {
                    Console.WriteLine("{0} achieved a new highscore ({1})", name, score);
                    streamWriter.WriteLine(name);
                    streamWriter.WriteLine(score);
                }
            }
            else
            {
                try
                {
                    bool highscore = false;
                    bool nameDoesNotExist = true;

                    using (StreamReader streamReader = new StreamReader(path))
                    {
                        string line = streamReader.ReadLine();

                        //lists through all the names in highscores file. If a player already has a highscore, checks its value
                        while (line!=null)
                        {
                            if (line == name)
                            {
                                nameDoesNotExist = false;

                                if (double.Parse(streamReader.ReadLine()) < score)
                                {
                                    Console.WriteLine("{0} achieved a new highscore ({1})", name, score);
                                    highscore = true;
                                }

                                break;
                            }
                            else
                            {
                                streamReader.ReadLine();
                            }

                            line = streamReader.ReadLine();
                        }
                    }

                    //if the name is not in the file, it's the first score of that player and thus their highscore
                    if (nameDoesNotExist)
                    {
                        using (StreamWriter streamWriter = new StreamWriter(path, true))
                        {
                            Console.WriteLine("{0} achieved a new highscore ({1})", name, score);
                            streamWriter.WriteLine(name);
                            streamWriter.WriteLine(score);
                        }
                    }

                    if (highscore)
                    {
                        //creates a list from a file
                        List<(string, double)> values = new List<(string, double)>();
                        using (StreamReader streamReader = new StreamReader(path))
                        {
                            string line;
                            while ((line = streamReader.ReadLine()) != null)
                            {
                                values.Add((line, double.Parse(streamReader.ReadLine())));
                            }
                        }

                        //checks for duplicit names
                        for (int i = 0; i < values.Count - 1; i++)
                        {
                            for (int j = i + 1; j < values.Count; j++)
                            {
                                if (values[i].Item1 == values[j].Item1)
                                {
                                    throw new ArgumentException("Duplicit names in the file");
                                }
                            }
                        }

                        //sets new highscore into the list
                        for (int i = 0; i < values.Count; i++)
                        {
                            if (values[i].Item1 == name)
                            {
                                values[i] = (name, score);
                            }
                        }

                        //writes modified list into the file
                        using (StreamWriter streamWriter = new StreamWriter(path, false))
                        {
                            for (int i = 0; i < values.Count; i++)
                            {
                                streamWriter.WriteLine(values[i].Item1);
                                streamWriter.WriteLine(values[i].Item2);
                            }
                        }
                    }
                }
                catch (Exception) //if something goes wrong, the program will overwrite all the data in the file with the argument name and score.
                {
                    using (StreamWriter streamWriter = new StreamWriter(path, false))
                    {
                        Console.WriteLine("Unexpected exception led to loss of information in file {0}", path);
                        Console.WriteLine("{0} achieved a new highscore ({1})", name, score);
                        streamWriter.WriteLine(name);
                        streamWriter.WriteLine(score);
                    }
                }
            }
        }
    }

    //</Strategy pattern>
}
