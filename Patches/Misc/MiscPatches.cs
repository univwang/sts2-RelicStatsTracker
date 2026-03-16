using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rooms;

namespace RelicStatsTracker.Patches.Misc;

/// <summary>
/// 其他遗物统计 Patches
/// 包含：Akabeko, Gorget, FencingManual
/// </summary>
public static class MiscPatches
{
    #region Akabeko - 赤牛

    /// <summary>
    /// 赤牛 - 第一回合开始后获得活力
    /// </summary>
    [HarmonyPatch(typeof(Akabeko), nameof(Akabeko.AfterSideTurnStart))]
    public static class AkabekoAfterSideTurnStartPatch
    {
        public static void Postfix(Akabeko __instance, CombatSide side, CombatState combatState, Task __result)
        {
            if (side != __instance.Owner.Creature.Side || combatState.RoundNumber > 1) return;

            __result.ContinueWith(_ =>
            {
                int vigorAmount = __instance.DynamicVars["VigorPower"].IntValue;
                RelicStatsManager.RecordTrigger(__instance, RelicStatType.Vigor, vigorAmount);
            });
        }
    }

    #endregion

    #region Gorget - 护喉甲

    /// <summary>
    /// 护喉甲 - 进入战斗房间时获得覆甲
    /// </summary>
    [HarmonyPatch(typeof(Gorget), nameof(Gorget.AfterRoomEntered))]
    public static class GorgetAfterRoomEnteredPatch
    {
        public static void Postfix(Gorget __instance, AbstractRoom room, Task __result)
        {
            if (room is not CombatRoom) return;

            __result.ContinueWith(_ =>
            {
                int platingAmount = __instance.DynamicVars["PlatingPower"].IntValue;
                RelicStatsManager.RecordTrigger(__instance, RelicStatType.Plating, platingAmount);
            });
        }
    }

    #endregion

    #region FencingManual - 击剑指南

    /// <summary>
    /// 击剑指南 - 第一回合开始后获得铸造
    /// </summary>
    [HarmonyPatch(typeof(FencingManual), nameof(FencingManual.AfterSideTurnStart))]
    public static class FencingManualAfterSideTurnStartPatch
    {
        public static void Postfix(FencingManual __instance, CombatSide side, CombatState combatState, Task __result)
        {
            if (side != __instance.Owner.Creature.Side || combatState.RoundNumber > 1) return;

            __result.ContinueWith(_ =>
            {
                int forgeAmount = __instance.DynamicVars.Forge.IntValue;
                RelicStatsManager.RecordTrigger(__instance, RelicStatType.Forge, forgeAmount);
            });
        }
    }

    #endregion
}