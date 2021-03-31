using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;

namespace Blackjack
{
    class Dealer:Character
    {
        private int deckAmount;
        private int deckPenetration;
        private int cardToDeal;

        private bool hitSoft17;
        private bool wait;
        private bool isVisible;

        string name;

        Card[] shoe;

        private Card hiddenCard;

        Random random;

        public Dealer(string name, List<Card> hand, BetterUI betterUI, int deckAmount, Random random, bool hitSoft17, bool wait, bool isVisible = true)
            :base(name, hand, betterUI)
        {
            this.name = name;
            this.deckAmount = deckAmount;
            this.random = random;
            this.hitSoft17 = hitSoft17;
            this.wait = wait;
            this.isVisible = isVisible;

            shoe = new Card[52*deckAmount];
        }

        public int DeckAmount { get { return deckAmount; } }
        public int CardToDeal { get { return cardToDeal; } }
        public int DeckPenetration { get { return deckPenetration; } }
        public bool HitSoft17 { get { return hitSoft17; } }

        public void CreateShoe()
        {
            string suit="", color = "";
            ConsoleColor consoleColor = 0;

            CardCreator[] creators = new CardCreator[] {
                new CardTwoCreator("2"),
                new CardThreeCreator("3"),
                new CardFourCreator("4"),
                new CardFiveCreator("5"),
                new CardSixCreator("6"),
                new CardSevenCreator("7"),
                new CardEightCreator("8"),
                new CardNineCreator("9"),
                new CardTenCreator("10"),
                new CardJackCreator("J"),
                new CardQueenCreator("K"),
                new CardKingCreator("Q"),
                new CardAceCreator("A", false),
            };

            for (int i = 0; i < deckAmount; i++)
            {
                for (int j = 0; j < 4; j++)
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

                    for (int k = 0; k < 13; k++)
                    {
                        shoe[i * 52 + j * 13 + k] = creators[k].CreateCard(suit, color, consoleColor);
                    }
                }
            }
        }       

        public void Reset()
        {
            Shuffle();
            SetDeckPenetration();
            cardToDeal = 0;

            foreach (Card c in shoe)
            {
                if (c is CardAce)
                {
                    CardAce cA = (CardAce)c;
                    cA.AceIsOne = false;
                }
            }
        }

        public void SetDeckPenetration() //deck penetration is 75%-85%
        { 
            deckPenetration = random.Next(deckAmount * 52 * 3 / 4, deckAmount * 52 * 17 / 20);
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

            cardToDeal = 0;
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

        public string GetHiddenCardName()
        {
            return hiddenCard == null ? "" : "?";
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
                        if (isVisible)
                        {
                            DisplayHand();
                            if (wait)
                            {
                                Thread.Sleep(1000);
                            }
                        }
                        break;
                    case CHOICE_STAND:
                        Stand();
                        if (isVisible)
                        {
                            DisplayHand();
                        }
                        break;
                    default:
                        break;
                }
            } while (choice!=CHOICE_STAND);
        }

        public string GetChoice()
        {
            if (GetHandValue(hand).Item2<17)
            {
                return CHOICE_HIT;
            }
            else if (GetHandValue(hand).Item2 == 17 && GetHandValue(hand).Item3)
            {
                if (hitSoft17)
                {
                    return CHOICE_HIT;
                }
                else
                {
                    return CHOICE_STAND;
                }
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
                SetSoftAceToHard(hand);

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

        public override void SetSoftAceToHard(List<Card> hand)
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
            if (isVisible)
            {
                if (Console.WindowHeight >= MINIMUM_WINDIW_HEIGHT && Console.WindowWidth >= MINIMUM_WINDIW_WIDTH)
                {
                    betterUI.DisplayDealerStatus(this);
                }
                else
                {
                    Console.Write("Hand: ");
                    foreach (Card c in hand)
                    {
                        Console.Write("{0} ", c.Name);
                    }
                    Console.WriteLine();
                }
            }        
        }

        public void Display()
        {
            foreach (Card card in shoe)
            {
                Console.WriteLine(card.Name);
            }
        }

        public override void SetHasBlackjack()
        {
            if (hiddenCard!=null)
            {
                if (hand[0].GetCardValue()+hiddenCard.GetCardValue() == 21 && hand.Count == 1)
                {
                    hasBlackjack = true;
                }
                else
                {
                    hasBlackjack = false;
                }
            }
            else
            {
                if (hand.Count == 2 && hand[0].GetCardValue() + hand[1].GetCardValue() == 21)
                {
                    hasBlackjack = true;
                }
                else
                {
                    hasBlackjack = false;
                }
            }
        }

        public double GetDecksInDiscard()
        {
            int wholeDecks = cardToDeal / 52;
            int rest = cardToDeal % 52;
            double halfDecks;

            if (rest < 13)
            {
                halfDecks = 0;
            }
            else if (rest >= 13 && rest < 39)
            {
                halfDecks = 1;
            }
            else
            {
                halfDecks = 2;
            }

            return wholeDecks + 0.5 * halfDecks;
        }

        public void ResetHand()
        {
            hand.Clear();
            hasBlackjack = false;
        }
    }
}
