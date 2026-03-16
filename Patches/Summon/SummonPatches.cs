using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models.Relics;

namespace RelicStatsTracker.Patches.Summon;

/// <summary>
/// 召唤类遗物统计 Patches
/// 包含：BoundPhylactery, PhylacteryUnbound, BoneFlute, Byrdpip, PaelsLegion
/// </summary>
public static class SummonPatches
{
    #region BoundPhylactery - 缚魂命匣

    /// <summary>
    /// 缚魂命匣 - 战斗开始时召唤 Osty
    /// </summary>
    [HarmonyPatch(typeof(BoundPhylactery), nameof(BoundPhylactery.BeforeCombatStart))]
    public static class BoundPhylacteryBeforeCombatStartPatch
    {
        public static void Postfix(BoundPhylactery __instance, Task __result)
        {
            __result.ContinueWith(_ =>
            {
                int summonCount = __instance.DynamicVars.Summon.IntValue;
                RelicStatsManager.RecordTrigger(__instance, RelicStatType.Summon, summonCount);
            });
        }
    }

    /// <summary>
    /// 缚魂命匣 - 每回合开始时召唤 Osty（非第一回合）
    /// </summary>
    [HarmonyPatch(typeof(BoundPhylactery), nameof(BoundPhylactery.AfterEnergyResetLate))]
    public static class BoundPhylacteryAfterEnergyResetLatePatch
    {
        public static void Postfix(BoundPhylactery __instance, Player player, Task __result)
        {
            if (player != __instance.Owner) return;
            if (player.Creature.CombatState.RoundNumber == 1) return;

            __result.ContinueWith(_ =>
            {
                int summonCount = __instance.DynamicVars.Summon.IntValue;
                RelicStatsManager.RecordTrigger(__instance, RelicStatType.Summon, summonCount);
            });
        }
    }

    #endregion

    #region PhylacteryUnbound - 无界命匣

    /// <summary>
    /// 无界命匣 - 战斗开始时召唤 Osty
    /// </summary>
    [HarmonyPatch(typeof(PhylacteryUnbound), nameof(PhylacteryUnbound.BeforeCombatStart))]
    public static class PhylacteryUnboundBeforeCombatStartPatch
    {
        public static void Postfix(PhylacteryUnbound __instance, Task __result)
        {
            __result.ContinueWith(_ =>
            {
                int summonCount = __instance.DynamicVars["StartOfCombat"].IntValue;
                RelicStatsManager.RecordTrigger(__instance, RelicStatType.Summon, summonCount);
            });
        }
    }

    /// <summary>
    /// 无界命匣 - 每回合开始时召唤 Osty
    /// </summary>
    [HarmonyPatch(typeof(PhylacteryUnbound), nameof(PhylacteryUnbound.AfterSideTurnStart))]
    public static class PhylacteryUnboundAfterSideTurnStartPatch
    {
        public static void Postfix(PhylacteryUnbound __instance, CombatSide side, CombatState combatState, Task __result)
        {
            if (side != CombatSide.Player) return;

            __result.ContinueWith(_ =>
            {
                int summonCount = __instance.DynamicVars["StartOfTurn"].IntValue;
                RelicStatsManager.RecordTrigger(__instance, RelicStatType.Summon, summonCount);
            });
        }
    }

    #endregion

    #region BoneFlute - 骨笛

    /// <summary>
    /// 骨笛 - Osty 攻击时获得格挡
    /// </summary>
    [HarmonyPatch(typeof(BoneFlute), nameof(BoneFlute.AfterAttack))]
    public static class BoneFluteAfterAttackPatch
    {
        public static void Postfix(BoneFlute __instance, AttackCommand command, Task __result)
        {
            // 检查攻击者是否是 Osty
            if (command.Attacker?.Monster is not MegaCrit.Sts2.Core.Models.Monsters.Osty) return;
            if (command.Attacker.PetOwner?.Creature != __instance.Owner.Creature) return;

            __result.ContinueWith(_ =>
            {
                int blockAmount = __instance.DynamicVars.Block.IntValue;
                RelicStatsManager.RecordTrigger(__instance, RelicStatType.Block, blockAmount);
            });
        }
    }

    #endregion

    #region Byrdpip - 异鸟宝宝

    /// <summary>
    /// 异鸟宝宝 - 战斗开始时召唤宠物
    /// </summary>
    [HarmonyPatch(typeof(global::MegaCrit.Sts2.Core.Models.Relics.Byrdpip), nameof(global::MegaCrit.Sts2.Core.Models.Relics.Byrdpip.BeforeCombatStart))]
    public static class ByrdpipBeforeCombatStartPatch
    {
        public static void Postfix(global::MegaCrit.Sts2.Core.Models.Relics.Byrdpip __instance, Task __result)
        {
            __result.ContinueWith(_ =>
            {
                RelicStatsManager.RecordTrigger(__instance, RelicStatType.Summon, 1);
            });
        }
    }

    #endregion

    #region PaelsLegion - 远古遗物

    /// <summary>
    /// PaelsLegion - 格挡翻倍
    /// 只有第一张触发效果的防御牌才统计
    /// </summary>
    [HarmonyPatch(typeof(global::MegaCrit.Sts2.Core.Models.Relics.PaelsLegion), nameof(global::MegaCrit.Sts2.Core.Models.Relics.PaelsLegion.AfterModifyingBlockAmount))]
    public static class PaelsLegionAfterModifyingBlockAmountPatch
    {
        private static bool _wasAffectedCardPlayNull;

        public static void Prefix(global::MegaCrit.Sts2.Core.Models.Relics.PaelsLegion __instance, decimal modifiedAmount, CardPlay? cardPlay)
        {
            // 记录 AffectedCardPlay 是否为 null（表示还未触发过）
            var affectedCardPlay = PatchHelper.GetPrivateField<CardPlay?>(__instance, "_affectedCardPlay");
            _wasAffectedCardPlayNull = affectedCardPlay == null && cardPlay != null && modifiedAmount > 0;
        }

        public static void Postfix(global::MegaCrit.Sts2.Core.Models.Relics.PaelsLegion __instance, decimal modifiedAmount)
        {
            if (!_wasAffectedCardPlayNull) return;

            // 只有第一张触发效果的卡牌才统计
            // modifiedAmount 是翻倍后的格挡量（即额外获得的格挡）
            RelicStatsManager.RecordTrigger(__instance, RelicStatType.Block, (int)modifiedAmount / 2);
        }
    }

    #endregion
}