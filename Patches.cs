using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace RelicStatsTracker;

/// <summary>
/// 遗物统计追踪的 Harmony Patches
/// 参考 STS1 RelicStats mod 的实现方式
/// </summary>
public static class Patches
{
    #region 悬浮提示增强

    /// <summary>
    /// 在遗物悬浮提示中添加统计数据
    /// </summary>
    [HarmonyPatch(typeof(RelicModel), nameof(RelicModel.HoverTip), MethodType.Getter)]
    public static class RelicModelHoverTipPatch
    {
        public static void Postfix(RelicModel __instance, ref HoverTip __result)
        {
            var stats = RelicStatsManager.GetStats(__instance);

            if (stats.HasAnyStats())
            {
                var statsText = Localization.BuildStatsText(stats);
                __result = new HoverTip(
                    __instance.Title,
                    __result.Description + statsText,
                    __result.Icon
                );
            }
        }
    }

    #endregion

    #region 新局重置

    /// <summary>
    /// 在新局开始时清除统计数据
    /// </summary>
    [HarmonyPatch(typeof(RunManager), "InitializeNewRun")]
    public static class RunManagerInitializeNewRunPatch
    {
        public static void Prefix()
        {
            RelicStatsManager.ClearCurrentRun();
        }
    }

    #endregion

    #region 治疗类遗物 - 使用前后生命值差值计算实际治疗量

    /// <summary>
    /// 燃烧之血 (BurningBlood) - 战斗胜利后治疗
    /// 使用 Prefix 记录治疗前生命值，Postfix 计算实际治疗量
    /// </summary>
    [HarmonyPatch(typeof(BurningBlood), nameof(BurningBlood.AfterCombatVictory))]
    public static class BurningBloodAfterCombatVictoryPatch
    {
        private static int _hpBeforeHeal;

        public static void Prefix(BurningBlood __instance, CombatRoom _)
        {
            _hpBeforeHeal = __instance.Owner.Creature.CurrentHp;
        }

