using MegaCrit.Sts2.Core.Saves.Runs;

namespace RelicStatsTracker;

/// <summary>
/// 遗物统计数据属性注册类
/// 用于向 SavedPropertiesTypeCache 注册属性名
/// 这样 SavedProperties 的网络序列化就可以使用这些属性名
/// </summary>
public class RelicStatsProperties
{
    // 基础统计
    [SavedProperty]
    public int RelicStatsTriggerCount { get; set; }

    [SavedProperty]
    public int RelicStatsTotalHeal { get; set; }

    [SavedProperty]
    public int RelicStatsTotalDamage { get; set; }

    [SavedProperty]
    public int RelicStatsTotalBlock { get; set; }

    [SavedProperty]
    public int RelicStatsTotalEnergy { get; set; }

    [SavedProperty]
    public int RelicStatsStrength { get; set; }

    [SavedProperty]
    public int RelicStatsDexterity { get; set; }

    [SavedProperty]
    public int RelicStatsCardsDrawn { get; set; }

    // 新增统计
    [SavedProperty]
    public int RelicStatsVigor { get; set; }

    [SavedProperty]
    public int RelicStatsFocus { get; set; }

    [SavedProperty]
    public int RelicStatsPlating { get; set; }

    [SavedProperty]
    public int RelicStatsForge { get; set; }

    [SavedProperty]
    public int RelicStatsStars { get; set; }

    [SavedProperty]
    public int RelicStatsMaxHp { get; set; }

    [SavedProperty]
    public int RelicStatsOrbs { get; set; }

    [SavedProperty]
    public int RelicStatsSummons { get; set; }

    [SavedProperty]
    public int RelicStatsBlockDoubled { get; set; }

    [SavedProperty]
    public int RelicStatsCardsUpgraded { get; set; }

    [SavedProperty]
    public int RelicStatsCardsObtained { get; set; }

    [SavedProperty]
    public int RelicStatsGold { get; set; }

    // 自定义统计数据（JSON 格式）
    [SavedProperty]
    public string? RelicStatsCustom { get; set; }
}