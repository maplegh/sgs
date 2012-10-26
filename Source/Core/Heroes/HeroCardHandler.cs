﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sanguosha.Core.Cards;
using Sanguosha.Core.Heroes;
using Sanguosha.Core.Skills;
using Sanguosha.Core.UI;
using Sanguosha.Core.Players;

namespace Sanguosha.Core.Heroes
{
    public class HeroCardHandler : CardHandler
    {
        protected override void Process(Players.Player source, Players.Player dest, ICard card)
        {
            throw new NotImplementedException();
        }

        protected override VerifierResult Verify(Player source, ICard card, List<Player> targets)
        {
            if (targets == null || targets.Count == 0)
            {
                return VerifierResult.Success;
            }
            else
            {
                return VerifierResult.Fail;
            }
        }

        private Hero hero;

        public Hero Hero
        {
            get { return hero; }
            set { hero = value; }
        }
        public HeroCardHandler(Hero h)
        {
            hero = h;
        }

        public override CardCategory Category
        {
            get { return CardCategory.Unknown; }
        }

        public override string CardType
        {
            get
            {
                return Hero.Name;
            }
        }
    }
}