        public static void Postfix(BurningBlood __instance, CombatRoom _, Task __result)
        {
            // 在原任务完成后执行统计
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

    /// <summary>
    /// 黑血 (BlackBlood) - 战斗胜利后治疗
    /// </summary>
    [HarmonyPatch(typeof(BlackBlood), nameof(BlackBlood.AfterCombatVictory))]
    public static class BlackBloodAfterCombatVictoryPatch
    {
        private static int _hpBeforeHeal;

        public static void Prefix(BlackBlood __instance, CombatRoom _)
        {
            _hpBeforeHeal = __instance.Owner.Creature.CurrentHp;
        }

        public static void Postfix(BlackBlood __instance, CombatRoom _, Task __result)
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

    /// <summary>
    /// 小血瓶 (BloodVial) - 第一回合开始时治疗
    /// </summary>
    [HarmonyPatch(typeof(BloodVial), nameof(BloodVial.AfterPlayerTurnStartLate))]
    public static class BloodVialAfterPlayerTurnStartLatePatch
    {
        private static int _hpBeforeHeal;
        private static bool _shouldTrack;

        public static void Prefix(BloodVial __instance, PlayerChoiceContext choiceContext, Player player)
        {
            _shouldTrack = player == __instance.Owner && player.Creature.CombatState.RoundNumber <= 1;
            if (_shouldTrack)
            {
                _hpBeforeHeal = __instance.Owner.Creature.CurrentHp;
            }
        }

        public static void Postfix(BloodVial __instance, PlayerChoiceContext choiceContext, Player player, Task __result)
        {
            if (!_shouldTrack) return;

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

    #endregion

    #region 格挡类遗物

    /// <summary>
    /// 锚 (Anchor) - 战斗开始时获得格挡
    /// </summary>
    [HarmonyPatch(typeof(Anchor), nameof(Anchor.BeforeCombatStart))]
    public static class AnchorBeforeCombatStartPatch
    {
        public static void Postfix(Anchor __instance, Task __result)
        {
            __result.ContinueWith(_ =>
            {
                int blockAmount = __instance.DynamicVars.Block.IntValue;
                RelicStatsManager.RecordTrigger(__instance, RelicStatType.Block, blockAmount);
            });
        }
    }

    /// <summary>
    /// 华丽扇 (OrnamentalFan) - 每打出3张攻击牌获得格挡
    /// </summary>
    [HarmonyPatch(typeof(OrnamentalFan), nameof(OrnamentalFan.AfterCardPlayed))]
    public static class OrnamentalFanAfterCardPlayedPatch
    {
        public static void Postfix(OrnamentalFan __instance, PlayerChoiceContext context, CardPlay cardPlay)
        {
            if (cardPlay.Card.Owner != __instance.Owner ||
                !CombatManager.Instance.IsInProgress ||
                cardPlay.Card.Type != CardType.Attack)
            {
                return;
            }

            int attacksPlayedThisTurn = GetPrivateField<int>(__instance, "_attacksPlayedThisTurn");
            int cardsNeeded = __instance.DynamicVars.Cards.IntValue;

            // 当计数归零时，说明刚刚触发了
            if (attacksPlayedThisTurn % cardsNeeded == 0)
            {
                int blockAmount = __instance.DynamicVars.Block.IntValue;
                RelicStatsManager.RecordTrigger(__instance, RelicStatType.Block, blockAmount);
            }
        }
    }

    #endregion

    #region 能量类遗物

    /// <summary>
    /// 提灯 (Lantern) - 第一回合开始时获得能量
    /// </summary>
    [HarmonyPatch(typeof(Lantern), nameof(Lantern.AfterSideTurnStart))]
    public static class LanternAfterSideTurnStartPatch
    {
        public static void Postfix(Lantern __instance, CombatSide side, CombatState combatState, Task __result)
        {
            if (side != __instance.Owner.Creature.Side || combatState.RoundNumber > 1) return;

            __result.ContinueWith(_ =>
            {
                int energyAmount = __instance.DynamicVars.Energy.IntValue;
                RelicStatsManager.RecordTrigger(__instance, RelicStatType.Energy, energyAmount);
            });
        }
    }

    /// <summary>
    /// 孙子兵法 (ArtOfWar) - 未打出攻击牌时获得能量
    /// </summary>
    [HarmonyPatch(typeof(ArtOfWar), nameof(ArtOfWar.AfterEnergyReset))]
    public static class ArtOfWarAfterEnergyResetPatch
    {
        public static void Postfix(ArtOfWar __instance, Player player, Task __result)
        {
            if (player != __instance.Owner) return;
            if (__instance.Owner.Creature.CombatState?.RoundNumber <= 1) return;

            bool anyAttacksPlayedLastTurn = GetPrivateField<bool>(__instance, "_anyAttacksPlayedLastTurn");
            if (anyAttacksPlayedLastTurn) return;

            __result.ContinueWith(_ =>
            {
                int energyAmount = __instance.DynamicVars.Energy.IntValue;
                RelicStatsManager.RecordTrigger(__instance, RelicStatType.Energy, energyAmount);
            });
        }
    }

    /// <summary>
    /// 双节棍 (Nunchaku) - 每打出10张攻击牌获得能量
    /// </summary>
    [HarmonyPatch(typeof(Nunchaku), nameof(Nunchaku.AfterCardPlayed))]
    public static class NunchakuAfterCardPlayedPatch
    {
        public static void Postfix(Nunchaku __instance, PlayerChoiceContext context, CardPlay cardPlay, Task __result)
        {
            if (cardPlay.Card.Owner != __instance.Owner || cardPlay.Card.Type != CardType.Attack) return;

            int attacksPlayed = __instance.AttacksPlayed;
            int cardsNeeded = __instance.DynamicVars.Cards.IntValue;

            // 当计数归零时，说明刚刚触发了
            if (attacksPlayed % cardsNeeded != 0 || !CombatManager.Instance.IsInProgress) return;

            __result.ContinueWith(_ =>
            {
                int energyAmount = __instance.DynamicVars.Energy.IntValue;
                RelicStatsManager.RecordTrigger(__instance, RelicStatType.Energy, energyAmount);
            });
        }
    }

    #endregion

    #region 力量/敏捷类遗物

    /// <summary>
    /// 手里剑 (Shuriken) - 每打出3张攻击牌获得力量
    /// </summary>
    [HarmonyPatch(typeof(Shuriken), nameof(Shuriken.AfterCardPlayed))]
    public static class ShurikenAfterCardPlayedPatch
    {
        public static void Postfix(Shuriken __instance, PlayerChoiceContext context, CardPlay cardPlay, Task __result)
        {
            if (cardPlay.Card.Owner != __instance.Owner ||
                !CombatManager.Instance.IsInProgress ||
                cardPlay.Card.Type != CardType.Attack)
            {
                return;
            }

            int attacksPlayedThisTurn = GetPrivateField<int>(__instance, "_attacksPlayedThisTurn");
            int cardsNeeded = __instance.DynamicVars.Cards.IntValue;

            // 当计数归零时，说明刚刚触发了
            if (attacksPlayedThisTurn % cardsNeeded == 0)
            {
                __result.ContinueWith(_ =>
                {
                    int strengthAmount = __instance.DynamicVars.Strength.IntValue;
                    RelicStatsManager.RecordTrigger(__instance, RelicStatType.Strength, strengthAmount);
                });
            }
        }
    }

    /// <summary>
    /// 苦无 (Kunai) - 每打出3张攻击牌获得敏捷
    /// </summary>
    [HarmonyPatch(typeof(Kunai), nameof(Kunai.AfterCardPlayed))]
    public static class KunaiAfterCardPlayedPatch
    {
        public static void Postfix(Kunai __instance, PlayerChoiceContext context, CardPlay cardPlay, Task __result)
        {
            if (cardPlay.Card.Owner != __instance.Owner ||
                !CombatManager.Instance.IsInProgress ||
                cardPlay.Card.Type != CardType.Attack)
            {
                return;
            }

            int attacksPlayedThisTurn = GetPrivateField<int>(__instance, "_attacksPlayedThisTurn");
            int cardsNeeded = __instance.DynamicVars.Cards.IntValue;

            // 当计数归零时，说明刚刚触发了
            if (attacksPlayedThisTurn % cardsNeeded == 0)
            {
                __result.ContinueWith(_ =>
                {
                    int dexterityAmount = __instance.DynamicVars.Dexterity.IntValue;
                    RelicStatsManager.RecordTrigger(__instance, RelicStatType.Dexterity, dexterityAmount);
                });
            }
        }
    }

    #endregion

    #region 抽牌类遗物

    /// <summary>
    /// 百年积木 (CentennialPuzzle) - 第一次受到伤害时抽牌
    /// </summary>
    [HarmonyPatch(typeof(CentennialPuzzle), nameof(CentennialPuzzle.AfterDamageReceived))]
    public static class CentennialPuzzleAfterDamageReceivedPatch
    {
        public static void Postfix(CentennialPuzzle __instance, PlayerChoiceContext choiceContext,
            Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource, Task __result)
        {
            if (!CombatManager.Instance.IsInProgress ||
                target != __instance.Owner.Creature ||
                result.UnblockedDamage <= 0 ||
                __instance.UsedThisCombat)
            {
                return;
            }

            __result.ContinueWith(_ =>
            {
                int cardsDrawn = __instance.DynamicVars.Cards.IntValue;
                RelicStatsManager.RecordTrigger(__instance, RelicStatType.CardsDrawn, cardsDrawn);
            });
        }
    }

    #endregion

    #region 易伤类遗物

    /// <summary>
    /// 弹珠袋 (BagOfMarbles) - 战斗开始时给予敌人易伤
    /// </summary>
    [HarmonyPatch(typeof(BagOfMarbles), nameof(BagOfMarbles.BeforeSideTurnStart))]
    public static class BagOfMarblesBeforeSideTurnStartPatch
    {
        public static void Postfix(BagOfMarbles __instance, PlayerChoiceContext choiceContext,
            CombatSide side, CombatState combatState, Task __result)
        {
            if (side != __instance.Owner.Creature.Side || combatState.RoundNumber > 1) return;

            __result.ContinueWith(_ =>
            {
                int vulnerableAmount = __instance.DynamicVars.Vulnerable.IntValue;
                int enemyCount = combatState.HittableEnemies.Count;
                RelicStatsManager.RecordCustomStat(__instance, "vulnerable_applied", vulnerableAmount * enemyCount);
            });
        }
    }

    #endregion

    #region 伤害类遗物

    /// <summary>
    /// 笔尖 (PenNib) - 每打出10张攻击牌，下一张攻击牌伤害翻倍
    /// </summary>
    [HarmonyPatch(typeof(PenNib), nameof(PenNib.BeforeCardPlayed))]
    public static class PenNibBeforeCardPlayedPatch
    {
        public static void Postfix(PenNib __instance, CardPlay cardPlay)
        {
            if (cardPlay.Card.Type != CardType.Attack) return;
            if (cardPlay.Card.Owner != __instance.Owner) return;

            // 当 AttacksPlayed 刚刚归零时，说明触发了双倍伤害
            if (__instance.AttacksPlayed == 0)
            {
                RelicStatsManager.RecordCustomStat(__instance, "double_damage_count", 1);
            }
        }
    }

    #endregion

    #region 辅助方法

    /// <summary>
    /// 获取私有字段值
    /// </summary>
    private static T? GetPrivateField<T>(object obj, string fieldName)
    {
        var field = obj.GetType().GetField(fieldName,
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        return field != null ? (T?)field.GetValue(obj) : default;
    }

    #endregion

    #region 数据持久化

    /// <summary>
    /// 在遗物序列化时保存统计数据
    /// </summary>
    [HarmonyPatch(typeof(RelicModel), nameof(RelicModel.ToSerializable))]
    public static class RelicModelToSerializablePatch
    {
        public static void Postfix(RelicModel __instance, SerializableRelic __result)
        {
            __result.Props ??= new SavedProperties();
            RelicStatsManager.SaveStatsToSavedProperties(__instance, __result.Props);
        }
    }

    /// <summary>
    /// 在遗物从序列化数据恢复时加载统计数据
    /// </summary>
    [HarmonyPatch(typeof(RelicModel), nameof(RelicModel.FromSerializable))]
    public static class RelicModelFromSerializablePatch
    {
        public static void Postfix(SerializableRelic save, RelicModel __result)
        {
            if (save.Props != null)
            {
                RelicStatsManager.LoadStatsToRuntime(__result, save.Props);
            }
        }
    }

    #endregion
}