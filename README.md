# RelicStatsTracker

**杀戮尖塔2 Mod - 遗物统计数据追踪器**

追踪遗物的触发次数和效果数值，并在遗物悬浮提示中显示统计数据。

## 功能特性

- 📊 **统计数据追踪**：追踪遗物的触发次数、治疗量、格挡量、能量获取等
- 💬 **悬浮提示增强**：在遗物悬浮提示中显示统计数据
- 💾 **数据持久化**：保存退出游戏后，再回来能够正确读取遗物的统计数据
- 📜 **历史记录支持**：一局游戏成功或失败后能够在历史记录栏查看遗物统计信息
- 🌐 **多语言支持**：支持中文和英文，可通过本地化文件自定义
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
├── RelicStatsProperties.cs  # 属性注册类（用于持久化）
├── Localization.cs          # 本地化支持
├── Patches.cs               # Harmony Patches
├── mod_manifest.json        # Mod 清单
├── project.godot            # Godot 项目配置
└── RelicStatsTracker/       # 打包资源目录
    └── localization/
        ├── eng/
        │   └── relics.json  # 英文本地化
        └── zhs/
            └── relics.json  # 中文本地化
```

## 本地化

### 本地化文件格式

本地化文件使用 JSON 格式，键名使用 `stats.` 前缀：

```json
{
  "stats.header": "\n——Stats——",
  "stats.trigger_count": "[color=yellow]Triggered: {0}[/color]",
  "stats.total_heal": "[color=green]Total Heal: {0}[/color]",
  "stats.total_damage": "[color=red]Total Damage: {0}[/color]",
  "stats.total_block": "[color=blue]Total Block: {0}[/color]",
  "stats.total_energy": "[color=cyan]Total Energy: {0}[/color]",
  "stats.strength_gained": "[color=red]Strength Gained: {0}[/color]",
  "stats.dexterity_gained": "[color=blue]Dexterity Gained: {0}[/color]",
  "stats.cards_drawn": "[color=white]Cards Drawn: {0}[/color]",
  "stats.double_damage": "[color=orange]Double Damage: {0}[/color]",
  "stats.vulnerable_applied": "[color=orange]Vulnerable Applied: {0}[/color]"
}
```

### 支持的语言

| 语言代码 | 目录名 |
|----------|--------|
| English | `eng` |
| 简体中文 | `zhs` |

### 添加新语言

1. 在 `RelicStatsTracker/localization/` 下创建新的语言目录（如 `jpn`）
2. 创建 `relics.json` 文件并翻译所有键值
3. 重新构建 Mod

### BBCode 格式化

本地化文本支持 BBCode 格式化：

| 标签 | 效果 |
|------|------|
| `[color=red]...[/color]` | 红色文本 |
| `[color=green]...[/color]` | 绿色文本 |
| `[color=blue]...[/color]` | 蓝色文本 |
| `[color=yellow]...[/color]` | 黄色文本 |
| `[color=cyan]...[/color]` | 青色文本 |
| `[color=orange]...[/color]` | 橙色文本 |
| `[b]...[/b]` | 粗体 |
| `{0}` | 参数占位符 |

## 技术实现

### 数据持久化

游戏使用 `SavedProperties` 系统来序列化遗物数据。该系统需要预先注册属性名才能正确进行网络序列化。

本 Mod 通过以下方式实现持久化：

1. **属性注册**：创建 `RelicStatsProperties` 类，定义带有 `[SavedProperty]` 特性的属性
2. **运行时注入**：在 Mod 初始化时调用 `SavedPropertiesTypeCache.InjectTypeIntoCache(typeof(RelicStatsProperties))`
3. **序列化 Patch**：Patch `RelicModel.ToSerializable()` 方法，在序列化时保存统计数据
4. **反序列化 Patch**：Patch `RelicModel.FromSerializable()` 方法，在加载时恢复统计数据

```csharp
// 属性注册类
public class RelicStatsProperties
{
    [SavedProperty]
    public int RelicStatsTriggerCount { get; set; }

    [SavedProperty]
    public int RelicStatsTotalHeal { get; set; }
    // ...
}

// Mod 初始化时注册
SavedPropertiesTypeCache.InjectTypeIntoCache(typeof(RelicStatsProperties));
```

### 悬浮提示增强

通过 Harmony Patch 修改 `RelicModel.HoverTip` 属性，在描述后追加统计信息：

```csharp
[HarmonyPatch(typeof(RelicModel), nameof(RelicModel.HoverTip), MethodType.Getter)]
public static class RelicModelHoverTipPatch
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
public static class BurningBloodAfterCombatVictoryPatch
{
    public static void Postfix(BurningBlood __instance, CombatRoom _, Task __result)
    {
        __result.ContinueWith(_ =>
        {
            int actualHeal = __instance.Owner.Creature.CurrentHp - _hpBeforeHeal;
            if (actualHeal > 0)
            {
                RelicStatsManager.RecordTrigger(__instance, RelicStatType.Heal, actualHeal);
            }
        });
    }
}
```

## 扩展指南

### 添加新遗物追踪

1. 在 `Patches.cs` 中添加新的 Patch 类（使用驼峰命名法）：

```csharp
[HarmonyPatch(typeof(YourRelic), nameof(YourRelic.TriggerMethod))]
public static class YourRelicTriggerMethodPatch
{
    public static void Postfix(YourRelic __instance, ...)
    {
        RelicStatsManager.RecordTrigger(__instance, RelicStatType.YourStatType, amount);
    }
}
```

2. 如果需要新的统计类型，在 `RelicStatsData.cs` 中添加字段，在 `RelicStatType` 枚举中添加类型。

3. 如果需要新的持久化属性，在 `RelicStatsProperties.cs` 中添加属性。

4. 在本地化文件中添加对应的文本。

### 添加新的本地化键

1. 在 `Localization.cs` 的 `Keys` 类中添加新键：

```csharp
public static class Keys
{
    // ...
    public const string NewStat = "stats.new_stat";
}
```

2. 在 `BuildStatsText` 方法中使用新键：

```csharp
if (stats.NewStat > 0)
{
    lines.Add(GetText(Keys.NewStat, stats.NewStat));
}
```

3. 在所有本地化文件中添加翻译：

```json
{
  "stats.new_stat": "[color=purple]New Stat: {0}[/color]"
}
```

## 已知限制

- 统计数据仅在当前局有效，新局开始后会重置
- 历史记录中的统计数据在游戏版本更新后可能不兼容

## 许可证

MIT License

## 致谢

- [MegaCrit](https://www.megacrit.com/) - 杀戮尖塔2 开发商
- [Harmony](https://github.com/pardeike/Harmony) - .NET 运行时补丁库