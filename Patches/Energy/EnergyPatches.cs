using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Relics;

namespace RelicStatsTracker.Patches.Energy;

/// <summary>
/// 能量类遗物统计 Patches
/// 包含：Lantern, ArtOfWar, Nunchaku, HappyFlower, GremlinHorn
/// </summary>
public static class EnergyPatches
{
    #region Lantern - 提灯

    /// <summary>
    /// 提灯 - 第一回合开始时获得能量
    /// </summary>
    [HarmonyPatch(typeof(Lantern), nameof(Lantern.AfterSideTurnStart))]
    public static class LanternAfterSideTurnStartPatch
    {
        public static void Postfix(Lantern __instance, CombatSide side, CombatState combatState, Task __result)
        {
            if (side != __instance.Owner.Creature.Side || combatState.RoundNumber > 1) return;

            __result.ContinueWith(_ =>
            {
                int energyAmount = __instance.DynamicVars.Energy.IntValue;
                RelicStatsManager.RecordTrigger(__instance, RelicStatType.Energy, energyAmount);
            });
        }
    }

    #endregion

    #region ArtOfWar - 孙子兵法

    /// <summary>
    /// 孙子兵法 - 未打出攻击牌时获得能量
    /// </summary>
    [HarmonyPatch(typeof(ArtOfWar), nameof(ArtOfWar.AfterEnergyReset))]
    public static class ArtOfWarAfterEnergyResetPatch
    {
        public static void Postfix(ArtOfWar __instance, Player player, Task __result)
        {
            if (player != __instance.Owner) return;
            if (__instance.Owner.Creature.CombatState?.RoundNumber <= 1) return;

            bool anyAttacksPlayedLastTurn = Patches.PatchHelper.GetPrivateField<bool>(__instance, "_anyAttacksPlayedLastTurn");
            if (anyAttacksPlayedLastTurn) return;

            __result.ContinueWith(_ =>
            {
                int energyAmount = __instance.DynamicVars.Energy.IntValue;
                RelicStatsManager.RecordTrigger(__instance, RelicStatType.Energy, energyAmount);
            });
        }
    }

    #endregion

    #region Nunchaku - 双节棍

    /// <summary>
    /// 双节棍 - 每打出10张攻击牌获得能量
    /// </summary>
    [HarmonyPatch(typeof(Nunchaku), nameof(Nunchaku.AfterCardPlayed))]
    public static class NunchakuAfterCardPlayedPatch
    {
        public static void Postfix(Nunchaku __instance, PlayerChoiceContext context,
            MegaCrit.Sts2.Core.Entities.Cards.CardPlay cardPlay, Task __result)
        {
            if (cardPlay.Card.Owner != __instance.Owner || cardPlay.Card.Type != CardType.Attack) return;

            int attacksPlayed = __instance.AttacksPlayed;
            int cardsNeeded = __instance.DynamicVars.Cards.IntValue;

            // 当计数归零时，说明刚刚触发了
            if (attacksPlayed % cardsNeeded != 0 || !MegaCrit.Sts2.Core.Combat.CombatManager.Instance.IsInProgress) return;

            __result.ContinueWith(_ =>
            {
                int energyAmount = __instance.DynamicVars.Energy.IntValue;
                RelicStatsManager.RecordTrigger(__instance, RelicStatType.Energy, energyAmount);
            });
        }
    }

    #endregion

    #region HappyFlower - 开心小花

    /// <summary>
    /// 开心小花 - 每3回合获得能量
    /// </summary>
    [HarmonyPatch(typeof(HappyFlower), nameof(HappyFlower.AfterSideTurnStart))]
    public static class HappyFlowerAfterSideTurnStartPatch
    {
        public static void Postfix(HappyFlower __instance, CombatSide side, CombatState combatState, Task __result)
        {
            if (side != __instance.Owner.Creature.Side) return;

            // 当 TurnsSeen == 0 时触发
            if (__instance.TurnsSeen == 0)
            {
                __result.ContinueWith(_ =>
                {
                    int energyAmount = __instance.DynamicVars.Energy.IntValue;
                    RelicStatsManager.RecordTrigger(__instance, RelicStatType.Energy, energyAmount);
                });
            }
        }
    }

    #endregion

    #region GremlinHorn - 地精之角

    /// <summary>
    /// 地精之角 - 敌人死亡时获得能量并抽牌
    /// </summary>
    [HarmonyPatch(typeof(GremlinHorn), nameof(GremlinHorn.AfterDeath))]
    public static class GremlinHornAfterDeathPatch
    {
        public static void Postfix(GremlinHorn __instance, PlayerChoiceContext choiceContext,
            Creature target, bool wasRemovalPrevented, float deathAnimLength, Task __result)
        {
            if (target.Side == __instance.Owner.Creature.Side) return;

            __result.ContinueWith(_ =>
            {
                int energyAmount = __instance.DynamicVars.Energy.IntValue;
                int cardsDrawn = __instance.DynamicVars.Cards.IntValue;
                RelicStatsManager.RecordTrigger(__instance, new System.Collections.Generic.Dictionary<RelicStatType, int>
                {
                    { RelicStatType.Energy, energyAmount },
                    { RelicStatType.CardsDrawn, cardsDrawn }
                });
            });
        }
    }

    #endregion
}