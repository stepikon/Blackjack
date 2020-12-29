using System;
using System.Collections.Generic;
using System.Text;

namespace Blackjack
{
    class PracticeCreator : GameModeCreator
    {
        public PracticeCreator(BetterUI betterUI, Random random)
            : base(betterUI, random)
        { }

        public override IPlayable CreateGameMode()
        {
            return new Practice(betterUI, random);
        }
    }
}
