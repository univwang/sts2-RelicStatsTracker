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
        public const string TriggerCount = "trigger_count";
        public const string TotalHeal = "total_heal";
        public const string TotalDamage = "total_damage";
        public const string TotalBlock = "total_block";
        public const string TotalEnergy = "total_energy";
        public const string StrengthGained = "strength_gained";
        public const string DexterityGained = "dexterity_gained";
        public const string CardsDrawn = "cards_drawn";
        public const string StatsHeader = "stats.header";
        // 自定义统计
        public const string DoubleDamageCount = "double_damage_count";
        public const string VulnerableApplied = "vulnerable_applied";
    }

    /// <summary>
    /// 默认本地化文本（英文）
    /// </summary>
    private static readonly Dictionary<string, string> EnglishTexts = new()
    {
        { Keys.TriggerCount, "[color=yellow]Triggered: {0}[/color]" },
        { Keys.TotalHeal, "[color=green]Total Heal: {0}[/color]" },
        { Keys.TotalDamage, "[color=red]Total Damage: {0}[/color]" },
        { Keys.TotalBlock, "[color=blue]Total Block: {0}[/color]" },
        { Keys.TotalEnergy, "[color=cyan]Total Energy: {0}[/color]" },
        { Keys.StrengthGained, "[color=red]Strength Gained: {0}[/color]" },
        { Keys.DexterityGained, "[color=blue]Dexterity Gained: {0}[/color]" },
        { Keys.CardsDrawn, "[color=white]Cards Drawn: {0}[/color]" },
        { Keys.StatsHeader, "\n\n[b]--- Stats ---[/b]" },
        { Keys.DoubleDamageCount, "[color=orange]Double Damage: {0}[/color]" },
        { Keys.VulnerableApplied, "[color=orange]Vulnerable Applied: {0}[/color]" }
    };

    /// <summary>
    /// 中文本地化文本
    /// </summary>
    private static readonly Dictionary<string, string> ChineseTexts = new()
    {
        { Keys.TriggerCount, "[color=yellow]触发次数: {0}[/color]" },
        { Keys.TotalHeal, "[color=green]总治疗量: {0}[/color]" },
        { Keys.TotalDamage, "[color=red]总伤害量: {0}[/color]" },
        { Keys.TotalBlock, "[color=blue]总格挡量: {0}[/color]" },
        { Keys.TotalEnergy, "[color=cyan]总能量: {0}[/color]" },
        { Keys.StrengthGained, "[color=red]获得力量: {0}[/color]" },
        { Keys.DexterityGained, "[color=blue]获得敏捷: {0}[/color]" },
        { Keys.CardsDrawn, "[color=white]抽牌数: {0}[/color]" },
        { Keys.StatsHeader, "\n\n[b]--- 统计 ---[/b]" },
        { Keys.DoubleDamageCount, "[color=orange]双倍伤害: {0}次[/color]" },
        { Keys.VulnerableApplied, "[color=orange]施加易伤: {0}层[/color]" }
    };

    /// <summary>
    /// 获取本地化文本
    /// </summary>
    public static string GetText(string key, params object[] args)
    {
        // 尝试从游戏本地化系统获取
        if (TryGetGameLocalization(key, out string? gameText))
        {
            return string.Format(gameText, args);
        }

        // 根据当前语言选择默认文本
        var texts = GetCurrentLanguageTexts();
        if (texts.TryGetValue(key, out string? text))
        {
            return string.Format(text, args);
        }

        // 返回键名作为后备
        return $"{key}: {string.Join(", ", args)}";
    }

    /// <summary>
    /// 尝试从游戏本地化系统获取文本
    /// </summary>
    private static bool TryGetGameLocalization(string key, out string? text)
    {
        text = null;
        try
        {
            var locString = LocString.GetIfExists(LocTable, key);
            if (locString != null)
            {
                text = locString.GetFormattedText();
                return !string.IsNullOrEmpty(text);
            }
        }
        catch
        {
            // 忽略本地化获取失败
        }
        return false;
    }

    /// <summary>
    /// 获取当前语言的文本字典
    /// </summary>
    private static Dictionary<string, string> GetCurrentLanguageTexts()
    {
        // 检查当前语言设置
        var currentLang = GetCurrentLanguage();
        return currentLang switch
        {
            "zhs" or "zht" or "chi" => ChineseTexts,
            _ => EnglishTexts
        };
    }

    /// <summary>
    /// 获取当前语言代码
    /// </summary>
    private static string GetCurrentLanguage()
    {
        try
        {
            // 尝试从游戏获取当前语言
            var locale = LocManager.Instance?.Language;
            return locale ?? "eng";
        }
        catch
        {
            return "eng";
        }
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
            "double_damage_count" => GetText(Keys.DoubleDamageCount, value),
            "vulnerable_applied" => GetText(Keys.VulnerableApplied, value),
            _ => $"[color=gray]{statName}: {value}[/color]"
        };
    }
}