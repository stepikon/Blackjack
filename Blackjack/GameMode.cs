using System;
using System.Collections.Generic;
using System.Text;

namespace Blackjack
{
    class GameMode : IPlayable
    {
        private IPlayable mode;

        public GameMode(IPlayable mode)
        {
            this.mode = mode;
        }

        public void Run()
        {
            mode.Run();
        }
    }
}
