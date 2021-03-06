﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using Sanguosha.Core.Cards;

namespace Sanguosha.UI.Controls
{
    public class DiscardDeck : CardStack, IDeckContainer
    {
        Timer _cleanUpCounter;
        int _currentTime;

        public DiscardDeck()
        {
            _cleanUpCounter = new Timer(1000);
            _cleanUpCounter.AutoReset = true;
            _cleanUpCounter.Elapsed += _cleanUpCounter_Elapsed;
            _cleanUpCounter.Start();
            _currentTime = 0;
        }

        private static int _ClearanceTimeAllowance = 5;

        private static int _MaxVisibleCards = 10;

        private void _MakeDisappear(CardView card)
        {
            card.Disappear(0.5d);
            Cards.Remove(card);
        }

        public void Clear()
        {
            foreach (CardView card in Cards)
            {
                card.Disappear(0.5d);
            }
            Cards.Clear();
        }

        private void MarkClearance(CardView card)
        {
            if (card.DiscardDeckClearTimeStamp > _currentTime)
            {
                card.DiscardDeckClearTimeStamp = _currentTime;
            }
        }

        private void _cleanUpCounter_Elapsed(object sender, ElapsedEventArgs e)
        {
            bool changed = false;
            _currentTime++;
            Application.Current.Dispatcher.Invoke((System.Threading.ThreadStart)delegate()
            {
                for (int i = 0; i < Cards.Count; i++)
                {
                    CardView card = Cards[i];
                    if (_currentTime - card.DiscardDeckClearTimeStamp > _ClearanceTimeAllowance)
                    {
                        changed = true;
                        _MakeDisappear(card);
                        i--;
                    }

                    if (_currentTime > card.DiscardDeckClearTimeStamp)
                    {
                        card.CardModel.IsFaded = true;
                    }
                }

                if (changed)
                {
                    RearrangeCards(0.5d);
                }
            });
        }

        /// <summary>
        /// Fade out all the cards currently in the deck and make them disappear after certain interval.
        /// </summary>
        public void UnlockCards()
        {
            foreach (var card in Cards)
            {
                MarkClearance(card);
            }
        }

        

        public void AddCards(DeckType deck, IList<CardView> cards, bool isFaked)
        {
            if (isFaked)
            {
                foreach (var card in cards)
                {
                    card.Disappear(0d);
                }
                return;
            }

            if (cards.Count == 0) return;

            int numAdded = cards.Count;
            int numRemoved = Cards.Count - Math.Max(_MaxVisibleCards, numAdded + 1);

            DeckType from = cards[0].Card.Place.DeckType;
            Canvas canvas = cards[0].Parent as Canvas;
            Trace.Assert(canvas != null);

            foreach (var card in cards)
            {
                card.CardModel.UpdateFootnote();
                card.CardModel.IsFootnoteVisible = true;
            }

            // Do not show cards that move from compute area to discard area
            // or from judge result area to discard aresa
            if (from != DeckType.Compute && from != DeckType.JudgeResult && from != DeckType.Dealing)
            {
                AddCards(cards, 0.3d);
            }
            else if (from == DeckType.Dealing && deck == DeckType.JudgeResult)
            {
                foreach (var card in Cards) MarkClearance(card);
                AppendCards(cards);
            }
            else
            {
                foreach (var card in cards)
                {
                    canvas.Children.Remove(card);
                }
            }

            // Card just entered compute area should hold until they enter discard area.
            foreach (var card in cards)
            {
                card.DiscardDeckClearTimeStamp = int.MaxValue;
            }            
            
            // When a card enters discard area, every thing in the deck should fade out (but
            // not disappear).
            // When there are too many cards in the deck, remove the dated ones.
            for (int i = 0; i < numRemoved; i++)
            {
                MarkClearance(Cards[i]);
            }                      
        }

        public IList<CardView> RemoveCards(DeckType deck, IList<Card> cards)
        {
            IList<CardView> result = new List<CardView>();
            IList<CardView> remaining = new List<CardView>();

            foreach (var card in cards)
            {
                var oldCardView = Cards.FirstOrDefault(
                    c => ((card.Id > 0 && c.Card.Id == card.Id) || (card.Id <= 0 && c.Card == card)));
                var cardView = CardView.CreateCard(card);
                if (oldCardView != null)
                {
                    ParentCanvas.Children.Add(cardView);
                    cardView.Position = oldCardView.Position;
                    cardView.Rebase(0);                    
                }
                else
                {
                    remaining.Add(cardView);
                }
                result.Add(cardView);
            }
            RemoveCards(remaining);
            return result;
        }
    }
}
