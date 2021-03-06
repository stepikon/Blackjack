﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace Blackjack
{
    //this class is a part of a <Strategy pattern>
    //STRUKTURU STRATEGY PATTERNU JSEM PREVZAL Z https://refactoring.guru/design-patterns/strategy/csharp/example
    
    class Practice : IPlayable
    {
        private const string TRUE_COUNT_CONVERSION = "True count conversion";
        private const string CARD_COUNTING = "Card counting";
        private const string BASIC_STRATEGY = "Basic strategy";
        private const string PRACTICE_GAME = "Practice game";
        private const string QUIT = "Return to main menu";

        protected BetterUI betterUI;
        protected Random random;

        public Practice(BetterUI betterUI, Random random)
        {
            this.betterUI = betterUI;
            this.random = random;
        }


        //launches a concrete practice mode
        public virtual void Run()
        {
            //part of a <Practice Factory pattern>
            //STRUKTURU FACTORY PATTERNU JSEM PREVZAL Z https://www.dofactory.com/net/factory-method-design-pattern

            GameMode gm;

            GameModeCreator[] creators = new GameModeCreator[]
            {
            new PracticeTrueCountConversionCreator(betterUI, random),
            new PracticeCardCountingCreator(betterUI, random),
            new PracticeBasicStrategyCreator(betterUI, random),
            new GameCreator(betterUI, random, true),
            new QuitCreator(betterUI, random)
            };

            string[] practiceOptions = new string[]
            {
            TRUE_COUNT_CONVERSION,
            CARD_COUNTING,
            BASIC_STRATEGY,
            PRACTICE_GAME,
            QUIT
            };

            //part of a <Practice Strategy pattern>
            //STRUKTURU STRATEGY PATTERNU JSEM PREVZAL Z https://refactoring.guru/design-patterns/strategy/csharp/example

            gm = new GameMode(creators[Array.IndexOf(practiceOptions, GetChoice("What do you want to practice?", practiceOptions))].CreateGameMode());

            gm.Run();

            //</Practice Factory pattern>
            //</Practice Strategy pattern>
        }


        //gets user's string choice
        public string GetChoice(string prompt, string[] options)
        {
            return betterUI.GetStringChoiceTopRight(prompt, options);
        }


        //gets user's string choice with colorised prompt
        public string GetChoice(string prompt, string[] options, ConsoleColor color)
        {
            return betterUI.GetStringChoiceTopRight(prompt, options, color);
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

    //</Strategy pattern>
}
