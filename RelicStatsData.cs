using System.Collections.Generic;

namespace RelicStatsTracker;

/// <summary>
/// 遗物统计数据结构
/// 支持多种统计类型的追踪
/// </summary>
public class RelicStatsData
{
    /// <summary>
    /// 触发次数
    /// </summary>
    public int TriggerCount { get; set; }

    /// <summary>
    /// 总治疗量
    /// </summary>
    public int TotalHealAmount { get; set; }

    /// <summary>
    /// 总伤害量
    /// </summary>
    public int TotalDamageAmount { get; set; }

    /// <summary>
    /// 总格挡量
    /// </summary>
    public int TotalBlockAmount { get; set; }

    /// <summary>
    /// 总能量获取
    /// </summary>
    public int TotalEnergyGained { get; set; }

    /// <summary>
    /// 力量获取次数
    /// </summary>
    public int StrengthGained { get; set; }

    /// <summary>
    /// 敏捷获取次数
    /// </summary>
    public int DexterityGained { get; set; }

    /// <summary>
    /// 抽牌数量
    /// </summary>
    public int CardsDrawn { get; set; }

    /// <summary>
    /// 自定义统计数据
    /// Key: 统计项名称
    /// Value: 统计值
    /// </summary>
    public Dictionary<string, int> CustomStats { get; set; } = new();

    /// <summary>
    /// 重置统计数据
    /// </summary>
    public void Reset()
    {
        TriggerCount = 0;
        TotalHealAmount = 0;
        TotalDamageAmount = 0;
        TotalBlockAmount = 0;
        TotalEnergyGained = 0;
        StrengthGained = 0;
        DexterityGained = 0;
        CardsDrawn = 0;
        CustomStats.Clear();
    }

    /// <summary>
    /// 是否有任何统计数据
    /// </summary>
    public bool HasAnyStats()
    {
        return TriggerCount > 0
            || TotalHealAmount > 0
            || TotalDamageAmount > 0
            || TotalBlockAmount > 0
            || TotalEnergyGained > 0
            || StrengthGained > 0
            || DexterityGained > 0
            || CardsDrawn > 0
            || CustomStats.Count > 0;
    }
}

/// <summary>
/// 遗物统计类型枚举
/// 用于标识不同类型的统计更新
/// </summary>
public enum RelicStatType
{
    /// <summary>
    /// 触发次数
    /// </summary>
    Trigger,

    /// <summary>
    /// 治疗量
    /// </summary>
    Heal,

    /// <summary>
    /// 伤害量
    /// </summary>
    Damage,

    /// <summary>
    /// 格挡量
    /// </summary>
    Block,

    /// <summary>
    /// 能量获取
    /// </summary>
    Energy,

    /// <summary>
    /// 力量获取
    /// </summary>
    Strength,

    /// <summary>
    /// 敏捷获取
    /// </summary>
    Dexterity,

    /// <summary>
    /// 抽牌
    /// </summary>
    CardsDrawn
}