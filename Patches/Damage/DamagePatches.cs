using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Relics;

namespace RelicStatsTracker.Patches.Damage;

/// <summary>
/// 伤害类遗物统计 Patches
/// 包含：MercuryHourglass, LetterOpener, PenNib, BagOfMarbles
/// </summary>
public static class DamagePatches
{
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
    /// 笔尖 - 每打出10张攻击牌，下一张攻击牌伤害翻倍
    /// </summary>
    [HarmonyPatch(typeof(PenNib), nameof(PenNib.BeforeCardPlayed))]
    public static class PenNibBeforeCardPlayedPatch
    {
        public static void Postfix(PenNib __instance, CardPlay cardPlay)
        {
            if (cardPlay.Card.Type != CardType.Attack) return;
            if (cardPlay.Card.Owner != __instance.Owner) return;

            // 当 AttacksPlayed 刚刚归零时，说明触发了双倍伤害
            if (__instance.AttacksPlayed == 0)
            {
                RelicStatsManager.RecordCustomStat(__instance, "double_damage_count", 1);
            }
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