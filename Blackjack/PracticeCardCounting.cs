using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace Blackjack
{
    class PracticeCardCounting : Practice
    {
        public PracticeCardCounting(BetterUI betterUI, Random random)
            : base(betterUI, random)
        {
        }

        public override void Run()
        {
            betterUI.ClearAll();
            Console.WriteLine("Standard Hi-Lo system.\n" +
                "After every 26 cards (half a deck) you'll be asked to enter current running count.\n" +
                "for every correct entry you'll get 1 point. Game ends when you're wrong.\n" +
                "NOTE: count does NOT reset after your entry\n" +
                "Press any key to start. Press 'q' to quit.");

            if (Console.ReadKey().KeyChar == 'q')
            {
                return;
            }

            Tuple<string, int>[] cards = new Tuple<string, int>[] {
            new Tuple<string, int>("2", 1),
            new Tuple<string, int>("3", 1),
            new Tuple<string, int>("4", 1),
            new Tuple<string, int>("5", 1),
            new Tuple<string, int>("6", 1),
            new Tuple<string, int>("7", 0),
            new Tuple<string, int>("8", 0),
            new Tuple<string, int>("9", 0),
            new Tuple<string, int>("10", -1),
            new Tuple<string, int>("J", -1),
            new Tuple<string, int>("Q", -1),
            new Tuple<string, int>("K", -1),
            new Tuple<string, int>("A", -1)
            };

            int score = 0;
            int index;
            int runningCount = 0;
            int input;
            int sleepMiliseconds = 1000;

            bool isCorrect;

            do
            {
                for (int i = 0; i < 26; i++)
                {
                    betterUI.ClearAll();
                    index = random.Next(13);

                    Console.WriteLine("Card {0}:", i + 1);

                    //to distinguish 2 consecutive hands
                    if (i % 2 == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                    }

                    Console.WriteLine(cards[index].Item1);
                    runningCount += cards[index].Item2;
                    Thread.Sleep(sleepMiliseconds);

                    Console.ForegroundColor = ConsoleColor.White;
                }

                do
                {
                    betterUI.ClearAll();
                    Console.WriteLine("What is the running count?");
                } while (!int.TryParse(Console.ReadLine(), out input));

                if (input == runningCount)
                {
                    isCorrect = true;
                    score++;
                    sleepMiliseconds = Math.Max(sleepMiliseconds - 20, 300);
                }
                else
                {
                    isCorrect = false;
                }
            } while (isCorrect);

            Console.WriteLine("WRONG, running count is {0}", runningCount);
            Console.WriteLine("Your score was {0}", score);
            AddToHighscores(Directory.GetCurrentDirectory() + @"\Highscores\CardCounting.txt", score);

            Console.WriteLine();
            Console.WriteLine("Press any key...");
            Console.ReadKey(true);
            Console.Clear();
        }
    }
}
