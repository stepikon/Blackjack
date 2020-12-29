using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace Blackjack
{
    class PracticeTrueCountConversion : Practice
    {
        public PracticeTrueCountConversion(BetterUI betterUI, Random random)
            : base(betterUI, random)
        {
        }

        public override void Run()
        {
            betterUI.ClearAll();
            Console.WriteLine("To count the True count, you have to devide running count by remaining decks.\n" +
                "You can play 4-8 deck games and true count is usually less than 40.\n" +
                "Your goal is to count as many true counts as you can in 10 minutes. Correct answers are worth 1 point, wrong are worth -1 point. Good luck.\n" +
                "NOTE: true counts are either integers i or they are i.5. They are never bigger then the ratio running count / remaining decks" +
                "Press any key to start. Press q to quit");

            if (Console.ReadKey().KeyChar == 'q')
            {
                return;
            }

            int score = 0;
            string input;
            double answer;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            do
            {
                betterUI.ClearAll();

                int numerator = random.Next(-40, 41);
                double denominator = random.Next(0, 8) + 0.5 * random.Next(2);

                if (denominator == 0 || denominator > 8)
                {
                    continue;
                }

                int wholeTrueCount = (int)(numerator / denominator);
                double halves;
                double trueCount = numerator / denominator;

                if (trueCount - wholeTrueCount < -0.5)
                {
                    halves = -2;
                }
                else if (trueCount - wholeTrueCount >= -0.5 && trueCount - wholeTrueCount < 0)
                {
                    halves = -1;
                }
                else if (trueCount - wholeTrueCount >= 0 && trueCount - wholeTrueCount < 0.5)
                {
                    halves = 0;
                }
                else
                {
                    halves = 1;
                }

                trueCount = wholeTrueCount + 0.5 * halves;

                do
                {
                    Console.Write("{0} / {1} = ", numerator, denominator);

                    do
                    {
                        input = Console.ReadLine();
                        if (input.Contains("."))
                        {
                            input = input.Replace('.', ',');
                        }
                    } while (!double.TryParse(input, out answer));

                    if (answer == trueCount)
                    {
                        score++;
                    }
                    else
                    {
                        Console.WriteLine("WRONG ANSWER");
                        score--;
                    }

                } while (answer != trueCount);
            } while (stopwatch.ElapsedMilliseconds < 600000);

            Console.WriteLine("Your score was {0}", score);

            stopwatch.Stop();
            stopwatch.Reset();

            AddToHighscores(Directory.GetCurrentDirectory() + @"\Highscores\TrueCountConversion.txt", score);
        }
    }
}
