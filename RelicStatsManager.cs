using System.Collections.Generic;
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
        }

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
            switch (kvp.Key)
            {
                case RelicStatType.Heal:
                    stats.TotalHealAmount += kvp.Value;
                    break;
                case RelicStatType.Damage:
                    stats.TotalDamageAmount += kvp.Value;
                    break;
                case RelicStatType.Block:
                    stats.TotalBlockAmount += kvp.Value;
                    break;
                case RelicStatType.Energy:
                    stats.TotalEnergyGained += kvp.Value;
                    break;
                case RelicStatType.Strength:
                    stats.StrengthGained += kvp.Value;
                    break;
                case RelicStatType.Dexterity:
                    stats.DexterityGained += kvp.Value;
                    break;
                case RelicStatType.CardsDrawn:
                    stats.CardsDrawn += kvp.Value;
                    break;
            }
        }

        LogStats(relic, stats);
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
    /// 将统计数据保存到 SerializableRelic
    /// </summary>
    public static void SaveStatsToSerializableRelic(RelicModel relic, SerializableRelic serializableRelic)
    {
        var stats = GetStats(relic);
        if (!stats.HasAnyStats()) return;

        serializableRelic.Props ??= new SavedProperties();
        serializableRelic.Props.ints ??= new List<SavedProperties.SavedProperty<int>>();

        if (stats.TriggerCount > 0)
        {
            serializableRelic.Props.ints.Add(new SavedProperties.SavedProperty<int>("relic_stats_trigger_count", stats.TriggerCount));
        }

        if (stats.TotalHealAmount > 0)
        {
            serializableRelic.Props.ints.Add(new SavedProperties.SavedProperty<int>("relic_stats_total_heal", stats.TotalHealAmount));
        }

        if (stats.TotalDamageAmount > 0)
        {
            serializableRelic.Props.ints.Add(new SavedProperties.SavedProperty<int>("relic_stats_total_damage", stats.TotalDamageAmount));
        }

        if (stats.TotalBlockAmount > 0)
        {
            serializableRelic.Props.ints.Add(new SavedProperties.SavedProperty<int>("relic_stats_total_block", stats.TotalBlockAmount));
        }

        if (stats.TotalEnergyGained > 0)
        {
            serializableRelic.Props.ints.Add(new SavedProperties.SavedProperty<int>("relic_stats_total_energy", stats.TotalEnergyGained));
        }

        if (stats.StrengthGained > 0)
        {
            serializableRelic.Props.ints.Add(new SavedProperties.SavedProperty<int>("relic_stats_strength", stats.StrengthGained));
        }

        if (stats.DexterityGained > 0)
        {
            serializableRelic.Props.ints.Add(new SavedProperties.SavedProperty<int>("relic_stats_dexterity", stats.DexterityGained));
        }

        if (stats.CardsDrawn > 0)
        {
            serializableRelic.Props.ints.Add(new SavedProperties.SavedProperty<int>("relic_stats_cards_drawn", stats.CardsDrawn));
        }
    }

    /// <summary>
    /// 从 SerializableRelic 加载统计数据
    /// </summary>
    public static RelicStatsData? LoadStatsFromSerializableRelic(SerializableRelic serializableRelic)
    {
        if (serializableRelic.Props?.ints == null) return null;

        var stats = new RelicStatsData();
        bool found = false;

        foreach (var prop in serializableRelic.Props.ints)
        {
            switch (prop.name)
            {
                case "relic_stats_trigger_count":
                    stats.TriggerCount = prop.value;
                    found = true;
                    break;
                case "relic_stats_total_heal":
                    stats.TotalHealAmount = prop.value;
                    found = true;
                    break;
                case "relic_stats_total_damage":
                    stats.TotalDamageAmount = prop.value;
                    found = true;
                    break;
                case "relic_stats_total_block":
                    stats.TotalBlockAmount = prop.value;
                    found = true;
                    break;
                case "relic_stats_total_energy":
                    stats.TotalEnergyGained = prop.value;
                    found = true;
                    break;
                case "relic_stats_strength":
                    stats.StrengthGained = prop.value;
                    found = true;
                    break;
                case "relic_stats_dexterity":
                    stats.DexterityGained = prop.value;
                    found = true;
                    break;
                case "relic_stats_cards_drawn":
                    stats.CardsDrawn = prop.value;
                    found = true;
                    break;
            }
        }

        return found ? stats : null;
    }

    #endregion
}