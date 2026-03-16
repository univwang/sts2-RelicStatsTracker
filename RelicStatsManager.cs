using System.Collections.Generic;
using System.Text.Json;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace RelicStatsTracker;

/// <summary>
/// 遗物统计数据管理器
/// 管理当前局的遗物统计数据（运行时存储）
/// </summary>
public static class RelicStatsManager
{
    /// <summary>
    /// 当前局的统计数据（运行时）
    /// Key: 遗物的唯一标识符（ModelId.Entry）
    /// </summary>
    private static readonly Dictionary<string, RelicStatsData> _currentRunStats = new();

    /// <summary>
    /// 获取遗物的统计数据
    /// </summary>
    public static RelicStatsData GetStats(RelicModel relic)
    {
        string key = GetRelicKey(relic);
        return GetStats(key);
    }

    /// <summary>
    /// 获取遗物的统计数据
    /// </summary>
    public static RelicStatsData GetStats(string key)
    {
        if (!_currentRunStats.TryGetValue(key, out var stats))
        {
            stats = new RelicStatsData();
            _currentRunStats[key] = stats;
        }
        return stats;
    }

    /// <summary>
    /// 记录触发事件（简单触发计数）
    /// </summary>
    public static void RecordTrigger(RelicModel relic)
    {
        var stats = GetStats(relic);
        stats.TriggerCount++;
        LogStats(relic, stats);
    }

    /// <summary>
    /// 记录触发事件（带数值）
    /// </summary>
    public static void RecordTrigger(RelicModel relic, RelicStatType statType, int amount = 0)
    {
        var stats = GetStats(relic);
        stats.TriggerCount++;

        ApplyStatType(stats, statType, amount);

        LogStats(relic, stats);
    }

    /// <summary>
    /// 记录多种统计数据
    /// </summary>
    public static void RecordTrigger(RelicModel relic, Dictionary<RelicStatType, int> statsToUpdate)
    {
        var stats = GetStats(relic);
        stats.TriggerCount++;

        foreach (var kvp in statsToUpdate)
        {
            ApplyStatType(stats, kvp.Key, kvp.Value);
        }

        LogStats(relic, stats);
    }

    /// <summary>
    /// 应用统计类型到数据
    /// </summary>
    private static void ApplyStatType(RelicStatsData stats, RelicStatType statType, int amount)
    {
        switch (statType)
        {
            case RelicStatType.Heal:
                stats.TotalHealAmount += amount;
                break;
            case RelicStatType.Damage:
                stats.TotalDamageAmount += amount;
                break;
            case RelicStatType.Block:
                stats.TotalBlockAmount += amount;
                break;
            case RelicStatType.Energy:
                stats.TotalEnergyGained += amount;
                break;
            case RelicStatType.Strength:
                stats.StrengthGained += amount;
                break;
            case RelicStatType.Dexterity:
                stats.DexterityGained += amount;
                break;
            case RelicStatType.CardsDrawn:
                stats.CardsDrawn += amount;
                break;
            case RelicStatType.Vigor:
                stats.TotalVigorGained += amount;
                break;
            case RelicStatType.Focus:
                stats.TotalFocusGained += amount;
                break;
            case RelicStatType.Plating:
                stats.TotalPlatingGained += amount;
                break;
            case RelicStatType.Forge:
                stats.TotalForgeGained += amount;
                break;
            case RelicStatType.Stars:
                stats.TotalStarsGained += amount;
                break;
            case RelicStatType.MaxHp:
                stats.TotalMaxHpGained += amount;
                break;
            case RelicStatType.OrbChanneled:
                stats.OrbsChanneled += amount;
                break;
            case RelicStatType.Summon:
                stats.SummonsCount += amount;
                break;
            case RelicStatType.BlockDoubled:
                stats.BlockDoubledCount += amount;
                break;
            case RelicStatType.CardsUpgraded:
                stats.CardsUpgraded += amount;
                break;
            case RelicStatType.CardsObtained:
                stats.CardsObtained += amount;
                break;
            case RelicStatType.Gold:
                stats.GoldGained += amount;
                break;
        }
    }

    /// <summary>
    /// 记录自定义统计数据
    /// </summary>
    public static void RecordCustomStat(RelicModel relic, string statName, int amount = 1)
    {
        var stats = GetStats(relic);
        if (!stats.CustomStats.ContainsKey(statName))
        {
            stats.CustomStats[statName] = 0;
        }
        stats.CustomStats[statName] += amount;
        stats.TriggerCount++;
        LogStats(relic, stats);
    }

