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
        private const string PRACTICE_GAME = "Practice game";

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
            PRACTICE_GAME
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
                case PRACTICE_GAME:
                    gm = new GameMode(new GameCreator(betterUI, random, true).CreateGameMode());
                    break;
                default:
                    gm = null;
                    break;
            }

            gm.Run();
        }

        public string GetChoice(string prompt, string[] options)
        {
            return betterUI.GetStringChoiceTopRight(prompt, options);
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
