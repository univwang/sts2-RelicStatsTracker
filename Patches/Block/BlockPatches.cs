using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.ValueProps;

namespace RelicStatsTracker.Patches.Block;

/// <summary>
/// 格挡类遗物统计 Patches
/// 包含：Anchor, OrnamentalFan, Orichalcum, HornCleat, CloakClasp
/// </summary>
public static class BlockPatches
{
    #region Anchor - 锚

    /// <summary>
    /// 锚 - 战斗开始时获得格挡
    /// </summary>
    [HarmonyPatch(typeof(global::MegaCrit.Sts2.Core.Models.Relics.Anchor), nameof(global::MegaCrit.Sts2.Core.Models.Relics.Anchor.BeforeCombatStart))]
    public static class AnchorBeforeCombatStartPatch
    {
        public static void Postfix(global::MegaCrit.Sts2.Core.Models.Relics.Anchor __instance, Task __result)
        {
            __result.ContinueWith(_ =>
            {
                int blockAmount = __instance.DynamicVars.Block.IntValue;
                RelicStatsManager.RecordTrigger(__instance, RelicStatType.Block, blockAmount);
            });
        }
    }

    #endregion

    #region OrnamentalFan - 华丽扇

    /// <summary>
    /// 华丽扇 - 每打出3张攻击牌获得格挡
    /// </summary>
    [HarmonyPatch(typeof(OrnamentalFan), nameof(OrnamentalFan.AfterCardPlayed))]
    public static class OrnamentalFanAfterCardPlayedPatch
    {
        public static void Postfix(OrnamentalFan __instance, PlayerChoiceContext context,
            MegaCrit.Sts2.Core.Entities.Cards.CardPlay cardPlay)
        {
            if (cardPlay.Card.Owner != __instance.Owner ||
                !MegaCrit.Sts2.Core.Combat.CombatManager.Instance.IsInProgress ||
                cardPlay.Card.Type != CardType.Attack)
            {
                return;
            }

            int attacksPlayedThisTurn = Patches.PatchHelper.GetPrivateField<int>(__instance, "_attacksPlayedThisTurn");
            int cardsNeeded = __instance.DynamicVars.Cards.IntValue;

            // 当计数归零时，说明刚刚触发了
            if (attacksPlayedThisTurn % cardsNeeded == 0)
            {
                int blockAmount = __instance.DynamicVars.Block.IntValue;
                RelicStatsManager.RecordTrigger(__instance, RelicStatType.Block, blockAmount);
            }
        }
    }

    #endregion

    #region Orichalcum - 奥利哈钢

    /// <summary>
    /// 奥利哈钢 - 回合结束时若没有格挡则获得格挡
    /// </summary>
    [HarmonyPatch(typeof(Orichalcum), nameof(Orichalcum.BeforeTurnEnd))]
    public static class OrichalcumBeforeTurnEndPatch
    {
        public static void Postfix(Orichalcum __instance, PlayerChoiceContext choiceContext, CombatSide side, Task __result)
        {
            if (side != __instance.Owner.Creature.Side) return;

            // 检查是否触发了（通过检查 ShouldTrigger 私有字段）
            bool shouldTrigger = Patches.PatchHelper.GetPrivateField<bool>(__instance, "_shouldTrigger");
            if (!shouldTrigger) return;

            __result.ContinueWith(_ =>
            {
                int blockAmount = __instance.DynamicVars.Block.IntValue;
                RelicStatsManager.RecordTrigger(__instance, RelicStatType.Block, blockAmount);
            });
        }
    }

    #endregion

    #region HornCleat - 船夹板

    /// <summary>
    /// 船夹板 - 第二回合开始时获得格挡
    /// </summary>
    [HarmonyPatch(typeof(HornCleat), nameof(HornCleat.AfterBlockCleared))]
    public static class HornCleatAfterBlockClearedPatch
    {
        public static void Postfix(HornCleat __instance, Creature creature, Task __result)
        {
            if (creature.CombatState.RoundNumber != 2) return;
            if (creature != __instance.Owner.Creature) return;

            __result.ContinueWith(_ =>
            {
                int blockAmount = __instance.DynamicVars.Block.IntValue;
                RelicStatsManager.RecordTrigger(__instance, RelicStatType.Block, blockAmount);
            });
        }
    }

    #endregion

    #region CloakClasp - 斗篷扣

    /// <summary>
    /// 斗篷扣 - 回合结束时按手牌数获得格挡
    /// </summary>
    [HarmonyPatch(typeof(CloakClasp), nameof(CloakClasp.BeforeTurnEnd))]
    public static class CloakClaspBeforeTurnEndPatch
    {
        public static void Postfix(CloakClasp __instance, PlayerChoiceContext choiceContext, CombatSide side, Task __result)
        {
            if (side != __instance.Owner.Creature.Side) return;

            __result.ContinueWith(_ =>
            {
                IReadOnlyList<CardModel> cards = PileType.Hand.GetPile(__instance.Owner).Cards;
                if (cards.Count > 0)
                {
                    int blockAmount = (int)((decimal)cards.Count * __instance.DynamicVars.Block.BaseValue);
                    RelicStatsManager.RecordTrigger(__instance, RelicStatType.Block, blockAmount);
                }
            });
        }
    }

    #endregion
}