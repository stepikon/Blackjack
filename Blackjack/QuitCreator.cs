using System;
using System.Collections.Generic;
using System.Text;

namespace Blackjack
{
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
}
