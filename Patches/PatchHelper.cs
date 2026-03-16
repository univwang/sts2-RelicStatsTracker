using System.Reflection;

namespace RelicStatsTracker.Patches;

/// <summary>
/// Patch 辅助方法
/// </summary>
public static class PatchHelper
{
    /// <summary>
    /// 获取私有字段值
    /// </summary>
    public static T? GetPrivateField<T>(object obj, string fieldName)
    {
        var field = obj.GetType().GetField(fieldName,
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        return field != null ? (T?)field.GetValue(obj) : default;
    }

    /// <summary>
    /// 设置私有字段值
    /// </summary>
    public static void SetPrivateField(object obj, string fieldName, object? value)
    {
        var field = obj.GetType().GetField(fieldName,
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        field?.SetValue(obj, value);
    }

    /// <summary>
    /// 获取私有属性值
    /// </summary>
    public static T? GetPrivateProperty<T>(object obj, string propertyName)
    {
        var property = obj.GetType().GetProperty(propertyName,
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        return property != null ? (T?)property.GetValue(obj) : default;
    }
}