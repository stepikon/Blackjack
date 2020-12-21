﻿using System;
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

        Card[] shoe;

        private Card hiddenCard;

        Random random;

        public Dealer(string name, List<Card> hand, int deckAmount, Random random)
            :base(name, hand)
        {
            this.name = name;
            this.deckAmount = deckAmount;
            this.random = random;

            shoe = new Card[52*deckAmount];
        }

        public string Name { get; }
        public int DeckAmount { get; }
        public int CardToDeal { get; }

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

        public void Deal(Character character, int i)
        {
            Card card = shoe[cardToDeal];
            character.hands[i].Add(card);
            cardToDeal++;
        }

        public void DealHidden()
        {
            hiddenCard = shoe[cardToDeal];
            cardToDeal++;
        }

        public void RevealHidden()
        {
            hand.Add(hiddenCard);
            hiddenCard = null;
        }

        public void TakeTurn()
        {
            string choice = "";

            do
            {
                choice = GetChoice();

                switch (choice)
                {
                    case CHOICE_HIT:
                        Hit();
                        break;
                    case CHOICE_STAND:
                        Stand();
                        break;
                    default:
                        break;
                }
            } while (choice!=CHOICE_STAND);
        }

        public override string GetChoice()
        {
            if (GetHandValue(hand).Item2<17)
            {
                return CHOICE_HIT;
            }
            else if (GetHandValue(hand).Item2 == 17 && GetHandValue(hand).Item3)
            {
                return CHOICE_HIT;
            }
            else
            {
                return CHOICE_STAND;
            }
        }

        public override Tuple<int, int, bool> GetHandValue(List<Card> hand)
        {
            int handValue = 0;
            bool hasSoftAce = false;

            foreach (Card c in hand)
            {
                handValue += c.GetCardValue();
            }

            foreach (Card c in hand)
            {
                if (c is CardAce)
                {
                    CardAce cA = (CardAce)c;
                    if (!cA.AceIsOne)
                    {
                        hasSoftAce = true;
                        break;
                    }
                }
            }

            if (handValue > 21 && hasSoftAce) //since 11+11=22, one hand can only contain 1 soft ace.
            {
                SetSoftAceToHard();

                handValue = 0;
                hasSoftAce = false;
                foreach (Card c in hand)
                {
                    handValue += c.GetCardValue();
                }

                foreach (Card c in hand)
                {
                    if (c is CardAce)
                    {
                        CardAce cA = (CardAce)c;
                        if (!cA.AceIsOne)
                        {
                            hasSoftAce = true;
                            break;
                        }
                    }
                }
            }

            return new Tuple<int, int, bool>(hasSoftAce ? handValue - 10 : handValue, handValue, hasSoftAce);
        }

        public override void SetSoftAceToHard()
        {
            foreach (Card c in hand)
            {
                if (c is CardAce)
                {
                    CardAce cA = (CardAce)c;
                    if (!cA.AceIsOne)
                    {
                        cA.AceIsOne = true;
                        break;
                    }
                }
            }
        }

        private void Hit()
        {
            Deal(this, 0);
        }

        private void Stand()
        { 
        
        }

        public void DisplayHand()
        {
            foreach (Card c in hand)
            {
                Console.WriteLine(c.Name);
            }
        }

        public void Display()
        {
            foreach (Card card in shoe)
            {
                Console.WriteLine(card.Name);
            }
        }

        public void ResetHand()
        {
            hand.Clear();
        }
    }
}