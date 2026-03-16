using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
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
    /// PaelsLegion - 格挡翻倍触发
    /// 在 AfterCardPlayed 中检测格挡翻倍效果
    /// </summary>
    [HarmonyPatch(typeof(global::MegaCrit.Sts2.Core.Models.Relics.PaelsLegion), nameof(global::MegaCrit.Sts2.Core.Models.Relics.PaelsLegion.AfterCardPlayed))]
    public static class PaelsLegionAfterCardPlayedPatch
    {
        public static void Postfix(global::MegaCrit.Sts2.Core.Models.Relics.PaelsLegion __instance,
            MegaCrit.Sts2.Core.Entities.Cards.CardPlay cardPlay, Task __result)
        {
            __result.ContinueWith(_ =>
            {
                // 检查是否触发了格挡翻倍（通过检查 AffectedCardPlay 是否被清除）
                // 如果触发了，遗物会 Flash 并且 Cooldown 会被设置
                var affectedCardPlay = Patches.PatchHelper.GetPrivateField<MegaCrit.Sts2.Core.Entities.Cards.CardPlay?>(__instance, "_affectedCardPlay");
                if (affectedCardPlay == null)
                {
                    // 检查 cooldown 是否刚被设置（表示触发了格挡翻倍）
                    var cooldown = Patches.PatchHelper.GetPrivateField<int>(__instance, "_cooldown");
                    var turnsNeeded = __instance.DynamicVars["Turns"].IntValue;
                    if (cooldown == turnsNeeded)
                    {
                        RelicStatsManager.RecordTrigger(__instance, RelicStatType.BlockDoubled, 1);
                    }
                }
            });
        }
    }

    #endregion
}