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

    #region 基础统计

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
    /// 力量获取
    /// </summary>
    public int StrengthGained { get; set; }

    /// <summary>
    /// 敏捷获取
    /// </summary>
    public int DexterityGained { get; set; }

    /// <summary>
    /// 抽牌数量
    /// </summary>
    public int CardsDrawn { get; set; }

    #endregion

    #region 新增统计

    /// <summary>
    /// 活力值获取
    /// </summary>
    public int TotalVigorGained { get; set; }

    /// <summary>
    /// 集中值获取
    /// </summary>
    public int TotalFocusGained { get; set; }

    /// <summary>
    /// 覆甲值获取
    /// </summary>
    public int TotalPlatingGained { get; set; }

    /// <summary>
    /// 铸造值获取
    /// </summary>
    public int TotalForgeGained { get; set; }

    /// <summary>
    /// 星星值获取
    /// </summary>
    public int TotalStarsGained { get; set; }

    /// <summary>
    /// 最大生命值增加
    /// </summary>
    public int TotalMaxHpGained { get; set; }

    /// <summary>
    /// 引导球数
    /// </summary>
    public int OrbsChanneled { get; set; }

    /// <summary>
    /// 召唤次数
    /// </summary>
    public int SummonsCount { get; set; }

    /// <summary>
    /// 格挡翻倍次数
    /// </summary>
    public int BlockDoubledCount { get; set; }

    /// <summary>
    /// 升级卡牌数
    /// </summary>
    public int CardsUpgraded { get; set; }

    /// <summary>
    /// 获得卡牌数
    /// </summary>
    public int CardsObtained { get; set; }

    /// <summary>
    /// 金币获取
    /// </summary>
    public int GoldGained { get; set; }

    #endregion

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
        TotalVigorGained = 0;
        TotalFocusGained = 0;
        TotalPlatingGained = 0;
        TotalForgeGained = 0;
        TotalStarsGained = 0;
        TotalMaxHpGained = 0;
        OrbsChanneled = 0;
        SummonsCount = 0;
        BlockDoubledCount = 0;
        CardsUpgraded = 0;
        CardsObtained = 0;
        GoldGained = 0;
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
            || TotalVigorGained > 0
            || TotalFocusGained > 0
            || TotalPlatingGained > 0
            || TotalForgeGained > 0
            || TotalStarsGained > 0
            || TotalMaxHpGained > 0
            || OrbsChanneled > 0
            || SummonsCount > 0
            || BlockDoubledCount > 0
            || CardsUpgraded > 0
            || CardsObtained > 0
            || GoldGained > 0
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
    CardsDrawn,

    /// <summary>
    /// 活力值
    /// </summary>
    Vigor,

    /// <summary>
    /// 集中值
    /// </summary>
    Focus,

    /// <summary>
    /// 覆甲值
    /// </summary>
    Plating,

    /// <summary>
    /// 铸造值
    /// </summary>
    Forge,

    /// <summary>
    /// 星星值
    /// </summary>
    Stars,

    /// <summary>
    /// 最大生命值
    /// </summary>
    MaxHp,

    /// <summary>
    /// 引导球
    /// </summary>
    OrbChanneled,

    /// <summary>
    /// 召唤
    /// </summary>
    Summon,

    /// <summary>
    /// 格挡翻倍
    /// </summary>
    BlockDoubled,

    /// <summary>
    /// 升级卡牌
    /// </summary>
    CardsUpgraded,

    /// <summary>
    /// 获得卡牌
    /// </summary>
    CardsObtained,

    /// <summary>
    /// 金币
    /// </summary>
    Gold
}