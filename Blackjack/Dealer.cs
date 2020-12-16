using System;
using System.Collections.Generic;
using System.Text;

namespace Blackjack
{
    class Dealer:Character
    {
        int deckAmount;
        int deckPenetration;
        int cardToDeal;

        string name;

        CardFace[] shoe;
        public Card[] hand;

        private Card hiddenCard;

        Random random;

        public Dealer(string name, int deckAmount, Random random)
            :base(name)
        {
            this.name = name;
            this.deckAmount = deckAmount;
            this.random = random;

            shoe = new CardFace[52*deckAmount];
        }

        public string Name { get; }

        public void BuildShoe()
        {
            string name = "", suit="", color = "";
            ConsoleColor consoleColor = 0;

            for (int i = 0; i < deckAmount; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    for (int k = 0; k < 13; k++)
                    {
                        switch (j)
                        {
                            case 0:
                                suit = "Hearts";
                                color = "Red";
                                consoleColor = ConsoleColor.Red;
                                break;
                            case 1:
                                suit = "Diamonds";
                                color = "Red";
                                consoleColor = ConsoleColor.Magenta;
                                break;
                            case 2:
                                suit = "Clubs";
                                color = "Black";
                                consoleColor = ConsoleColor.DarkGray;
                                break;
                            case 3:
                                suit = "Spades";
                                color = "Black";
                                consoleColor = ConsoleColor.Black;
                                break;
                            default:
                                Console.WriteLine("Never Happens");
                                break;
                        }

                        switch (k)
                        {
                            case 0:
                                name = "K";
                                shoe[i * 52 + j * 13 + k] = new CardKing(name, suit, color, consoleColor);
                                break;
                            case 1:
                                name = "A";
                                shoe[i * 52 + j * 13 + k] = new CardAce(name, suit, color, consoleColor, false);
                                break;
                            case 2:
                                name = k.ToString();
                                shoe[i * 52 + j * 13 + k] = new CardTwo(name, suit, color, consoleColor);
                                break;
                            case 3:
                                name = k.ToString();
                                shoe[i * 52 + j * 13 + k] = new CardThree(name, suit, color, consoleColor);
                                break;
                            case 4:
                                name = k.ToString();
                                shoe[i * 52 + j * 13 + k] = new CardFour(name, suit, color, consoleColor);
                                break;
                            case 5:
                                name = k.ToString();
                                shoe[i * 52 + j * 13 + k] = new CardFive(name, suit, color, consoleColor);
                                break;
                            case 6:
                                name = k.ToString();
                                shoe[i * 52 + j * 13 + k] = new CardSix(name, suit, color, consoleColor);
                                break;
                            case 7:
                                name = k.ToString();
                                shoe[i * 52 + j * 13 + k] = new CardSeven(name, suit, color, consoleColor);
                                break;
                            case 8:
                                name = k.ToString();
                                shoe[i * 52 + j * 13 + k] = new CardEight(name, suit, color, consoleColor);
                                break;
                            case 9:
                                name = k.ToString();
                                shoe[i * 52 + j * 13 + k] = new CardNine(name, suit, color, consoleColor);
                                break;
                            case 10:
                                name = k.ToString();
                                shoe[i * 52 + j * 13 + k] = new CardTen(name, suit, color, consoleColor);
                                break;
                            case 11:
                                name = "J";
                                shoe[i * 52 + j * 13 + k] = new CardJack(name, suit, color, consoleColor);
                                break;
                            case 12:
                                name = "Q";
                                shoe[i * 52 + j * 13 + k] = new CardQueen(name, suit, color, consoleColor);
                                break;
                            default:
                                Console.WriteLine("Never Happens");
                                break;
                        }
                    }
                }
            }
        }       

        public void Reset()
        {
            Shuffle();
            SetDeckPenetration();
            cardToDeal = 0;
        }

        public void SetDeckPenetration() //deck penetration is 75%-85%
        {
            deckPenetration = random.Next((int)3/4*deckAmount*52, 17/20*deckAmount*52);
        }

        public void Shuffle()
        {
            CardFace temp;
            int index;

            for (int i = 0; i < deckAmount*52; i++)
            {
                index = random.Next(deckAmount*52);

                temp = shoe[i];
                shoe[i] = shoe[index];
                shoe[index] = temp;
            }
        }

        public CardFace Deal()
        {
            CardFace card = shoe[cardToDeal];
            cardToDeal++;
            return card;
        }

        public void Display()
        {
            cardToDeal = 0;
            Console.BackgroundColor = ConsoleColor.White;
            CardFace card, card2;
            card2 = new CardAce("A", "Hearts", "red", ConsoleColor.Black, false);

            for (int i = 0; i < deckAmount*52; i++)
            {
                card = Deal();
                Console.ForegroundColor = card.C;
                Console.WriteLine(card.Name + card.Suit + card.Color + card.GetCardValue());
                //Console.WriteLine(card.GetType()==card2.GetType());
            }
        }
    }
}
