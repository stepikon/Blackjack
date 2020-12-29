using System;
using System.Collections.Generic;
using System.Text;

namespace Blackjack
{
    class PracticeTrueCountConversionCreator : PracticeCreator
    {
        public PracticeTrueCountConversionCreator(BetterUI betterUI, Random random)
            : base(betterUI, random)
        { }

        public override IPlayable CreateGameMode()
        {
            return new PracticeTrueCountConversion(betterUI, random);
        }
    }
}
