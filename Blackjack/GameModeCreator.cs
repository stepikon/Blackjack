using System;
using System.Collections.Generic;
using System.Text;

namespace Blackjack
{
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
}
