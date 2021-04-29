using System;
using System.Collections.Generic;
using System.Text;

namespace Blackjack
{
    //part of a <Practice Factory pattern>
    //STRUKTURU FACTORY PATTERNU JSEM PREVZAL Z https://www.dofactory.com/net/factory-method-design-pattern

    class PracticeCardCountingCreator : PracticeCreator
    {
        public PracticeCardCountingCreator(BetterUI betterUI, Random random)
            : base(betterUI, random)
        {
        }


        public override IPlayable CreateGameMode()
        {
            return new PracticeCardCounting(betterUI, random);
        }
    }

    //<Practice Factory pattern>
}
