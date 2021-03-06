﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sanguosha.Core.Triggers;
using Sanguosha.Core.Cards;
using Sanguosha.Core.UI;
using Sanguosha.Core.Skills;
using Sanguosha.Expansions.Basic.Cards;

namespace Sanguosha.Expansions.Basic.Skills
{
    /// <summary>
    /// 龙胆–出牌阶段，你可以将你手牌的【杀】当【闪】、【闪】当【杀】使用或打出。
    /// </summary>
    public class LongDan : CardTransformSkill
    {
        public override VerifierResult TryTransform(List<Card> cards, object arg, out CompositeCard card)
        {
            card = null;
            if (cards == null || cards.Count < 1)
            {
                return VerifierResult.Partial;
            }
            if (cards.Count > 1)
            {
                return VerifierResult.Fail;
            }
            if (cards[0].Owner != Owner || cards[0].Place.DeckType != DeckType.Hand)
            {
                return VerifierResult.Fail;
            }
            if (cards[0].Type is Shan)
            {
                card = new CompositeCard();
                card.Subcards = new List<Card>(cards);
                card.Type = new RegularSha();
                return VerifierResult.Success;
            }
            else if (cards[0].Type is Sha)
            {
                card = new CompositeCard();
                card.Subcards = new List<Card>(cards);
                card.Type = new Shan();
                return VerifierResult.Success;
            }
            return VerifierResult.Fail;
        }

        public override List<CardHandler> PossibleResults
        {
            get { return new List<CardHandler>() {new Shan(), new Sha()}; }
        }
    }
}
