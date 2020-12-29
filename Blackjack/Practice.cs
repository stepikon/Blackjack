using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace Blackjack
{
    class Practice : IPlayable
    {
        private const string TRUE_COUNT_CONVERSION = "True count conversion";
        private const string CARD_COUNTING = "Card counting";
        private const string BASIC_STRATEGY = "Basic strategy";

        protected BetterUI betterUI;
        protected Random random;        

        public Practice(BetterUI betterUI, Random random)
        {
            this.betterUI = betterUI;
            this.random = random;
        }

        public virtual void Run()
        {
            GameMode gm;
            string[] practiceOptions = new string[]
            {
            TRUE_COUNT_CONVERSION,
            CARD_COUNTING,
            BASIC_STRATEGY,
            };

            switch (GetChoice("What do you want to practice?", practiceOptions))
            {
                case TRUE_COUNT_CONVERSION:
                    gm = new GameMode(new PracticeTrueCountConversionCreator(betterUI, random).CreateGameMode());
                    break;
                case CARD_COUNTING:
                    gm = new GameMode(new PracticeCardCountingCreator(betterUI, random).CreateGameMode());
                    break;
                case BASIC_STRATEGY:
                    gm = new GameMode(new PracticeBasicStrategyCreator(betterUI, random).CreateGameMode());
                    break;
                default:
                    gm = null;
                    break;
            }

            gm.Run();
        }

        
        /*
        public void RunTopDeviations()
        {
            betterUI.ClearAll();
            Console.Write("Hit 17 most important deviations. Enter the correct decision.\n" +
                "Each correct answer is worth a point, the game ends after a wrong answer.\n" +
                "Press any key to start. Press q to quit.");
            if (Console.ReadKey().KeyChar == 'q')
            {
                return;
            }

            betterUI.ClearAll();

            string[] deviations = new string[] { 
            "Insurance at true count: >= 3",
            "16 vs dealer 10 at true count: Any positive running count",
            "16 vs dealer 9 at true count: >= 4",
            "15 vs dealer 10 at true count: >= 4",
            "13 vs dealer 2 at true count: <= -1",
            "13 vs dealer 3 at true count: < -2",
            "12 vs dealer 2 at true count: >=3 ",
            "12 vs dealer 3 at true count: >= 2", 
            "12 vs dealer 4 at true count: any negative running count",
            "12 vs dealer 5 at true count: < -1",
            "12 vs dealer 6 at true count: < -3",
            "11 vs dealer A at true count: >=-1",
            "10 vs dealer 10 at true count: >= 4",
            "10 vs dealer A at true count: >= 3",
            "9 vs dealer 2 at true count: >= 1",
            "9 vs dealer 7 at true count: >= 3",
            "10-10 vs dealer 5 at true count: >= 5",
            "10-10 vs dealer 6 at true count: >= 4",
            };

            const string INSURANCE = "insurance";
            const string NO_INSURANCE = "no insurance";
            const string HIT = "hit";
            const string STAND = "stand";
            const string DOUBLE = "double";
            const string SPLIT = "split";

            int index;
            int score = 0;
            string playerChoice, correctChoice;
            bool isCorrect;

            do
            {
                index = random.Next(18);

                switch (index)
                {
                    case 0:
                        correctChoice = INSURANCE;
                        break;
                    case 1:
                    case 2:
                    case 3:
                    case 6:
                    case 7:
                        correctChoice = STAND;
                        break;
                    case 4:
                    case 5:
                    case 8:
                    case 9:
                    case 10:
                        correctChoice = HIT;
                        break;
                    case 11:
                    case 12:
                    case 13:
                    case 14:
                    case 15:
                        correctChoice = DOUBLE;
                        break;
                    default:
                        correctChoice = SPLIT;
                        break;
                }

                if (index == 0)
                {
                    playerChoice = GetChoice(deviations[0], new string[] { INSURANCE, NO_INSURANCE });
                }
                else
                {
                    playerChoice = GetChoice(deviations[index], new string[] { HIT, STAND, DOUBLE, SPLIT});
                }

                if (playerChoice.Equals(correctChoice))
                {
                    score++;
                    Console.WriteLine("You are correct\n" +
                        "press any key to continue, press q to quit.");
                    isCorrect = true;
                }
                else
                {
                    Console.WriteLine("You are wrong, correct decision is {0}", correctChoice);
                    isCorrect = false;
                }
            } while (isCorrect && Console.ReadKey().KeyChar != 'q');

            Console.WriteLine("Your score was {0}", score);
            AddToHighscores(Directory.GetCurrentDirectory() + @"\Highscores\18Deviations", score);
        }
        */
        public string GetChoice(string prompt, string[] options)
        {
            betterUI.ClearAll();

            int chosenOption = 0;
            ConsoleKey ch;

            do
            {
                //displays current choice
                betterUI.ClearAll();
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(prompt);
                for (int i = 0; i < options.Length; i++)
                {
                    if (i == chosenOption)
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.WriteLine(options[i]);
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(options[i]);
                    }
                }

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
                    default:
                        prompt = prompt.Contains(" (use up and down arrows or W and S keys)") ? prompt : prompt + " (use up and down arrows or W and S keys)";
                        break;
                }               
            } while (ch != ConsoleKey.Enter);

            betterUI.ClearAll();
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;

            return options[chosenOption];
        }

        public void AddToHighscores(string path, int score)
        {
            if (!Directory.Exists(Directory.GetCurrentDirectory() + @"\Highscores"))
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\Highscores");
            }

            if (!File.Exists(path))
            {
                using (File.Create(path)) { }
                using (StreamWriter streamWriter = new StreamWriter(path, false))
                {
                    Console.WriteLine("new highscore");
                    streamWriter.WriteLine(score);
                }
            }
            else
            {
                try
                {
                    bool highscore = false;
                    using (StreamReader streamReader = new StreamReader(path))
                    {
                        if (int.Parse(streamReader.ReadLine()) < score)
                        {
                            Console.WriteLine("new highscore");
                            highscore = true;
                        }
                    }

                    if (highscore)
                    {
                        using (StreamWriter streamWriter = new StreamWriter(path, false))
                        {
                            streamWriter.WriteLine(score);
                        }
                    }
                }
                catch (Exception)
                {
                    using (StreamWriter streamWriter = new StreamWriter(path, false))
                    {
                        streamWriter.WriteLine(score);
                    }
                }
            }
        }
    }
}
