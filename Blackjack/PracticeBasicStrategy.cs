using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace Blackjack
{
    class PracticeBasicStrategy : Practice
    {
        private bool hit17;
        private bool surrenderAllowed;
        private bool doubleAllowed;
        private bool DASAllowed;

        public PracticeBasicStrategy(BetterUI betterUI, Random random, bool hit17, bool surrenderAllowed, bool doubleAllowed, bool DASAllowed)
            : base (betterUI, random)
        {
            this.hit17 = hit17;
            this.surrenderAllowed = surrenderAllowed;
            this.doubleAllowed = doubleAllowed;
            this.DASAllowed = DASAllowed;
        }

        public override void Run()
        {
            betterUI.ClearAll();
            Console.Write("Enter the correct decision based on basic strategy.\n" +
                "Each correct answer is worth a point, the game ends after a wrong answer.\n" +
                "You will be told if you have a pair \n " + 
                "Press any key to start. Press q to quit.");

            if (Console.ReadKey().KeyChar == 'q')
            {
                return;
            }

            betterUI.ClearAll();

            Tuple<string, int>[] cards = new Tuple<string, int>[] {
            new Tuple<string, int>("2", 2),
            new Tuple<string, int>("3", 3),
            new Tuple<string, int>("4", 4),
            new Tuple<string, int>("5", 5),
            new Tuple<string, int>("6", 6),
            new Tuple<string, int>("7", 7),
            new Tuple<string, int>("8", 8),
            new Tuple<string, int>("9", 9),
            new Tuple<string, int>("10", 10),
            new Tuple<string, int>("J", 10),
            new Tuple<string, int>("Q", 10),
            new Tuple<string, int>("K", 10),
            new Tuple<string, int>("A", 11)
            };

            const string HIT = "hit";
            const string STAND = "stand";
            const string DOUBLE = "double";
            const string SPLIT = "split";
            const string SURRENDER = "surrender";

            string correctChoice;
            string playerChoice;
            Tuple<Tuple<string, int>, Tuple<string, int>> playerHand;
            Tuple<string, int> dealerCard;
            bool softTotal;
            bool isCorrect;
            int score = 0;

            do
            {
                playerHand = new Tuple<Tuple<string, int>, Tuple<string, int>>(cards[random.Next(13)], cards[random.Next(13)]);
                dealerCard = cards[random.Next(13)];
                softTotal = playerHand.Item1.Item2 == 11 || playerHand.Item2.Item2 == 11;

                //normal thought process is: 1) Can I surrender? yes or 2 2) Can I split? Yes or step 3. 3) Can I double? Yes or step 4. 4)Hit/Stand.
                //here the code goes "backwards", so hit/stand options may be overwritten by double or split or surrender.
                //basic strategy from https://wizardofodds.com/games/blackjack/strategy/4-decks/ (it's actually 4-8 decks)

                if (hit17)
                {
                    if (softTotal)
                    {
                        //hit / stand
                        if (playerHand.Item1.Item2 + playerHand.Item2.Item2 <= 17
                            || (playerHand.Item1.Item2 + playerHand.Item2.Item2 == 18 && dealerCard.Item2 >= 9))
                        {
                            correctChoice = HIT;
                        }
                        else
                        {
                            correctChoice = STAND;
                        }

                        //double
                        if (doubleAllowed)
                        {
                            if ((playerHand.Item1.Item2 + playerHand.Item2.Item2 == 19 && dealerCard.Item2 == 6)
                                || (playerHand.Item1.Item2 + playerHand.Item2.Item2 == 18 && dealerCard.Item2 >= 2 && dealerCard.Item2 <= 6)
                                || (playerHand.Item1.Item2 + playerHand.Item2.Item2 == 17 && dealerCard.Item2 >= 3 && dealerCard.Item2 <= 6)
                                || (playerHand.Item1.Item2 + playerHand.Item2.Item2 == 16 && dealerCard.Item2 >= 4 && dealerCard.Item2 <= 6)
                                || (playerHand.Item1.Item2 + playerHand.Item2.Item2 == 15 && dealerCard.Item2 >= 4 && dealerCard.Item2 <= 6)
                                || (playerHand.Item1.Item2 + playerHand.Item2.Item2 == 14 && dealerCard.Item2 >= 5 && dealerCard.Item2 <= 6)
                                || (playerHand.Item1.Item2 + playerHand.Item2.Item2 == 13 && dealerCard.Item2 >= 5 && dealerCard.Item2 <= 6))
                            {
                                correctChoice = DOUBLE;
                            }
                        }

                        //split, since hand contains A, you can olny split if hand is AA and you always split AA
                        if (playerHand.Item1.Item2 == playerHand.Item2.Item2)
                        {
                            correctChoice = SPLIT;
                        }
                    }
                    else //hard total
                    {
                        //hit/stand
                        if (playerHand.Item1.Item2 + playerHand.Item2.Item2 <= 11
                            || (playerHand.Item1.Item2 + playerHand.Item2.Item2 == 12 && !(dealerCard.Item2 >= 4 && dealerCard.Item2 <= 6))
                            || (playerHand.Item1.Item2 + playerHand.Item2.Item2 == 13 && dealerCard.Item2 >= 7 && dealerCard.Item2 <= 11)
                            || (playerHand.Item1.Item2 + playerHand.Item2.Item2 == 14 && dealerCard.Item2 >= 7 && dealerCard.Item2 <= 11)
                            || (playerHand.Item1.Item2 + playerHand.Item2.Item2 == 15 && dealerCard.Item2 >= 7 && dealerCard.Item2 <= 11)
                            || (playerHand.Item1.Item2 + playerHand.Item2.Item2 == 16 && dealerCard.Item2 >= 7 && dealerCard.Item2 <= 11))
                        {
                            correctChoice = HIT;
                        }
                        else
                        {
                            correctChoice = STAND;
                        }

                        //double
                        if (doubleAllowed)
                        {
                            if ((playerHand.Item1.Item2 + playerHand.Item2.Item2 == 9 && dealerCard.Item2 >= 3 && dealerCard.Item2 <= 6)
                                || (playerHand.Item1.Item2 + playerHand.Item2.Item2 == 10 && dealerCard.Item2 >= 2 && dealerCard.Item2 <= 9)
                                || (playerHand.Item1.Item2 + playerHand.Item2.Item2 == 11))
                            {
                                correctChoice = DOUBLE;
                            }
                        }

                        //split
                        if (playerHand.Item1.Item2 == playerHand.Item2.Item2)
                        {
                            if (DASAllowed)
                            {
                                if ((playerHand.Item1.Item2 == 2 && dealerCard.Item2 >= 2 && dealerCard.Item2 <= 7)
                                    || (playerHand.Item1.Item2 == 3 && dealerCard.Item2 >= 2 && dealerCard.Item2 <= 7)
                                    || (playerHand.Item1.Item2 == 4 && dealerCard.Item2 >= 5 && dealerCard.Item2 <= 6)
                                    || (playerHand.Item1.Item2 == 6 && dealerCard.Item2 >= 2 && dealerCard.Item2 <= 6)
                                    || (playerHand.Item1.Item2 == 7 && dealerCard.Item2 >= 2 && dealerCard.Item2 <= 7)
                                    || (playerHand.Item1.Item2 == 8)
                                    || (playerHand.Item1.Item2 == 9 && dealerCard.Item2 != 7 && dealerCard.Item2 != 10 && dealerCard.Item2 != 11))
                                {
                                    correctChoice = SPLIT;
                                }
                            }
                            else
                            {
                                if ((playerHand.Item1.Item2 == 2 && dealerCard.Item2 >= 4 && dealerCard.Item2 <= 7)
                                    || (playerHand.Item1.Item2 == 3 && dealerCard.Item2 >= 4 && dealerCard.Item2 <= 7)
                                    || (playerHand.Item1.Item2 == 6 && dealerCard.Item2 >= 3 && dealerCard.Item2 <= 6)
                                    || (playerHand.Item1.Item2 == 7 && dealerCard.Item2 >= 2 && dealerCard.Item2 <= 7)
                                    || (playerHand.Item1.Item2 == 8)
                                    || (playerHand.Item1.Item2 == 9 && dealerCard.Item2 != 7 && dealerCard.Item2 != 10 && dealerCard.Item2 != 11))
                                {
                                    correctChoice = SPLIT;
                                }
                            }
                        }

                        //surrender
                        if (surrenderAllowed)
                        {
                            if (!softTotal)
                            {
                                if ((playerHand.Item1.Item2 + playerHand.Item2.Item2 == 15 && dealerCard.Item2 >= 10 && dealerCard.Item2 <= 11)
                                    || (playerHand.Item1.Item2 + playerHand.Item2.Item2 == 16 && dealerCard.Item2 >= 9 && dealerCard.Item2 <= 11)
                                    || (playerHand.Item1.Item2 + playerHand.Item2.Item2 == 17 && dealerCard.Item2 == 11)
                                    || (playerHand.Item1.Item2 == 8 && playerHand.Item2.Item2 == 8 && dealerCard.Item2 == 11)) //88 vs A
                                {
                                    correctChoice = SURRENDER;

                                    if (playerHand.Item1.Item2 == 8 && playerHand.Item2.Item2 == 8 && dealerCard.Item2 != 11)
                                    {
                                        correctChoice = SPLIT;
                                    }
                                }
                            }
                        }
                    }
                }
                else //stand 17
                {
                    if (softTotal)
                    {
                        //hit / stand
                        if (playerHand.Item1.Item2 + playerHand.Item2.Item2 <= 17
                            || (playerHand.Item1.Item2 + playerHand.Item2.Item2 == 18 && dealerCard.Item2 >= 9))
                        {
                            correctChoice = HIT;
                        }
                        else
                        {
                            correctChoice = STAND;
                        }

                        //double
                        if (doubleAllowed)
                        {
                            if ((playerHand.Item1.Item2 + playerHand.Item2.Item2 == 18 && dealerCard.Item2 >= 3 && dealerCard.Item2 <= 6)
                                || (playerHand.Item1.Item2 + playerHand.Item2.Item2 == 17 && dealerCard.Item2 >= 3 && dealerCard.Item2 <= 6)
                                || (playerHand.Item1.Item2 + playerHand.Item2.Item2 == 16 && dealerCard.Item2 >= 4 && dealerCard.Item2 <= 6)
                                || (playerHand.Item1.Item2 + playerHand.Item2.Item2 == 15 && dealerCard.Item2 >= 4 && dealerCard.Item2 <= 6)
                                || (playerHand.Item1.Item2 + playerHand.Item2.Item2 == 14 && dealerCard.Item2 >= 5 && dealerCard.Item2 <= 6)
                                || (playerHand.Item1.Item2 + playerHand.Item2.Item2 == 13 && dealerCard.Item2 >= 5 && dealerCard.Item2 <= 6))
                            {
                                correctChoice = DOUBLE;
                            }
                        }

                        //split, since hand contains A, you can olny split if hand is AA, which you always split
                        if (playerHand.Item1.Item2 == playerHand.Item2.Item2)
                        {
                            correctChoice = SPLIT;
                        }
                    }
                    else //hard total
                    {
                        //hit/stand
                        if (playerHand.Item1.Item2 + playerHand.Item2.Item2 <= 11
                            || (playerHand.Item1.Item2 + playerHand.Item2.Item2 == 12 && !(dealerCard.Item2 >= 4 && dealerCard.Item2 <= 6))
                            || (playerHand.Item1.Item2 + playerHand.Item2.Item2 == 13 && dealerCard.Item2 >= 7 && dealerCard.Item2 <= 11)
                            || (playerHand.Item1.Item2 + playerHand.Item2.Item2 == 14 && dealerCard.Item2 >= 7 && dealerCard.Item2 <= 11)
                            || (playerHand.Item1.Item2 + playerHand.Item2.Item2 == 15 && dealerCard.Item2 >= 7 && dealerCard.Item2 <= 11)
                            || (playerHand.Item1.Item2 + playerHand.Item2.Item2 == 16 && dealerCard.Item2 >= 7 && dealerCard.Item2 <= 11))
                        {
                            correctChoice = HIT;
                        }
                        else
                        {
                            correctChoice = STAND;
                        }

                        //double
                        if (doubleAllowed)
                        {
                            if ((playerHand.Item1.Item2 + playerHand.Item2.Item2 == 9 && dealerCard.Item2 >= 3 && dealerCard.Item2 <= 6)
                                || (playerHand.Item1.Item2 + playerHand.Item2.Item2 == 10 && dealerCard.Item2 >= 2 && dealerCard.Item2 <= 9)
                                || (playerHand.Item1.Item2 + playerHand.Item2.Item2 == 11 && dealerCard.Item2 != 11))
                            {
                                correctChoice = DOUBLE;
                            }
                        }

                        //split
                        if (playerHand.Item1.Item2 == playerHand.Item2.Item2)
                        {
                            if (DASAllowed)
                            {
                                if ((playerHand.Item1.Item2 == 2 && dealerCard.Item2 >= 2 && dealerCard.Item2 <= 7)
                                    || (playerHand.Item1.Item2 == 3 && dealerCard.Item2 >= 2 && dealerCard.Item2 <= 7)
                                    || (playerHand.Item1.Item2 == 4 && dealerCard.Item2 >= 5 && dealerCard.Item2 <= 6)
                                    || (playerHand.Item1.Item2 == 6 && dealerCard.Item2 >= 2 && dealerCard.Item2 <= 6)
                                    || (playerHand.Item1.Item2 == 7 && dealerCard.Item2 >= 2 && dealerCard.Item2 <= 7)
                                    || (playerHand.Item1.Item2 == 8)
                                    || (playerHand.Item1.Item2 == 9 && dealerCard.Item2 != 7 && dealerCard.Item2 != 10 && dealerCard.Item2 != 11))
                                {
                                    correctChoice = SPLIT;
                                }
                            }
                            else
                            {
                                if ((playerHand.Item1.Item2 == 2 && dealerCard.Item2 >= 4 && dealerCard.Item2 <= 7)
                                    || (playerHand.Item1.Item2 == 3 && dealerCard.Item2 >= 4 && dealerCard.Item2 <= 7)
                                    || (playerHand.Item1.Item2 == 6 && dealerCard.Item2 >= 3 && dealerCard.Item2 <= 6)
                                    || (playerHand.Item1.Item2 == 7 && dealerCard.Item2 >= 2 && dealerCard.Item2 <= 7)
                                    || (playerHand.Item1.Item2 == 8)
                                    || (playerHand.Item1.Item2 == 9 && dealerCard.Item2 != 7 && dealerCard.Item2 != 10 && dealerCard.Item2 != 11))
                                {
                                    correctChoice = SPLIT;
                                }
                            }
                        }

                        //surrender
                        if (surrenderAllowed)
                        {
                            if (!softTotal)
                            {
                                if ((playerHand.Item1.Item2 + playerHand.Item2.Item2 == 15 && dealerCard.Item2 == 10)
                                    || (playerHand.Item1.Item2 + playerHand.Item2.Item2 == 16 && dealerCard.Item2 >= 9 && dealerCard.Item2 <= 11))
                                {
                                    correctChoice = SURRENDER;

                                    if (playerHand.Item1.Item2 == 8 && playerHand.Item2.Item2 == 8)
                                    {
                                        correctChoice = SPLIT;
                                    }
                                }
                            }
                        }
                    }
                }

                if (softTotal)
                {
                    if (playerHand.Item1.Item2 == playerHand.Item2.Item2) //only AA pairs. Soft total of AA is 12
                    {
                        if (surrenderAllowed)
                        {
                            if (doubleAllowed)
                            {
                                playerChoice = GetChoice(
                                    String.Format("Soft total: {0} (pair {1} {1}), dealer shows: {2}",
                                    12, playerHand.Item1.Item1, dealerCard.Item1),
                                    new string[] { HIT, STAND, DOUBLE, SPLIT, SURRENDER },
                                    ConsoleColor.Cyan);
                            }
                            else
                            {
                                playerChoice = GetChoice(
                                    String.Format("Soft total: {0} (pair {1} {1}), dealer shows: {2}",
                                    12, playerHand.Item1.Item1, dealerCard.Item1),
                                    new string[] { HIT, STAND, SPLIT, SURRENDER },
                                    ConsoleColor.Cyan);
                            }
                        }
                        else
                        {
                            if (doubleAllowed)
                            {
                                playerChoice = GetChoice(
                                    String.Format("Soft total: {0} (pair {1} {1}), dealer shows: {2}",
                                    12, playerHand.Item1.Item1, dealerCard.Item1),
                                    new string[] { HIT, STAND, DOUBLE, SPLIT },
                                    ConsoleColor.Cyan);
                            }
                            else
                            {
                                playerChoice = GetChoice(
                                    String.Format("Soft total: {0} (pair {1} {1}), dealer shows: {2}",
                                    12, playerHand.Item1.Item1, dealerCard.Item1),
                                    new string[] { HIT, STAND, SPLIT },
                                    ConsoleColor.Cyan);
                            }
                        }                       
                    }
                    else
                    {
                        if (surrenderAllowed)
                        {
                            if (doubleAllowed)
                            {
                                playerChoice = GetChoice(
                                    String.Format("Soft total: {0}, dealer shows: {1}",
                                    playerHand.Item1.Item2 + playerHand.Item2.Item2, dealerCard.Item1),
                                    new string[] { HIT, STAND, DOUBLE, SURRENDER },
                                    ConsoleColor.Cyan);
                            }
                            else
                            {
                                playerChoice = GetChoice(
                                    String.Format("Soft total: {0}, dealer shows: {1}",
                                    playerHand.Item1.Item2 + playerHand.Item2.Item2, dealerCard.Item1),
                                    new string[] { HIT, STAND, SURRENDER },
                                    ConsoleColor.Cyan);
                            }
                        }
                        else
                        {
                            if (doubleAllowed)
                            {
                                playerChoice = GetChoice(
                                    String.Format("Soft total: {0}, dealer shows: {1}",
                                    playerHand.Item1.Item2 + playerHand.Item2.Item2, dealerCard.Item1),
                                    new string[] { HIT, STAND, DOUBLE },
                                    ConsoleColor.Cyan);

                            }
                            else
                            {
                                playerChoice = GetChoice(
                                    String.Format("Soft total: {0}, dealer shows: {1}",
                                    playerHand.Item1.Item2 + playerHand.Item2.Item2, dealerCard.Item1),
                                    new string[] { HIT, STAND },
                                    ConsoleColor.Cyan);
                            }
                        }
                    }
                }
                else
                {
                    if (playerHand.Item1.Item2 == playerHand.Item2.Item2)
                    {
                        if (surrenderAllowed)
                        {
                            if (doubleAllowed)
                            {
                                playerChoice = GetChoice(
                                    String.Format("Hard total: {0} (pair {1} {2}), dealer shows: {3}",
                                    playerHand.Item1.Item2 + playerHand.Item2.Item2, playerHand.Item1.Item1, playerHand.Item2.Item1, dealerCard.Item1),
                                    new string[] { HIT, STAND, DOUBLE, SPLIT, SURRENDER },
                                    ConsoleColor.Red);
                            }
                            else
                            {
                                playerChoice = GetChoice(
                                    String.Format("Hard total: {0} (pair {1} {2}), dealer shows: {3}",
                                    playerHand.Item1.Item2 + playerHand.Item2.Item2, playerHand.Item1.Item1, playerHand.Item2.Item1, dealerCard.Item1),
                                    new string[] { HIT, STAND, SPLIT, SURRENDER },
                                    ConsoleColor.Red);
                            }
                        }
                        else
                        {
                            if (doubleAllowed)
                            {
                                playerChoice = GetChoice(
                                    String.Format("Hard total: {0} (pair {1} {2}), dealer shows: {3}",
                                    playerHand.Item1.Item2 + playerHand.Item2.Item2, playerHand.Item1.Item1, playerHand.Item2.Item1, dealerCard.Item1),
                                    new string[] { HIT, STAND, DOUBLE, SPLIT },
                                    ConsoleColor.Red);
                            }
                            else
                            {
                                playerChoice = GetChoice(
                                    String.Format("Hard total: {0} (pair {1} {2}), dealer shows: {3}",
                                    playerHand.Item1.Item2 + playerHand.Item2.Item2, playerHand.Item1.Item1, playerHand.Item2.Item1, dealerCard.Item1),
                                    new string[] { HIT, STAND, SPLIT },
                                    ConsoleColor.Red);
                            }
                        }
                    }
                    else
                    {
                        if (surrenderAllowed)
                        {
                            if (doubleAllowed)
                            {
                                playerChoice = GetChoice(
                                    String.Format("Hard total: {0}, dealer shows: {1}",
                                    playerHand.Item1.Item2 + playerHand.Item2.Item2, dealerCard.Item1),
                                    new string[] { HIT, STAND, DOUBLE, SURRENDER },
                                    ConsoleColor.Red);
                            }
                            else
                            {
                                playerChoice = GetChoice(
                                    String.Format("Hard total: {0}, dealer shows: {1}",
                                    playerHand.Item1.Item2 + playerHand.Item2.Item2, dealerCard.Item1),
                                    new string[] { HIT, STAND, SURRENDER },
                                    ConsoleColor.Red);
                            }
                        }
                        else
                        {
                            if (doubleAllowed)
                            {
                                playerChoice = GetChoice(
                                    String.Format("Hard total: {0}, dealer shows: {1}",
                                    playerHand.Item1.Item2 + playerHand.Item2.Item2, dealerCard.Item1),
                                    new string[] { HIT, STAND, DOUBLE },
                                    ConsoleColor.Red);
                            }
                            else
                            {
                                playerChoice = GetChoice(
                                    String.Format("Hard total: {0}, dealer shows: {1}",
                                    playerHand.Item1.Item2 + playerHand.Item2.Item2, dealerCard.Item1),
                                    new string[] { HIT, STAND },
                                    ConsoleColor.Red);
                            }
                        }
                    }
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
            } while (isCorrect && Console.ReadKey(true).KeyChar != 'q');

            Console.WriteLine("Your score was {0}", score);
            AddToHighscores(Directory.GetCurrentDirectory() + @"\Highscores\BasicStrategy.txt", score);

            Console.WriteLine();
            Console.WriteLine("Press any key...");
            Console.ReadKey(true);
            Console.Clear();
        }
    }
}
