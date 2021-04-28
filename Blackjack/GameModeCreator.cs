using System;
using System.Collections.Generic;
using System.Text;

namespace Blackjack
{
    //this abstract class is a part of a <Factory pattern>
    //STRUKTURU FACTORY PATTERNU JSEM PREVZAL Z https://www.dofactory.com/net/factory-method-design-pattern

    abstract class GameModeCreator
    {
        protected BetterUI betterUI;
        protected Random random;

        public GameModeCreator(BetterUI betterUI, Random random)
        {
            this.betterUI = betterUI;
            this.random = random;
        }

        public abstract IPlayable CreateGameMode();
    }

    //</Factory pattern>
}
