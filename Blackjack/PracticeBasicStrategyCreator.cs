using System;
using System.Collections.Generic;
using System.Text;

namespace Blackjack
{
    class PracticeBasicStrategyCreator : PracticeCreator
    {
        public PracticeBasicStrategyCreator(BetterUI betterUI, Random random)
            : base(betterUI, random)
        { }

        public override IPlayable CreateGameMode()
        {
            return new PracticeBasicStrategy(betterUI, random);
        }
    }
}
