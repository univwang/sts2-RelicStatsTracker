using HarmonyLib;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace RelicStatsTracker.Patches.Core;

/// <summary>
/// 遗物悬浮提示增强 Patch
/// 在遗物悬浮提示中添加统计数据
/// </summary>
[HarmonyPatch(typeof(RelicModel), nameof(RelicModel.HoverTip), MethodType.Getter)]
public static class RelicModelHoverTipPatch
{
    public static void Postfix(RelicModel __instance, ref HoverTip __result)
    {
        var stats = RelicStatsManager.GetStats(__instance);

        if (stats.HasAnyStats())
        {
            var statsText = Localization.BuildStatsText(stats);
            __result = new HoverTip(
                __instance.Title,
                __result.Description + statsText,
                __result.Icon
            );
        }
    }
}