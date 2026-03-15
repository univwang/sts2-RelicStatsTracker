using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Saves.Runs;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace RelicStatsTracker;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    internal const string ModId = "RelicStatsTracker";

    public static Logger Logger { get; } =
        new(ModId, LogType.Generic);

    public static void Initialize()
    {
        // 注册遗物统计数据属性名到 SavedPropertiesTypeCache
        // 这样 SavedProperties 的网络序列化就可以使用这些属性名
        SavedPropertiesTypeCache.InjectTypeIntoCache(typeof(RelicStatsProperties));
        Logger.Info("[RelicStats] Registered stats properties to SavedPropertiesTypeCache.");

        Harmony harmony = new(ModId);
        harmony.PatchAll(Assembly.GetExecutingAssembly());
        Logger.Info("RelicStatsTracker mod initialized! Tracking relic trigger statistics.");
    }
}