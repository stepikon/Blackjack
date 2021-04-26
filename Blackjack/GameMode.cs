using System;
using System.Collections.Generic;
using System.Text;

namespace Blackjack
{
    class GameMode
    {
        private IPlayable mode;

        public GameMode()
        {
        }

        public GameMode(IPlayable mode)
        {
            this.mode = mode;
        }

        public void SetGamemode(IPlayable mode)
        {
            this.mode = mode;
        }

        public void Run()
        {
            mode.Run();
        }
    }
}
