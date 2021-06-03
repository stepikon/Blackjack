using System;
using System.Collections.Generic;
using System.Text;

namespace Blackjack
{
    abstract class SimulationCreator : GameModeCreator
    {
        protected string[] stringOptions = new string[] {
        "Number of decks: ",
        "Dealer hits soft 17: ",
        "Allow surrender: ",
        "Allow double after split: ",
        "Allow resplit: ",
        "--Allow resplit aces: ",
        "Number of AIs: ",
        "Make dealer and AIs visible: ",
        "AI leaves when true count is -1 or lower: "};

        protected string[] numberOfDecks = new string[] { "4", "5", "6", "7", "8" };
        protected string[] dealerHitsSoft17 = new string[] { "yes", "no" };
        protected string[] allowSurrender = new string[] { "yes", "no" };
        protected string[] allowDAS = new string[] { "yes", "no" };
        protected string[] allowResplit = new string[] { "yes", "no" };
        protected string[] allowResplitAces = new string[] { "yes", "no" };
        protected string[] numberOfAIs = new string[] { "1", "2", "3", "4", "5", "6", "7" };
        protected string[] isVisible = new string[] { "yes", "no" };
        protected string[] AILeaves = new string[] { "yes", "no" };

        protected string[] names = new string[7];
        protected int[] chips = new int[7];
        protected int[] betUnits = new int[7];
        protected int[] betSpreadMultipliers = new int[7];


        public SimulationCreator(BetterUI betterUI, Random random)
            : base(betterUI, random)
        {
        }


        protected void DoSpecifyAIsAlgorithm(int numberOfAIs, Tuple<int, int>tableLimits)
        {
            for (int i = 0; i < numberOfAIs; i++)
            {
                Console.WriteLine("Enter AI{0}'s name (1-25 characters)", i + 1);
                names[i] = betterUI.GetStringInput(1, 25, 1);

                Console.SetCursorPosition(0, 2);
                Console.Write(new String(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, 2);
                Console.WriteLine("Enter AI{0}'s chips", i + 1);
                chips[i] = betterUI.GetIntInput(3, 0);

                Console.SetCursorPosition(0, 4);
                Console.Write(new String(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, 4);
                Console.WriteLine("Enter AI{0}'s bet unit. Value must fall within the table limits.", i + 1);
                betUnits[i] = betterUI.GetIntInput(5, tableLimits.Item2, tableLimits.Item1);

                Console.SetCursorPosition(0, 6);
                Console.Write(new String(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, 7);
                Console.Write(new String(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, 6);
                Console.WriteLine("Enter AI{0}'s bet spread multiplier \n" +
                        "(bet spread will be 6*multiplier). Expecting integer from <1;5>", i + 1);
                betSpreadMultipliers[i] = betterUI.GetIntInput(8, 5, 1);

                Console.Clear();
            }
        }


        public abstract override IPlayable CreateGameMode();
    }
}
