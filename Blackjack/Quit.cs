using System;
using System.Collections.Generic;
using System.Text;

namespace Blackjack
{
    //part of a <Strategy pattern>
    //STRUKTURU STRATEGY PATTERNU JSEM PREVZAL Z https://refactoring.guru/design-patterns/strategy/csharp/example

    //terminates the program or quits the practice mode
    class Quit : IPlayable
    {
        public Quit()
        {
        }


        public void Run()
        {
            return;
        }
    }

    //</Strategy pattern>
}
