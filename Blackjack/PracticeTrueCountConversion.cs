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
        private bool onlyIntegers;

        public PracticeTrueCountConversion(BetterUI betterUI, Random random, bool onlyIntegers)
            : base(betterUI, random)
        {
            this.onlyIntegers = onlyIntegers;
        }

        public override void Run()
        {
            betterUI.ClearAll();
            if (onlyIntegers)
            {
                Console.WriteLine("To count the True count, you have to devide running count by remaining decks.\n" +
                    "You can play 4-8 deck games and true count is usually between -40 and 40.\n" +
                    "Your goal is to count as many true counts as you can in 5 minutes. Correct answers are worth 1 point, wrong are worth -1 point. Good luck.\n" +
                    "NOTE: true counts are only integers less than or equal to the ratio running count / remaining decks\n" +
                    "Press any key to start. Press q to quit");
            }
            else
            {
                Console.WriteLine("To count the True count, you have to devide running count by remaining decks.\n" +
                    "You can play 4-8 deck games and true count is usually between -40 and 40.\n" +
                    "Your goal is to count as many true counts as you can in 5 minutes. Correct answers are worth 1 point, wrong are worth -1 point. Good luck.\n" +
                    "NOTE: true counts are in format k*0.5, where k is an integer (i.e. 1; 0.5; -3.5).\n" +
                    "They are never bigger then the ratio running count / remaining decks\n" +
                    "Press any key to start. Press q to quit");
            }

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

                trueCount = onlyIntegers ? Math.Floor(trueCount) : wholeTrueCount + 0.5 * halves;

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
            } while (stopwatch.ElapsedMilliseconds < 300000);

            Console.WriteLine("Your score was {0}", score);

            stopwatch.Stop();
            stopwatch.Reset();

            if (onlyIntegers)
            {
                AddToHighscores(Directory.GetCurrentDirectory() + @"\Highscores\IntegerTrueCountConversion.txt", score);
            }
            else
            {
                AddToHighscores(Directory.GetCurrentDirectory() + @"\Highscores\TrueCountConversion.txt", score);
            }

            Console.WriteLine();
            Console.WriteLine("Press any key...");
            Console.ReadKey(true);
            Console.Clear();
        }
    }
}
