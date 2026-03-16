using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace RelicStatsTracker.Patches.Persistence;

/// <summary>
/// 数据持久化 Patches
/// 在遗物序列化/反序列化时保存/加载统计数据
/// </summary>
public static class PersistencePatches
{
    /// <summary>
    /// 在遗物序列化时保存统计数据
    /// </summary>
    [HarmonyPatch(typeof(RelicModel), nameof(RelicModel.ToSerializable))]
    public static class RelicModelToSerializablePatch
    {
        public static void Postfix(RelicModel __instance, SerializableRelic __result)
        {
            __result.Props ??= new SavedProperties();
            RelicStatsManager.SaveStatsToSavedProperties(__instance, __result.Props);
        }
    }

    /// <summary>
    /// 在遗物从序列化数据恢复时加载统计数据
    /// </summary>
    [HarmonyPatch(typeof(RelicModel), nameof(RelicModel.FromSerializable))]
    public static class RelicModelFromSerializablePatch
    {
        public static void Postfix(SerializableRelic save, RelicModel __result)
        {
            if (save.Props != null)
            {
                RelicStatsManager.LoadStatsToRuntime(__result, save.Props);
            }
        }
    }
}