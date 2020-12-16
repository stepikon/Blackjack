using System;

namespace Blackjack
{
    class Program
    {
        static void Main(string[] args)
        {
            Dealer dealer = new Dealer("Dealer", new Hand(), 20, new Random());
            dealer.BuildShoe();
            dealer.Shuffle();

            Player[] players = new Player[1];
            players[0] = new HumanPlayer("human", new Hand());
            Game game = new Game(dealer, players, new Random());

            game.Run();
        }
    }
}
