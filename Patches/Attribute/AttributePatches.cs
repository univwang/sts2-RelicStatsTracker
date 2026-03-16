using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Relics;

namespace RelicStatsTracker.Patches.Attribute;

/// <summary>
/// 属性类遗物统计 Patches
/// 包含：Shuriken, Kunai
/// </summary>
public static class AttributePatches
{
    #region Shuriken - 手里剑

    /// <summary>
    /// 手里剑 - 每打出3张攻击牌获得力量
    /// </summary>
    [HarmonyPatch(typeof(Shuriken), nameof(Shuriken.AfterCardPlayed))]
    public static class ShurikenAfterCardPlayedPatch
    {
        public static void Postfix(Shuriken __instance, PlayerChoiceContext context, CardPlay cardPlay, Task __result)
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
                __result.ContinueWith(_ =>
                {
                    int strengthAmount = __instance.DynamicVars.Strength.IntValue;
                    RelicStatsManager.RecordTrigger(__instance, RelicStatType.Strength, strengthAmount);
                });
            }
        }
    }

    #endregion

    #region Kunai - 苦无

    /// <summary>
    /// 苦无 - 每打出3张攻击牌获得敏捷
    /// </summary>
    [HarmonyPatch(typeof(Kunai), nameof(Kunai.AfterCardPlayed))]
    public static class KunaiAfterCardPlayedPatch
    {
        public static void Postfix(Kunai __instance, PlayerChoiceContext context, CardPlay cardPlay, Task __result)
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
                __result.ContinueWith(_ =>
                {
                    int dexterityAmount = __instance.DynamicVars.Dexterity.IntValue;
                    RelicStatsManager.RecordTrigger(__instance, RelicStatType.Dexterity, dexterityAmount);
                });
            }
        }
    }

    #endregion
}