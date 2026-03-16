using System.Collections.Generic;
using MegaCrit.Sts2.Core.Localization;

namespace RelicStatsTracker;

/// <summary>
/// 本地化支持类
/// 提供统计文本的多语言支持
/// </summary>
public static class Localization
{
    private const string LocTable = "relics";

    /// <summary>
    /// 本地化键名
    /// </summary>
    public static class Keys
    {
        // 基础统计
        public const string TriggerCount = "stats.trigger_count";
        public const string TotalHeal = "stats.total_heal";
        public const string TotalDamage = "stats.total_damage";
        public const string TotalBlock = "stats.total_block";
        public const string TotalEnergy = "stats.total_energy";
        public const string StrengthGained = "stats.strength_gained";
        public const string DexterityGained = "stats.dexterity_gained";
        public const string CardsDrawn = "stats.cards_drawn";

        // 新增统计
        public const string TotalVigor = "stats.total_vigor";
        public const string TotalFocus = "stats.total_focus";
        public const string TotalPlating = "stats.total_plating";
        public const string TotalForge = "stats.total_forge";
        public const string TotalStars = "stats.total_stars";
        public const string TotalMaxHp = "stats.total_max_hp";
        public const string OrbsChanneled = "stats.orbs_channeled";
        public const string SummonsCount = "stats.summons_count";
        public const string BlockDoubled = "stats.block_doubled";
        public const string CardsUpgraded = "stats.cards_upgraded";
        public const string CardsObtained = "stats.cards_obtained";
        public const string GoldGained = "stats.gold_gained";

        // 标题
        public const string StatsHeader = "stats.header";

        // 自定义统计
        public const string DoubleDamage = "stats.double_damage";
        public const string VulnerableApplied = "stats.vulnerable_applied";
    }

    /// <summary>
    /// 获取本地化文本
    /// </summary>
    public static string GetText(string key, params object[] args)
    {
        try
        {
            var locString = LocString.GetIfExists(LocTable, key);
            if (locString != null)
            {
                var rawText = locString.GetRawText();
                if (!string.IsNullOrEmpty(rawText))
                {
                    return string.Format(rawText, args);
                }
            }
        }
        catch
        {
            // 忽略本地化获取失败
        }

        // 返回键名作为后备
        return $"[{key}: {string.Join(", ", args)}]";
    }

    /// <summary>
    /// 构建统计信息文本
    /// </summary>
    public static string BuildStatsText(RelicStatsData stats)
    {
        var lines = new List<string>();

        // 添加统计标题
        lines.Add(GetText(Keys.StatsHeader));

        // 触发次数
        if (stats.TriggerCount > 0)
        {
            lines.Add(GetText(Keys.TriggerCount, stats.TriggerCount));
        }

        // 治疗量
        if (stats.TotalHealAmount > 0)
        {
            lines.Add(GetText(Keys.TotalHeal, stats.TotalHealAmount));
        }

        // 伤害量
        if (stats.TotalDamageAmount > 0)
        {
            lines.Add(GetText(Keys.TotalDamage, stats.TotalDamageAmount));
        }

        // 格挡量
        if (stats.TotalBlockAmount > 0)
        {
            lines.Add(GetText(Keys.TotalBlock, stats.TotalBlockAmount));
        }

        // 能量
        if (stats.TotalEnergyGained > 0)
        {
            lines.Add(GetText(Keys.TotalEnergy, stats.TotalEnergyGained));
        }

        // 力量
        if (stats.StrengthGained > 0)
        {
            lines.Add(GetText(Keys.StrengthGained, stats.StrengthGained));
        }

        // 敏捷
        if (stats.DexterityGained > 0)
        {
            lines.Add(GetText(Keys.DexterityGained, stats.DexterityGained));
        }

        // 抽牌
        if (stats.CardsDrawn > 0)
        {
            lines.Add(GetText(Keys.CardsDrawn, stats.CardsDrawn));
        }

        // 活力值
        if (stats.TotalVigorGained > 0)
        {
            lines.Add(GetText(Keys.TotalVigor, stats.TotalVigorGained));
        }

        // 集中值
        if (stats.TotalFocusGained > 0)
        {
            lines.Add(GetText(Keys.TotalFocus, stats.TotalFocusGained));
        }

        // 覆甲值
        if (stats.TotalPlatingGained > 0)
        {
            lines.Add(GetText(Keys.TotalPlating, stats.TotalPlatingGained));
        }

        // 铸造值
        if (stats.TotalForgeGained > 0)
        {
            lines.Add(GetText(Keys.TotalForge, stats.TotalForgeGained));
        }

        // 星星值
        if (stats.TotalStarsGained > 0)
        {
            lines.Add(GetText(Keys.TotalStars, stats.TotalStarsGained));
        }

        // 最大生命值
        if (stats.TotalMaxHpGained > 0)
        {
            lines.Add(GetText(Keys.TotalMaxHp, stats.TotalMaxHpGained));
        }

        // 引导球
        if (stats.OrbsChanneled > 0)
        {
            lines.Add(GetText(Keys.OrbsChanneled, stats.OrbsChanneled));
        }

        // 召唤次数
        if (stats.SummonsCount > 0)
        {
            lines.Add(GetText(Keys.SummonsCount, stats.SummonsCount));
        }

        // 格挡翻倍
        if (stats.BlockDoubledCount > 0)
        {
            lines.Add(GetText(Keys.BlockDoubled, stats.BlockDoubledCount));
        }

        // 升级卡牌
        if (stats.CardsUpgraded > 0)
        {
            lines.Add(GetText(Keys.CardsUpgraded, stats.CardsUpgraded));
        }

        // 获得卡牌
        if (stats.CardsObtained > 0)
        {
            lines.Add(GetText(Keys.CardsObtained, stats.CardsObtained));
        }

        // 金币
        if (stats.GoldGained > 0)
        {
            lines.Add(GetText(Keys.GoldGained, stats.GoldGained));
        }

        // 自定义统计数据
        if (stats.CustomStats.Count > 0)
        {
            foreach (var kvp in stats.CustomStats)
            {
                var customText = GetCustomStatText(kvp.Key, kvp.Value);
                if (!string.IsNullOrEmpty(customText))
                {
                    lines.Add(customText);
                }
            }
        }

        return string.Join("\n", lines);
    }

    /// <summary>
    /// 获取自定义统计文本
    /// </summary>
    private static string GetCustomStatText(string statName, int value)
    {
        return statName switch
        {
            "double_damage_count" => GetText(Keys.DoubleDamage, value),
            "vulnerable_applied" => GetText(Keys.VulnerableApplied, value),
            _ => $"[color=gray]{statName}: {value}[/color]"
        };
    }
}