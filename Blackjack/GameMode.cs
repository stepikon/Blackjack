using System;
using System.Collections.Generic;
using System.Text;

namespace Blackjack
{
    //part of a <Strategy pattern> (and <Practice Strategy pattern>)
    //STRUKTURU STRATEGY PATTERNU JSEM PREVZAL Z https://refactoring.guru/design-patterns/strategy/csharp/example

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


        public IPlayable GetGamemode()
        {
            return mode;
        }


        public void Run()
        {
            mode.Run();
        }
    }

    //</Strategy pattern> (</Practice Strategy pattern>)
}