    /// <summary>
    /// 清除当前局的统计数据
    /// </summary>
    public static void ClearCurrentRun()
    {
        _currentRunStats.Clear();
        MainFile.Logger.Info("[RelicStats] Cleared all stats for new run.");
    }

    /// <summary>
    /// 获取遗物的唯一标识键
    /// </summary>
    private static string GetRelicKey(RelicModel relic)
    {
        return relic.Id.Entry;
    }

    /// <summary>
    /// 输出统计日志
    /// </summary>
    private static void LogStats(RelicModel relic, RelicStatsData stats)
    {
        MainFile.Logger.Info($"[RelicStats] {relic.Id.Entry} triggered! " +
            $"Count: {stats.TriggerCount}, " +
            $"Heal: {stats.TotalHealAmount}, " +
            $"Damage: {stats.TotalDamageAmount}, " +
            $"Block: {stats.TotalBlockAmount}, " +
            $"Energy: {stats.TotalEnergyGained}");
    }

    #region 持久化支持

    /// <summary>
    /// 将统计数据保存到 SavedProperties
    /// </summary>
    public static void SaveStatsToSavedProperties(RelicModel relic, SavedProperties props)
    {
        var stats = GetStats(relic);
        if (!stats.HasAnyStats()) return;

        props.ints ??= new List<SavedProperties.SavedProperty<int>>();

        if (stats.TriggerCount > 0)
            props.ints.Add(new SavedProperties.SavedProperty<int>("RelicStatsTriggerCount", stats.TriggerCount));

        if (stats.TotalHealAmount > 0)
            props.ints.Add(new SavedProperties.SavedProperty<int>("RelicStatsTotalHeal", stats.TotalHealAmount));

        if (stats.TotalDamageAmount > 0)
            props.ints.Add(new SavedProperties.SavedProperty<int>("RelicStatsTotalDamage", stats.TotalDamageAmount));

        if (stats.TotalBlockAmount > 0)
            props.ints.Add(new SavedProperties.SavedProperty<int>("RelicStatsTotalBlock", stats.TotalBlockAmount));

        if (stats.TotalEnergyGained > 0)
            props.ints.Add(new SavedProperties.SavedProperty<int>("RelicStatsTotalEnergy", stats.TotalEnergyGained));

        if (stats.StrengthGained > 0)
            props.ints.Add(new SavedProperties.SavedProperty<int>("RelicStatsStrength", stats.StrengthGained));

        if (stats.DexterityGained > 0)
            props.ints.Add(new SavedProperties.SavedProperty<int>("RelicStatsDexterity", stats.DexterityGained));

        if (stats.CardsDrawn > 0)
            props.ints.Add(new SavedProperties.SavedProperty<int>("RelicStatsCardsDrawn", stats.CardsDrawn));

        if (stats.TotalVigorGained > 0)
            props.ints.Add(new SavedProperties.SavedProperty<int>("RelicStatsVigor", stats.TotalVigorGained));

        if (stats.TotalFocusGained > 0)
            props.ints.Add(new SavedProperties.SavedProperty<int>("RelicStatsFocus", stats.TotalFocusGained));

        if (stats.TotalPlatingGained > 0)
            props.ints.Add(new SavedProperties.SavedProperty<int>("RelicStatsPlating", stats.TotalPlatingGained));

        if (stats.TotalForgeGained > 0)
            props.ints.Add(new SavedProperties.SavedProperty<int>("RelicStatsForge", stats.TotalForgeGained));

        if (stats.TotalStarsGained > 0)
            props.ints.Add(new SavedProperties.SavedProperty<int>("RelicStatsStars", stats.TotalStarsGained));

        if (stats.TotalMaxHpGained > 0)
            props.ints.Add(new SavedProperties.SavedProperty<int>("RelicStatsMaxHp", stats.TotalMaxHpGained));

        if (stats.OrbsChanneled > 0)
            props.ints.Add(new SavedProperties.SavedProperty<int>("RelicStatsOrbs", stats.OrbsChanneled));

        if (stats.SummonsCount > 0)
            props.ints.Add(new SavedProperties.SavedProperty<int>("RelicStatsSummons", stats.SummonsCount));

        if (stats.BlockDoubledCount > 0)
            props.ints.Add(new SavedProperties.SavedProperty<int>("RelicStatsBlockDoubled", stats.BlockDoubledCount));

        if (stats.CardsUpgraded > 0)
            props.ints.Add(new SavedProperties.SavedProperty<int>("RelicStatsCardsUpgraded", stats.CardsUpgraded));

        if (stats.CardsObtained > 0)
            props.ints.Add(new SavedProperties.SavedProperty<int>("RelicStatsCardsObtained", stats.CardsObtained));

        if (stats.GoldGained > 0)
            props.ints.Add(new SavedProperties.SavedProperty<int>("RelicStatsGold", stats.GoldGained));

        // 自定义统计数据序列化为 JSON
        if (stats.CustomStats.Count > 0)
        {
            var customJson = JsonSerializer.Serialize(stats.CustomStats);
            props.strings ??= new List<SavedProperties.SavedProperty<string>>();
            props.strings.Add(new SavedProperties.SavedProperty<string>("RelicStatsCustom", customJson));
        }
    }

