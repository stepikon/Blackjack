using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace Blackjack
{
    class Game : IPlayable
    {
        //to display everything in a more fancy way
        private const int MINIMUM_WINDIW_WIDTH = 7 * 25;
        private const int MINIMUM_WINDIW_HEIGHT = 38;

        Dealer dealer;
        Tuple<int, int> tableLimits;
        Player[] players;
        BetterUI betterUI;
        Random random;
        private bool practice;

        public Game(Dealer dealer, Tuple<int, int> tableLimits, Player[] players, Random random, BetterUI betterUI, bool practice)
        {
            this.dealer = dealer;
            this.tableLimits = tableLimits;
            this.players = players;
            this.random = random;
            this.betterUI = betterUI;
            this.practice = practice;
        }

        public void Run()
        {
            dealer.CreateShoe();
            dealer.Shuffle();
            dealer.Reset();
            bool dealerSkips; //skips dealer turn. Dealer skips his turn if all players are over 21, all have blackjack, all surrendered or some combination of those situations

            //initial check
            foreach (Player p in players)
            {
                if (p!=null)
                {
                    p.IsRuined = p.Chips < tableLimits.Item1;
                }
            }

            do
            {
                dealerSkips = true;
                betterUI.ClearAll();

                if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                {
                    betterUI.DisplayTableRules(dealer.HitSoft17);
                    betterUI.DisplayLimits(tableLimits);
                }

                //Betting
                foreach (Player p in players)
                {
                    if (!(p == null || p.IsRuined || p.IsGone))
                    {
                        p.Bet(p.hands[0], tableLimits);

                        if (!p.IsGone)
                        {
                            p.BetPair(tableLimits);
                        }
                    }
                }
                if (Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH)
                {
                    Console.WriteLine("-----");
                }

                //Dealing
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < players.Length; j++)
                    {
                        if (!(players[j] == null || players[j].IsRuined || players[j].IsGone))
                        {
                            dealer.Deal(players[j], 0);
                        }
                    }

                    if (i == 0)
                    {
                        dealer.Deal(dealer, 0);
                    }
                    else
                    {
                        dealer.DealHidden();
                    }
                }

                //displays statuses
                if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                {
                    betterUI.DisplayPlayersStatus(players);
                    betterUI.DisplayDealerStatus(dealer);
                }

                //Pair bonus gets paid always first
                foreach (Player p in players)
                {
                    if (!(p == null || p.IsRuined || p.IsGone))
                    {
                        if (p.PairBet != 0)
                        {
                            if (p.hand[0].GetType() == p.hand[1].GetType()
                                && p.hand[0].Color == p.hand[1].Color
                                && p.hand[0].Suit == p.hand[1].Suit)
                            {
                                if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                                {
                                    betterUI.DisplayPairs(players, p, "Perfect pair.");
                                }
                                else
                                {
                                    Console.WriteLine("{0}: Perfect pair.", p.Name);
                                }
                                p.Chips += p.PairBet * 26;
                            }
                            else if (p.hand[0].GetType() == p.hand[1].GetType()
                                && p.hand[0].Color == p.hand[1].Color)
                            {
                                if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                                {
                                    betterUI.DisplayPairs(players, p, "Color pair.");
                                }
                                else
                                {
                                    Console.WriteLine("{0}: Color pair.", p.Name);
                                }
                                p.Chips += p.PairBet * 13;
                            }
                            else if (p.hand[0].GetType() == p.hand[1].GetType())
                            {
                                if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                                {
                                    betterUI.DisplayPairs(players, p, "Pair.");
                                }
                                else
                                {
                                    Console.WriteLine("{0}: Pair.", p.Name);
                                }
                                p.Chips += p.PairBet * 7;
                            }
                            else
                            {
                                if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                                {
                                    betterUI.DisplayPairs(players, p, "Not a pair.");
                                }
                                else
                                {
                                    Console.WriteLine("{0}: Not a pair.", p.Name);
                                }
                            }
                        }
                    }                   
                }

                //checks blackjacks
                foreach (Player p in players)
                {
                    if (!(p == null || p.IsRuined || p.IsGone))
                    {
                        p.SetHasBlackjack();
                    }
                }
                dealer.SetHasBlackjack();

                //Displays hands
                foreach (Player p in players)
                {
                    if (!(p == null || p.IsRuined || p.IsGone))
                    {
                        if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                        {
                            betterUI.DisplayPlayersStatus(players);
                        }
                        else
                        {
                            p.DisplayHands();
                        }
                    }
                }
                if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                {
                    betterUI.DisplayDealerStatus(dealer);
                }
                else
                {
                    Console.WriteLine();
                    dealer.DisplayHand();
                    Console.WriteLine("-----");
                }

                //offers insurance if dealer shows an Ace
                if (dealer.hand[0] is CardAce)
                {
                    foreach (Player p in players)
                    {
                        if (!(p == null || p.IsRuined || p.IsGone))
                        {
                            p.CountDealt(players, dealer.hand, dealer.DeckAmount - dealer.GetDecksInDiscard());
                            p.BetInsurance();
                        }

                        if (Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH)
                        {
                            Console.WriteLine("-----");
                        }
                    }
                }

                //displays blackjacks
                if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                {
                    betterUI.DisplayPlayersBlackjack(players);
                    betterUI.DisplayDealerBlackjack(dealer);
                }

                //players turns
                if (!dealer.HasBlackjack)
                {
                    if (dealer.hand[0] is CardAce)
                    {
                        //displays blackjacks
                        if (Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH)
                        {
                            Console.WriteLine("Nobody home");
                        }
                    }

                    foreach (Player p in players)
                    {
                        if (!(p == null || p.IsRuined || p.IsGone))
                        {
                            p.CountDealt(players,dealer.hand, dealer.DeckAmount - dealer.GetDecksInDiscard());

                            if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                            {
                                betterUI.ClearMessages();
                            }

                            p.TakeTurn(players, dealer);
                        }

                        if (Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH)
                        {
                            Console.WriteLine("-----");
                        }
                    }
                }

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
                foreach (Player p in players)
                {
                    if (!(p == null || p.IsRuined || p.IsGone))
                    {
                        foreach (int handValue in p.GetHandValues())
                        {
                            dealerSkips = dealerSkips && (p.HasBlackjack || handValue > 21 || p.Surrender);
                        }
                    }
                }

                //Dealers turn
                if (!dealerSkips)
                {
                    if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                    {
                        betterUI.DisplayTurn(dealer.Name);
                    }
                    else
                    {
                        Console.WriteLine("It's {0}'s turn.", dealer.Name);
                    }

                    dealer.TakeTurn();
                }

                if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                {
                    betterUI.DisplayDealerStatus(dealer);
                    betterUI.ClearTurn();
                    betterUI.ClearOptionsSpace();
                }
                else
                {
                    dealer.DisplayHand();
                    Console.WriteLine("-----");
                }

                //outcomes
                if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                {
                    betterUI.ClearMessages();
                }

                foreach (Player p in players)
                {
                    if (!(p == null || p.IsRuined || p.IsGone))
                    {
                        for (int i = 0; i < p.GetHandValues().Count; i++)
                        {
                            if (p.Surrender)
                            {
                                Surrender(p, i);
                            }
                            else if ((p.GetHandValues()[i] > dealer.GetHandValue(dealer.hand).Item2
                                || dealer.GetHandValue(dealer.hand).Item2 > 21
                                || (p.HasBlackjack && !dealer.HasBlackjack))
                                && p.GetHandValues()[i] <= 21)
                            {
                                Win(p, i);
                            }
                            else if (p.GetHandValues()[i] < dealer.GetHandValue(dealer.hand).Item2
                                || p.GetHandValues()[i] > 21
                                || (!p.HasBlackjack && dealer.HasBlackjack))
                            {
                                Lose(p, i);
                            }
                            else
                            {
                                Push(p, i);
                            }
                        }

                        if (dealer.HasBlackjack && p.Insurance != 0)
                        {
                            WinInsurance(p, p.Insurance);
                        }
                    }                   
                }

                if (!dealer.HasBlackjack)
                {
                    if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                    {
                        //betterUI.DisplayMessage(String.Format("Dealer has {0}", dealer.GetHandValue(dealer.hand)));
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine(dealer.GetHandValue(dealer.hand).Item2);
                        Console.WriteLine("----");
                    }
                }

                if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                {
                    betterUI.DisplayDealerStatus(dealer);
                    betterUI.DisplayPlayersStatus(players);
                }


                //updates counts
                foreach (Player p in players)
                {
                    if (p!=null)
                    {
                        p.UpdateRunningCount(players, dealer.hand);
                        p.UpdateTrueCount(dealer.DeckAmount - dealer.GetDecksInDiscard());

                        if (practice)
                        {
                            Console.SetCursorPosition(0, 0);
                            Console.WriteLine("PRACTICE: RC{0}, TC{1}", p.RunningCount, p.TrueCount);
                        }
                    }
                }

                //Reset
                foreach (Player p in players)
                {
                    if (p!=null)
                    {
                        p.ResetHands();
                    }
                }
                dealer.ResetHand();

                //checks if anyone is ruined
                foreach (Player p in players)
                {
                    if (p!=null)
                    {
                        if (p.Chips < tableLimits.Item1)
                        {
                            p.IsRuined = true;
                        }
                    }                    
                }

                //Shuffle if necessary
                if (dealer.CardToDeal >= dealer.DeckPenetration)
                {
                    dealer.Reset();

                    foreach (Player p in players)
                    {
                        if (p!=null)
                        {
                            p.ResetCounts();
                        }
                    }

                    if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.SetCursorPosition(0, 36);
                        Console.WriteLine("Shuffling.");
                    }
                    else
                    {
                        Console.WriteLine("Shuffling");
                    }
                }

                if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.SetCursorPosition(0,37);
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
                    Console.WriteLine("{0} went from {1} chips and ended with {2} chips (with {3} % of original chips)"
                        , p.Name, p.OriginalChips, p.Chips, (double)100 * p.Chips / p.OriginalChips);
                }               
            }

            //highscores
            foreach (Player p in players)
            {
                if (p!=null)
                {
                    if (p is HumanPlayer)
                    {
                        AddToHighscores(Directory.GetCurrentDirectory() + @"\Highscores\AbsoluteChipAmount.txt", p.Name, p.Chips);
                        AddToHighscores(Directory.GetCurrentDirectory() + @"\Highscores\RelativeChipAmount.txt", p.Name, (double)p.Chips / p.OriginalChips);
                    }
                }
            }
        }

        private void Surrender(Player player, int handIndex)
        {
            player.Chips += 0.5 * player.GetBet(handIndex);

            if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
            {
                betterUI.DisplayOutcomes("SURRENDER", handIndex, player.GetHandValues()[handIndex], players, player);
            }
            else
            {
                Console.WriteLine("You surrender, returning {0} chips, you now have {1} chips.",
                0.5 * player.GetBet(handIndex), player.Chips);
            }
        }

        private void Win(Player player, int handIndex)
        {
            player.Chips += player.HasBlackjack ? player.GetBet(handIndex) * 2.5 : player.GetBet(handIndex) * 2;

            if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
            {
                betterUI.DisplayOutcomes("WIN", handIndex, player.GetHandValues()[handIndex], players, player);
            }
            else
            {
                Console.WriteLine("You won {0} chips, you now have {1} chips.",
                player.HasBlackjack ? player.GetBet(handIndex) * 2.5 : player.GetBet(handIndex) * 2,
                player.Chips);
            }           
        }

        private void Lose(Player player, int handIndex)
        {
            if (dealer.HasBlackjack)
            {
                if (Console.WindowHeight < MINIMUM_WINDIW_HEIGHT || Console.WindowWidth < MINIMUM_WINDIW_WIDTH)
                {
                    Console.WriteLine("Dealer has blackjack");
                }               
            }

            if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
            {
                betterUI.DisplayOutcomes("LOSS", handIndex, player.GetHandValues()[handIndex], players, player);
            }
            else
            {
                Console.WriteLine("You lost, you now have {0} chips.", player.Chips);
            }
        }

        private void Push(Player player, int handIndex)
        {
            player.Chips += player.GetBet(handIndex);

            if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
            {
                betterUI.DisplayOutcomes("PUSH", handIndex, player.GetHandValues()[handIndex], players, player);
            }
            else
            {
                Console.WriteLine("Push, {0} chips returned, you now have {1} chips.", player.GetBet(handIndex), player.Chips);
            }        
        }

        private void WinInsurance(Player player, double insurance)
        {            
            player.Chips += insurance * 3;

            if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
            {
                betterUI.DisplayOutcomes("INSURANCE WIN", 0, player.GetHandValues()[0], players, player);
            }
            else
            {
                Console.WriteLine("You get {0} chips from insurance", insurance * 3);
            }
        }

        private bool ExistActivePlayers()
        {
            bool existActivePlayers = false;

            foreach (Player p in players)
            {
                if (p!=null)
                {
                    existActivePlayers = existActivePlayers || !(p.IsRuined || p.IsGone);
                }
            }

            return existActivePlayers;
        }

        public void AddToHighscores(string path, string name, double score)
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
                            }
                            else
                            {
                                streamReader.ReadLine();
                            }

                            line = streamReader.ReadLine();
                        }
                    }

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
                                    throw new ArgumentException("Duplicit names");
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
                catch (Exception)
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
}
