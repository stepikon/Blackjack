using System;
using System.Collections.Generic;
using System.Text;

namespace Blackjack
{
    //this interface is a part of a <Strategy pattern>
    //STRUKTURU STRATEGY PATTERNU JSEM PREVZAL Z https://refactoring.guru/design-patterns/strategy/csharp/example

    interface IPlayable
    {
        public void Run();
    }

    //</Strategy pattern>
}
