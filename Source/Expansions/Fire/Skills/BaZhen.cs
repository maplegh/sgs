﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sanguosha.Core.UI;
using Sanguosha.Core.Skills;
using Sanguosha.Core.Players;
using Sanguosha.Core.Games;
using Sanguosha.Core.Triggers;
using Sanguosha.Core.Exceptions;
using Sanguosha.Core.Cards;
using System.Diagnostics;
using Sanguosha.Expansions.Basic.Cards;

namespace Sanguosha.Expansions.Fire.Skills
{

    public class BaZhen : TriggerSkill
    {
        protected void Run(Player Owner, GameEvent gameEvent, GameEventArgs eventArgs)
        {
            NotifySkillUse(new List<Player>());
            int answer;
            if (Game.CurrentGame.UiProxies[Owner].AskForMultipleChoice(
                    new MultipleChoicePrompt(Prompt.SkillUseYewNoPrompt, new BaGuaZhen.BaGuaZhenSkill()), Prompt.YesNoChoices, out answer)
                    && answer == 0)
            {
                ReadOnlyCard c = Game.CurrentGame.Judge(Owner, null, new Card() { Type = new BaGuaZhen() }, (judgeResultCard) => { return judgeResultCard.SuitColor == SuitColorType.Red; });
                if (c.SuitColor == SuitColorType.Red)
                {
                    eventArgs.Cards = new List<Card>();
                    ActionLog log = new ActionLog();
                    log.Source = Owner;
                    log.SkillAction = new BaGuaZhen().EquipmentSkill;
                    log.GameAction = GameAction.None;
                    Game.CurrentGame.NotificationProxy.NotifySkillUse(log);
                    throw new TriggerResultException(TriggerResult.Success);
                }
            }
        }

        public BaZhen()
        {
            var trigger = new AutoNotifyPassiveSkillTrigger(
                this,
                (p, e, a) =>
                {
                    foreach (var ac in Game.CurrentGame.Decks[p, DeckType.Equipment])
                        if (ac.Type is Armor) return false;
                    return a.Card.Type is Shan && a.ReadonlyCard[Armor.IgnoreAllArmor] == 0 && a.ReadonlyCard[Armor.IgnorePlayerArmor] != Owner.Id + 1;
                },
                Run,
                TriggerCondition.OwnerIsSource
            ) { IsAutoNotify = false };
            Triggers.Add(GameEvent.PlayerRequireCard, trigger);
            IsEnforced = true;
        }

    }
}
