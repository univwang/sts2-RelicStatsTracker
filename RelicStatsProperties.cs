using MegaCrit.Sts2.Core.Saves.Runs;

namespace RelicStatsTracker;

/// <summary>
/// 遗物统计数据属性注册类
/// 用于向 SavedPropertiesTypeCache 注册属性名
/// 这样 SavedProperties 的网络序列化就可以使用这些属性名
/// </summary>
public class RelicStatsProperties
{
    /// <summary>
    /// 触发次数
    /// </summary>
    [SavedProperty]
    public int RelicStatsTriggerCount { get; set; }

    /// <summary>
    /// 总治疗量
    /// </summary>
    [SavedProperty]
    public int RelicStatsTotalHeal { get; set; }

    /// <summary>
    /// 总伤害量
    /// </summary>
    [SavedProperty]
    public int RelicStatsTotalDamage { get; set; }

    /// <summary>
    /// 总格挡量
    /// </summary>
    [SavedProperty]
    public int RelicStatsTotalBlock { get; set; }

    /// <summary>
    /// 总能量获取
    /// </summary>
    [SavedProperty]
    public int RelicStatsTotalEnergy { get; set; }

    /// <summary>
    /// 力量获取次数
    /// </summary>
    [SavedProperty]
    public int RelicStatsStrength { get; set; }

    /// <summary>
    /// 敏捷获取次数
    /// </summary>
    [SavedProperty]
    public int RelicStatsDexterity { get; set; }

    /// <summary>
    /// 抽牌数量
    /// </summary>
    [SavedProperty]
    public int RelicStatsCardsDrawn { get; set; }

    /// <summary>
    /// 自定义统计数据（JSON 格式）
    /// </summary>
    [SavedProperty]
    public string? RelicStatsCustom { get; set; }
}