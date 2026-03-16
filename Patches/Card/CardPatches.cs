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

namespace RelicStatsTracker.Patches.Card;

/// <summary>
/// 抽牌类遗物统计 Patches
/// 包含：CentennialPuzzle, BagOfPreparation
/// </summary>
public static class CardPatches
{
    #region CentennialPuzzle - 百年积木

    /// <summary>
    /// 百年积木 - 第一次受到伤害时抽牌
    /// </summary>
    [HarmonyPatch(typeof(CentennialPuzzle), nameof(CentennialPuzzle.AfterDamageReceived))]
    public static class CentennialPuzzleAfterDamageReceivedPatch
    {
        public static void Postfix(CentennialPuzzle __instance, PlayerChoiceContext choiceContext,
            Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource, Task __result)
        {
            if (!MegaCrit.Sts2.Core.Combat.CombatManager.Instance.IsInProgress ||
                target != __instance.Owner.Creature ||
                result.UnblockedDamage <= 0 ||
                __instance.UsedThisCombat)
            {
                return;
            }

            __result.ContinueWith(_ =>
            {
                int cardsDrawn = __instance.DynamicVars.Cards.IntValue;
                RelicStatsManager.RecordTrigger(__instance, RelicStatType.CardsDrawn, cardsDrawn);
            });
        }
    }

    #endregion

    #region BagOfPreparation - 准备背包

    /// <summary>
    /// 准备背包 - 第一回合额外抽牌
    /// 使用 ModifyHandDraw 方法修改抽牌数
    /// </summary>
    [HarmonyPatch(typeof(BagOfPreparation), nameof(BagOfPreparation.ModifyHandDraw))]
    public static class BagOfPreparationModifyHandDrawPatch
    {
        public static void Postfix(BagOfPreparation __instance, Player player, decimal count, ref decimal __result)
        {
            if (player != __instance.Owner) return;
            if (player.Creature.CombatState.RoundNumber > 1) return;

            // 计算额外抽牌数
            decimal extraCards = __result - count;
            if (extraCards > 0)
            {
                RelicStatsManager.RecordTrigger(__instance, RelicStatType.CardsDrawn, (int)extraCards);
            }
        }
    }

    #endregion
}