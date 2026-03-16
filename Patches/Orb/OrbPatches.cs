using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rooms;

namespace RelicStatsTracker.Patches.Orb;

/// <summary>
/// 球类遗物统计 Patches
/// 包含：CrackedCore, InfusedCore, DataDisk
/// </summary>
public static class OrbPatches
{
    #region CrackedCore - 破损核心

    /// <summary>
    /// 破损核心 - 第一回合开始前引导闪电球
    /// </summary>
    [HarmonyPatch(typeof(CrackedCore), nameof(CrackedCore.BeforeSideTurnStart))]
    public static class CrackedCoreBeforeSideTurnStartPatch
    {
        public static void Postfix(CrackedCore __instance, PlayerChoiceContext choiceContext,
            CombatSide side, CombatState combatState, Task __result)
        {
            if (side != __instance.Owner.Creature.Side || combatState.RoundNumber > 1) return;

            __result.ContinueWith(_ =>
            {
                int orbCount = __instance.DynamicVars["Lightning"].IntValue;
                RelicStatsManager.RecordTrigger(__instance, RelicStatType.OrbChanneled, orbCount);
            });
        }
    }

    #endregion

    #region InfusedCore - 注能核心

    /// <summary>
    /// 注能核心 - 第一回合开始后引导闪电球
    /// </summary>
    [HarmonyPatch(typeof(InfusedCore), nameof(InfusedCore.AfterSideTurnStart))]
    public static class InfusedCoreAfterSideTurnStartPatch
    {
        public static void Postfix(InfusedCore __instance, CombatSide side, CombatState combatState, Task __result)
        {
            if (side != __instance.Owner.Creature.Side || combatState.RoundNumber > 1) return;

            __result.ContinueWith(_ =>
            {
                int orbCount = __instance.DynamicVars["Lightning"].IntValue;
                RelicStatsManager.RecordTrigger(__instance, RelicStatType.OrbChanneled, orbCount);
            });
        }
    }

    #endregion

    #region DataDisk - 数据磁盘

    /// <summary>
    /// 数据磁盘 - 进入战斗房间时获得集中
    /// </summary>
    [HarmonyPatch(typeof(DataDisk), nameof(DataDisk.AfterRoomEntered))]
    public static class DataDiskAfterRoomEnteredPatch
    {
        public static void Postfix(DataDisk __instance, AbstractRoom room, Task __result)
        {
            if (room is not CombatRoom) return;

            __result.ContinueWith(_ =>
            {
                int focusAmount = __instance.DynamicVars["FocusPower"].IntValue;
                RelicStatsManager.RecordTrigger(__instance, RelicStatType.Focus, focusAmount);
            });
        }
    }

    #endregion
}