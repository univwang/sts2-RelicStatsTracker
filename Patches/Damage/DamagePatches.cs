using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.ValueProps;

namespace RelicStatsTracker.Patches.Damage;

/// <summary>
/// 伤害类遗物统计 Patches
/// 包含：MercuryHourglass, LetterOpener, PenNib, BagOfMarbles
/// </summary>
public static class DamagePatches
{
    // 追踪当前被 PenNib 翻倍的卡牌
    private static readonly HashSet<CardModel> _penNibDoubledCards = new();

    #region MercuryHourglass - 水银沙漏

    /// <summary>
    /// 水银沙漏 - 回合开始时对所有敌人造成伤害
    /// </summary>
    [HarmonyPatch(typeof(MercuryHourglass), nameof(MercuryHourglass.AfterPlayerTurnStart))]
    public static class MercuryHourglassAfterPlayerTurnStartPatch
    {
        public static void Postfix(MercuryHourglass __instance, PlayerChoiceContext choiceContext, Player player, Task __result)
        {
            if (player != __instance.Owner) return;

            __result.ContinueWith(_ =>
            {
                int damage = __instance.DynamicVars.Damage.IntValue;
                int enemyCount = player.Creature.CombatState.HittableEnemies.Count;
                RelicStatsManager.RecordTrigger(__instance, RelicStatType.Damage, damage * enemyCount);
            });
        }
    }

    #endregion

    #region LetterOpener - 开信刀

    /// <summary>
    /// 开信刀 - 每打出3张技能牌对所有敌人造成伤害
    /// </summary>
    [HarmonyPatch(typeof(LetterOpener), nameof(LetterOpener.AfterCardPlayed))]
    public static class LetterOpenerAfterCardPlayedPatch
    {
        public static void Postfix(LetterOpener __instance, PlayerChoiceContext context, CardPlay cardPlay, Task __result)
        {
            if (cardPlay.Card.Owner != __instance.Owner ||
                !CombatManager.Instance.IsInProgress ||
                cardPlay.Card.Type != CardType.Skill)
            {
                return;
            }

            int skillsPlayedThisTurn = Patches.PatchHelper.GetPrivateField<int>(__instance, "_skillsPlayedThisTurn");
            int cardsNeeded = __instance.DynamicVars.Cards.IntValue;

            // 当计数归零时，说明刚刚触发了
            if (skillsPlayedThisTurn % cardsNeeded == 0)
            {
                __result.ContinueWith(_ =>
                {
                    int damage = __instance.DynamicVars.Damage.IntValue;
                    int enemyCount = __instance.Owner.Creature.CombatState.HittableEnemies.Count;
                    RelicStatsManager.RecordTrigger(__instance, RelicStatType.Damage, damage * enemyCount);
                });
            }
        }
    }

    #endregion

    #region PenNib - 笔尖

    /// <summary>
    /// 笔尖 - 追踪被翻倍的卡牌
    /// 在 ModifyDamageMultiplicative 中，当返回 2 时记录卡牌
    /// </summary>
    [HarmonyPatch(typeof(PenNib), nameof(PenNib.ModifyDamageMultiplicative))]
    public static class PenNibModifyDamageMultiplicativePatch
    {
        public static void Postfix(PenNib __instance, Creature? target, decimal amount, ValueProp props,
            Creature? dealer, CardModel? cardSource, ref decimal __result)
        {
            // 如果返回 2，说明伤害被翻倍
            if (__result == 2m && cardSource != null)
            {
                _penNibDoubledCards.Add(cardSource);
            }
        }
    }

    /// <summary>
    /// 笔尖 - 在攻击完成后统计伤害
    /// </summary>
    [HarmonyPatch(typeof(Hook), nameof(Hook.AfterAttack))]
    public static class HookAfterAttackPatch
    {
        public static void Postfix(CombatState combatState, AttackCommand command, Task __result)
        {
            // 检查是否是 PenNib 翻倍的攻击
            var cardSource = command.ModelSource as CardModel;
            if (cardSource == null || !_penNibDoubledCards.Contains(cardSource)) return;

            // 移除追踪
            _penNibDoubledCards.Remove(cardSource);

            __result.ContinueWith(_ =>
            {
                // 统计伤害
                int totalDamage = command.Results.Sum(r => r.TotalDamage);
                if (totalDamage <= 0) return;

                // 找到 PenNib 遗物
                var player = command.Attacker?.Player ?? command.Attacker?.PetOwner;
                if (player == null) return;

                var penNib = player.Relics.OfType<PenNib>().FirstOrDefault();
                if (penNib != null)
                {
                    RelicStatsManager.RecordTrigger(penNib, RelicStatType.Damage, totalDamage);
                }
            });
        }
    }

    #endregion

    #region BagOfMarbles - 弹珠袋

    /// <summary>
    /// 弹珠袋 - 战斗开始时给予敌人易伤
    /// </summary>
    [HarmonyPatch(typeof(BagOfMarbles), nameof(BagOfMarbles.BeforeSideTurnStart))]
    public static class BagOfMarblesBeforeSideTurnStartPatch
    {
        public static void Postfix(BagOfMarbles __instance, PlayerChoiceContext choiceContext,
            CombatSide side, CombatState combatState, Task __result)
        {
            if (side != __instance.Owner.Creature.Side || combatState.RoundNumber > 1) return;

            __result.ContinueWith(_ =>
            {
                int vulnerableAmount = __instance.DynamicVars.Vulnerable.IntValue;
                int enemyCount = combatState.HittableEnemies.Count;
                RelicStatsManager.RecordCustomStat(__instance, "vulnerable_applied", vulnerableAmount * enemyCount);
            });
        }
    }

    #endregion
}