using System;
using System.Collections.Generic;
using System.Text;

namespace Blackjack
{
    class Dealer
    {
        int deckAmount;
        int deckPenetration;
        int cardToDeal;

        Card[] shoe;
        Card[] hand;

        Random random;

        public Dealer(int deckAmount, Random random)
        {
            this.deckAmount = deckAmount;
            this.random = random;

            shoe = new Card[52*deckAmount];
        }

        public void BuildShoe()
        {
            string rank = "", suit="", color = "";
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
                                rank = "K";
                                shoe[i * 52 + j * 13 + k] = new CardKing(rank, suit, color, consoleColor);
                                break;
                            case 1:
                                rank = "A";
                                shoe[i * 52 + j * 13 + k] = new CardAce(rank, suit, color, consoleColor, false);
                                break;
                            case 2:
                                rank = k.ToString();
                                shoe[i * 52 + j * 13 + k] = new CardTwo(rank, suit, color, consoleColor);
                                break;
                            case 3:
                                rank = k.ToString();
                                shoe[i * 52 + j * 13 + k] = new CardThree(rank, suit, color, consoleColor);
                                break;
                            case 4:
                                rank = k.ToString();
                                shoe[i * 52 + j * 13 + k] = new CardFour(rank, suit, color, consoleColor);
                                break;
                            case 5:
                                rank = k.ToString();
                                shoe[i * 52 + j * 13 + k] = new CardFive(rank, suit, color, consoleColor);
                                break;
                            case 6:
                                rank = k.ToString();
                                shoe[i * 52 + j * 13 + k] = new CardSix(rank, suit, color, consoleColor);
                                break;
                            case 7:
                                rank = k.ToString();
                                shoe[i * 52 + j * 13 + k] = new CardSeven(rank, suit, color, consoleColor);
                                break;
                            case 8:
                                rank = k.ToString();
                                shoe[i * 52 + j * 13 + k] = new CardEight(rank, suit, color, consoleColor);
                                break;
                            case 9:
                                rank = k.ToString();
                                shoe[i * 52 + j * 13 + k] = new CardNine(rank, suit, color, consoleColor);
                                break;
                            case 10:
                                rank = k.ToString();
                                shoe[i * 52 + j * 13 + k] = new CardTen(rank, suit, color, consoleColor);
                                break;
                            case 11:
                                rank = "J";
                                shoe[i * 52 + j * 13 + k] = new CardJack(rank, suit, color, consoleColor);
                                break;
                            case 12:
                                rank = "Q";
                                shoe[i * 52 + j * 13 + k] = new CardQueen(rank, suit, color, consoleColor);
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
            Card temp;
            int index;

            for (int i = 0; i < deckAmount*52; i++)
            {
                index = random.Next(deckAmount*52);

                temp = shoe[i];
                shoe[i] = shoe[index];
                shoe[index] = temp;
            }
        }

        public Card Deal()
        {
            Card card = shoe[cardToDeal];
            cardToDeal++;
            return card;
        }

        public void Display()
        {
            cardToDeal = 0;
            Console.BackgroundColor = ConsoleColor.White;
            Card card, card2;
            card2 = new CardAce("A", "Hearts", "red", ConsoleColor.Black, false);

            for (int i = 0; i < deckAmount*52; i++)
            {
                card = Deal();
                Console.ForegroundColor = card.C;
                Console.WriteLine(card.Rank + card.Suit + card.Color + card.GetCardValue(true) + "or" + card.GetCardValue(false));
                Console.WriteLine(card.GetType()==card2.GetType());
            }
        }
    }
}
