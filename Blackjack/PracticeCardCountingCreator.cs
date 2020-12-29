using System;
using System.Collections.Generic;
using System.Text;

namespace Blackjack
{
    class PracticeCardCountingCreator : PracticeCreator
    {
        public PracticeCardCountingCreator(BetterUI betterUI, Random random)
            : base(betterUI, random)
        { }

        public override IPlayable CreateGameMode()
        {
            return new PracticeCardCounting(betterUI, random);
        }
    }
}