    /// <summary>
    /// 从 SavedProperties 加载统计数据
    /// </summary>
    public static RelicStatsData? LoadStatsFromSavedProperties(SavedProperties props)
    {
        if (props.ints == null && props.strings == null) return null;

        var stats = new RelicStatsData();
        bool found = false;

        if (props.ints != null)
        {
            foreach (var prop in props.ints)
            {
                switch (prop.name)
                {
                    case "RelicStatsTriggerCount":
                        stats.TriggerCount = prop.value;
                        found = true;
                        break;
                    case "RelicStatsTotalHeal":
                        stats.TotalHealAmount = prop.value;
                        found = true;
                        break;
                    case "RelicStatsTotalDamage":
                        stats.TotalDamageAmount = prop.value;
                        found = true;
                        break;
                    case "RelicStatsTotalBlock":
                        stats.TotalBlockAmount = prop.value;
                        found = true;
                        break;
                    case "RelicStatsTotalEnergy":
                        stats.TotalEnergyGained = prop.value;
                        found = true;
                        break;
                    case "RelicStatsStrength":
                        stats.StrengthGained = prop.value;
                        found = true;
                        break;
                    case "RelicStatsDexterity":
                        stats.DexterityGained = prop.value;
                        found = true;
                        break;
                    case "RelicStatsCardsDrawn":
                        stats.CardsDrawn = prop.value;
                        found = true;
                        break;
                    case "RelicStatsVigor":
                        stats.TotalVigorGained = prop.value;
                        found = true;
                        break;
                    case "RelicStatsFocus":
                        stats.TotalFocusGained = prop.value;
                        found = true;
                        break;
                    case "RelicStatsPlating":
                        stats.TotalPlatingGained = prop.value;
                        found = true;
                        break;
                    case "RelicStatsForge":
                        stats.TotalForgeGained = prop.value;
                        found = true;
                        break;
                    case "RelicStatsStars":
                        stats.TotalStarsGained = prop.value;
                        found = true;
                        break;
                    case "RelicStatsMaxHp":
                        stats.TotalMaxHpGained = prop.value;
                        found = true;
                        break;
                    case "RelicStatsOrbs":
                        stats.OrbsChanneled = prop.value;
                        found = true;
                        break;
                    case "RelicStatsSummons":
                        stats.SummonsCount = prop.value;
                        found = true;
                        break;
                    case "RelicStatsBlockDoubled":
                        stats.BlockDoubledCount = prop.value;
                        found = true;
                        break;
                    case "RelicStatsCardsUpgraded":
                        stats.CardsUpgraded = prop.value;
                        found = true;
                        break;
                    case "RelicStatsCardsObtained":
                        stats.CardsObtained = prop.value;
                        found = true;
                        break;
                    case "RelicStatsGold":
                        stats.GoldGained = prop.value;
                        found = true;
                        break;
                }
            }
        }

        // 加载自定义统计数据
        if (props.strings != null)
        {
            foreach (var prop in props.strings)
            {
                if (prop.name == "RelicStatsCustom")
                {
                    try
                    {
                        var customStats = JsonSerializer.Deserialize<Dictionary<string, int>>(prop.value);
                        if (customStats != null)
                        {
                            stats.CustomStats = customStats;
                            found = true;
                        }
                    }
                    catch
                    {
                        // 忽略反序列化错误
                    }
                }
            }
        }

        return found ? stats : null;
    }

    /// <summary>
    /// 将统计数据加载到运行时缓存
    /// </summary>
    public static void LoadStatsToRuntime(RelicModel relic, SavedProperties props)
    {
        var stats = LoadStatsFromSavedProperties(props);
        if (stats != null)
        {
            string key = GetRelicKey(relic);
            _currentRunStats[key] = stats;
            MainFile.Logger.Info($"[RelicStats] Loaded stats for {relic.Id.Entry}: TriggerCount={stats.TriggerCount}");
        }
    }

    #endregion
}