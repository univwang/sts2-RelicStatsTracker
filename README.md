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

### 召唤类（骷髅人相关）
| 遗物 | 追踪数据 |
|------|----------|
| 缚魂命匣 (BoundPhylactery) | 召唤次数 |
| 无界命匣 (PhylacteryUnbound) | 召唤次数 |
| 骨笛 (BoneFlute) | 格挡量 |
| 异鸟宝宝 (Byrdpip) | 召唤次数 |
| Paels Legion | 格挡量 |

### 治疗类
| 遗物 | 追踪数据 |
|------|----------|
| 燃烧之血 (BurningBlood) | 触发次数、总治疗量 |
| 黑血 (BlackBlood) | 触发次数、总治疗量 |
| 小血瓶 (BloodVial) | 触发次数、总治疗量 |
| 放大镜 (Pantograph) | 触发次数、总治疗量 |
| 永恒羽毛 (EternalFeather) | 触发次数、总治疗量 |
| 黑石护符 (DarkstonePeriapt) | 触发次数、最大生命值 |

### 格挡类
| 遗物 | 追踪数据 |
|------|----------|
| 锚 (Anchor) | 触发次数、总格挡量 |
| 华丽扇 (OrnamentalFan) | 触发次数、总格挡量 |
| 奥利哈钢 (Orichalcum) | 触发次数、总格挡量 |
| 船夹板 (HornCleat) | 触发次数、总格挡量 |
| 斗篷扣 (CloakClasp) | 触发次数、总格挡量 |

### 能量类
| 遗物 | 追踪数据 |
|------|----------|
| 提灯 (Lantern) | 触发次数、总能量 |
| 孙子兵法 (ArtOfWar) | 触发次数、总能量 |
| 双节棍 (Nunchaku) | 触发次数、总能量 |
| 开心小花 (HappyFlower) | 触发次数、总能量 |
| 地精之角 (GremlinHorn) | 触发次数、总能量、抽牌数 |

### 属性类
| 遗物 | 追踪数据 |
|------|----------|
| 手里剑 (Shuriken) | 触发次数、获得力量 |
| 苦无 (Kunai) | 触发次数、获得敏捷 |

### 伤害类
| 遗物 | 追踪数据 |
|------|----------|
| 水银沙漏 (MercuryHourglass) | 触发次数、总伤害量 |
| 开信刀 (LetterOpener) | 触发次数、总伤害量 |
| 笔尖 (PenNib) | 触发次数、总伤害量 |
| 弹珠袋 (BagOfMarbles) | 触发次数、施加易伤 |

### 抽牌类
| 遗物 | 追踪数据 |
|------|----------|
| 百年谜题 (CentennialPuzzle) | 触发次数、抽牌数 |
| 准备背包 (BagOfPreparation) | 触发次数、抽牌数 |

### 球类
| 遗物 | 追踪数据 |
|------|----------|
| 破损核心 (CrackedCore) | 触发次数、引导球数 |
| 注能核心 (InfusedCore) | 触发次数、引导球数 |
| 数据磁盘 (DataDisk) | 触发次数、集中值 |

