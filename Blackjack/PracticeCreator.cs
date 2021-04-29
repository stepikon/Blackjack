using System;
using System.Collections.Generic;
using System.Text;

namespace Blackjack
{
    //part of a <Factory pattern> (and <Practice Factory pattern>)
    //STRUKTURU FACTORY PATTERNU JSEM PREVZAL Z https://www.dofactory.com/net/factory-method-design-pattern

    class PracticeCreator : GameModeCreator
    {
        public PracticeCreator(BetterUI betterUI, Random random)
            : base(betterUI, random)
        {
        }


        public override IPlayable CreateGameMode()
        {
            return new Practice(betterUI, random);
        }
    }

    //</Factory pattern> (</Practice Factory pattern>)
}
