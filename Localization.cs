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
        public const string TriggerCount = "stats.trigger_count";
        public const string TotalHeal = "stats.total_heal";
        public const string TotalDamage = "stats.total_damage";
        public const string TotalBlock = "stats.total_block";
        public const string TotalEnergy = "stats.total_energy";
        public const string StrengthGained = "stats.strength_gained";
        public const string DexterityGained = "stats.dexterity_gained";
        public const string CardsDrawn = "stats.cards_drawn";
        public const string StatsHeader = "stats.header";
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