### 其他
| 遗物 | 追踪数据 |
|------|----------|
| 赤牛 (Akabeko) | 触发次数、活力值 |
| 护喉甲 (Gorget) | 触发次数、覆甲值 |
| 击剑指南 (FencingManual) | 触发次数、铸造值 |

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
├── Patches/                 # Harmony Patches（按类别分文件夹）
│   ├── PatchHelper.cs       # 辅助方法
│   ├── Core/                # 核心功能
│   │   ├── HoverTipPatch.cs
│   │   └── RunResetPatch.cs
│   ├── Summon/              # 召唤类遗物
│   ├── Healing/             # 治疗类遗物
│   ├── Block/               # 格挡类遗物
│   ├── Energy/              # 能量类遗物
│   ├── Damage/              # 伤害类遗物
│   ├── Card/                # 抽牌类遗物
│   ├── Attribute/           # 属性类遗物
│   ├── Orb/                 # 球类遗物
│   ├── Misc/                # 其他遗物
│   └── Persistence/         # 数据持久化
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
  "stats.total_vigor": "[color=orange]Total Vigor: {0}[/color]",
  "stats.total_focus": "[color=purple]Total Focus: {0}[/color]",
  "stats.total_plating": "[color=gray]Total Plating: {0}[/color]",
  "stats.total_forge": "[color=orange]Total Forge: {0}[/color]",
  "stats.total_stars": "[color=yellow]Total Stars: {0}[/color]",
  "stats.total_max_hp": "[color=green]Max HP Gained: {0}[/color]",
  "stats.orbs_channeled": "[color=cyan]Orbs Channeled: {0}[/color]",
  "stats.summons_count": "[color=purple]Summons: {0}[/color]",
  "stats.block_doubled": "[color=blue]Block Doubled: {0}[/color]",
  "stats.cards_upgraded": "[color=yellow]Cards Upgraded: {0}[/color]",
  "stats.cards_obtained": "[color=white]Cards Obtained: {0}[/color]",
  "stats.gold_gained": "[color=gold]Gold Gained: {0}[/color]",
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
| `[color=purple]...[/color]` | 紫色文本 |
| `[color=gray]...[/color]` | 灰色文本 |
| `[color=gold]...[/color]` | 金色文本 |
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

通过 Patch 各遗物的触发方法来记录统计数据。Patch 文件按遗物类别分文件夹组织：

- `Summon/` - 召唤类遗物（骷髅人相关）
- `Healing/` - 治疗类遗物
- `Block/` - 格挡类遗物
- `Energy/` - 能量类遗物
- `Damage/` - 伤害类遗物
- `Card/` - 抽牌类遗物
- `Attribute/` - 属性类遗物（力量、敏捷等）
- `Orb/` - 球类遗物
- `Misc/` - 其他遗物

## 扩展指南

### 添加新遗物追踪

1. 确定遗物类别，在对应的 `Patches/` 子目录下添加或修改 Patch 文件

2. 使用驼峰命名法创建 Patch 类：

```csharp
[HarmonyPatch(typeof(YourRelic), nameof(YourRelic.TriggerMethod))]
public static class YourRelicTriggerMethodPatch
{
    public static void Postfix(YourRelic __instance, ..., Task __result)
    {
        __result.ContinueWith(_ =>
        {
            RelicStatsManager.RecordTrigger(__instance, RelicStatType.YourStatType, amount);
        });
    }
}
```

3. 如果需要新的统计类型：
   - 在 `RelicStatsData.cs` 中添加字段
   - 在 `RelicStatType` 枚举中添加类型
   - 在 `RelicStatsManager.cs` 的 `ApplyStatType` 方法中添加处理
   - 在 `Localization.cs` 的 `Keys` 类中添加键名
   - 在 `BuildStatsText` 方法中添加显示逻辑
   - 在本地化文件中添加翻译

### 治疗类遗物注意事项

治疗类遗物需要使用前后生命值差值计算实际治疗量，以排除溢出：

```csharp
private static int _hpBeforeHeal;

public static void Prefix(RelicModel __instance)
{
    _hpBeforeHeal = __instance.Owner.Creature.CurrentHp;
}

public static void Postfix(RelicModel __instance, Task __result)
{
    __result.ContinueWith(_ =>
    {
        int actualHeal = __instance.Owner.Creature.CurrentHp - _hpBeforeHeal;
        if (actualHeal > 0)
            RelicStatsManager.RecordTrigger(__instance, RelicStatType.Heal, actualHeal);
    });
}
```

### 计数触发类遗物

对于每 N 次触发一次效果的遗物（如 Shuriken、Nunchaku），检查计数器是否归零：

```csharp
int counter = GetPrivateField<int>(__instance, "_attacksPlayedThisTurn");
int threshold = __instance.DynamicVars.Cards.IntValue;

if (counter % threshold == 0) // 刚刚触发
{
    RelicStatsManager.RecordTrigger(__instance, statType, amount);
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