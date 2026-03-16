using HarmonyLib;
using MegaCrit.Sts2.Core.Runs;

namespace RelicStatsTracker.Patches.Core;

/// <summary>
/// 新局重置 Patch
/// 在新局开始时清除统计数据
/// </summary>
[HarmonyPatch(typeof(RunManager), "InitializeNewRun")]
public static class RunManagerInitializeNewRunPatch
{
    public static void Prefix()
    {
        RelicStatsManager.ClearCurrentRun();
    }
}