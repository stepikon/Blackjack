using System;

namespace Blackjack
{
    class Program
    {
        static void Main(string[] args)
        {
            Dealer shoe = new Dealer(20, new Random());
            shoe.BuildShoe();
            shoe.Display();
            shoe.Shuffle();
            shoe.Display();
        }
    }
}
