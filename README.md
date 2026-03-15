# RelicStatsTracker

**杀戮尖塔2 Mod - 遗物统计数据追踪器**

追踪遗物的触发次数和效果数值，并在遗物悬浮提示中显示统计数据。

## 功能特性

- 📊 **统计数据追踪**：追踪遗物的触发次数、治疗量、格挡量、能量获取等
- 💬 **悬浮提示增强**：在遗物悬浮提示中显示统计数据
- 🌐 **多语言支持**：支持中文和英文
- 🔄 **自动重置**：新局开始时自动清除上一局的统计数据

## 支持的遗物

### 治疗类
| 遗物 | 追踪数据 |
|------|----------|
| 燃烧之血 (BurningBlood) | 触发次数、总治疗量 |
| 黑血 (BlackBlood) | 触发次数、总治疗量 |
| 小血瓶 (BloodVial) | 触发次数、总治疗量 |

### 格挡类
| 遗物 | 追踪数据 |
|------|----------|
| 锚 (Anchor) | 触发次数、总格挡量 |
| 华丽扇 (OrnamentalFan) | 触发次数、总格挡量 |

### 能量类
| 遗物 | 追踪数据 |
|------|----------|
| 提灯 (Lantern) | 触发次数、总能量 |
| 孙子兵法 (ArtOfWar) | 触发次数、总能量 |
| 双节棍 (Nunchaku) | 触发次数、总能量 |

### 属性类
| 遗物 | 追踪数据 |
|------|----------|
| 手里剑 (Shuriken) | 触发次数、获得力量 |
| 苦无 (Kunai) | 触发次数、获得敏捷 |

### 其他
| 遗物 | 追踪数据 |
|------|----------|
| 百年谜题 (CentennialPuzzle) | 触发次数、抽牌数 |
| 笔尖 (PenNib) | 触发次数、双倍伤害次数 |
| 弹珠袋 (BagOfMarbles) | 触发次数、施加易伤 |

## 安装

1. 确保已安装 [杀戮尖塔2 Mod 加载器](https://github.com/MegaCrit/Sts2ModLoader)
2. 将编译好的 `RelicStatsTracker.dll` 和 `RelicStatsTracker.pck` 放入游戏的 `mods` 文件夹
3. 启动游戏即可

## 构建

### 前置要求

- .NET 9.0 SDK
- Godot 4.5.1 (用于导出 .pck 文件)

### 构建步骤

```bash
# 克隆仓库
git clone <repo-url>
cd RelicStatsTracker

# 构建
dotnet build

# 发布（会自动导出 .pck 文件）
dotnet publish
```

### 配置

如果游戏不在默认 Steam 路径，可以在项目文件中配置：

```xml
<PropertyGroup Condition="'$(IsWindows)' == 'true'">
    <Sts2Path>你的游戏路径</Sts2Path>
    <GodotPath>你的 Godot 路径</GodotPath>
</PropertyGroup>
```

## 项目结构

```
RelicStatsTracker/
├── MainFile.cs              # Mod 入口
├── RelicStatsData.cs        # 统计数据结构
├── RelicStatsManager.cs     # 统计数据管理器
├── Localization.cs          # 本地化支持
├── Patches.cs               # Harmony Patches
├── mod_manifest.json        # Mod 清单
├── project.godot            # Godot 项目配置
└── RelicStatsTracker/
    └── localization/
        ├── en.json          # 英文本地化
        └── zhs.json         # 中文本地化
```

## 技术实现

### 悬浮提示增强

通过 Harmony Patch 修改 `RelicModel.HoverTip` 属性，在描述后追加统计信息：

```csharp
[HarmonyPatch(typeof(RelicModel), nameof(RelicModel.HoverTip), MethodType.Getter)]
public static class RelicModel_HoverTip_Patch
{
    public static void Postfix(RelicModel __instance, ref HoverTip __result)
    {
        var stats = RelicStatsManager.GetStats(__instance);
        if (stats.HasAnyStats())
        {
            var statsText = Localization.BuildStatsText(stats);
            __result = new HoverTip(__instance.Title, __result.Description + statsText, __result.Icon);
        }
    }
}
```

### 遗物触发追踪

通过 Patch 各遗物的触发方法来记录统计数据：

```csharp
[HarmonyPatch(typeof(BurningBlood), nameof(BurningBlood.AfterCombatVictory))]
public static class BurningBlood_AfterCombatVictory_Patch
{
    public static void Postfix(BurningBlood __instance, CombatRoom _)
    {
        int healAmount = __instance.DynamicVars.Heal.IntValue;
        RelicStatsManager.RecordTrigger(__instance, RelicStatType.Heal, healAmount);
    }
}
```

## 扩展指南

### 添加新遗物追踪

1. 在 `Patches.cs` 中添加新的 Patch 类：

```csharp
[HarmonyPatch(typeof(YourRelic), nameof(YourRelic.TriggerMethod))]
public static class YourRelic_TriggerMethod_Patch
{
    public static void Postfix(YourRelic __instance, ...)
    {
        RelicStatsManager.RecordTrigger(__instance, RelicStatType.YourStatType, amount);
    }
}
```

2. 如果需要新的统计类型，在 `RelicStatsData.cs` 中添加字段，在 `RelicStatType` 枚举中添加类型。

3. 在本地化文件中添加对应的文本。

## 已知限制

- 统计数据仅在当前局有效，新局开始后会重置
- 不支持历史记录中的统计数据查看（游戏序列化系统限制）

## 许可证

MIT License

## 致谢

- [MegaCrit](https://www.megacrit.com/) - 杀戮尖塔2 开发商
- [Harmony](https://github.com/pardeike/Harmony) - .NET 运行时补丁库