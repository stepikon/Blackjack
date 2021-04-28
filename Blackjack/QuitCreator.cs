using System;
using System.Collections.Generic;
using System.Text;

namespace Blackjack
{
    //part of a <Factory pattern>
    //STRUKTURU FACTORY PATTERNU JSEM PREVZAL Z https://www.dofactory.com/net/factory-method-design-pattern

    class QuitCreator : GameModeCreator
    {
        public QuitCreator(BetterUI betterUI, Random random)
            :base(betterUI, random)
        {
        }

        public override IPlayable CreateGameMode()
        {
            return new Quit();
        }
    }

    //</Factory pattern>
}
