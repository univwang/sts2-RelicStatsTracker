using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace RelicStatsTracker;

/// <summary>
/// 遗物统计追踪的 Harmony Patches
/// </summary>
public static class Patches
{
    #region 悬浮提示增强

    /// <summary>
    /// 在遗物悬浮提示中添加统计数据
    /// 通过修改 HoverTip 属性，在描述后追加统计信息
    /// </summary>
    [HarmonyPatch(typeof(RelicModel), nameof(RelicModel.HoverTip), MethodType.Getter)]
    public static class RelicModel_HoverTip_Patch
    {
        public static void Postfix(RelicModel __instance, ref HoverTip __result)
        {
            // 获取统计数据
            var stats = RelicStatsManager.GetStats(__instance);

            // 如果有统计数据，修改描述文本
            if (stats.HasAnyStats())
            {
                // 构建统计信息文本
                var statsText = Localization.BuildStatsText(stats);

                // 创建新的 HoverTip，在原描述后追加统计信息
                // 注意：HoverTip 是 record struct，需要创建新实例
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
    [HarmonyPatch(typeof(RunManager), nameof(RunManager.Launch))]
    public static class RunManager_Launch_Patch
    {
        public static void Prefix()
        {
            // 清除上一局的统计数据
            RelicStatsManager.ClearCurrentRun();
        }
    }

    #endregion

    #region 燃烧之血 (BurningBlood) - 治疗类遗物

    /// <summary>
    /// 追踪燃烧之血（BurningBlood）的触发
    /// 战斗胜利后治疗6点生命值
    /// </summary>
    [HarmonyPatch(typeof(BurningBlood), nameof(BurningBlood.AfterCombatVictory))]
    public static class BurningBlood_AfterCombatVictory_Patch
    {
        public static void Postfix(BurningBlood __instance, CombatRoom _)
        {
            // 获取治疗量（从动态变量中获取）
            int healAmount = __instance.DynamicVars.Heal.IntValue;
            RelicStatsManager.RecordTrigger(__instance, RelicStatType.Heal, healAmount);
        }
    }

    #endregion

    #region 黑血 (BlackBlood) - 治疗类遗物

    /// <summary>
    /// 追踪黑血（BlackBlood）的触发
    /// 战斗胜利后治疗12点生命值
    /// </summary>
    [HarmonyPatch(typeof(BlackBlood), nameof(BlackBlood.AfterCombatVictory))]
    public static class BlackBlood_AfterCombatVictory_Patch
    {
        public static void Postfix(BlackBlood __instance, CombatRoom _)
        {
            int healAmount = __instance.DynamicVars.Heal.IntValue;
            RelicStatsManager.RecordTrigger(__instance, RelicStatType.Heal, healAmount);
        }
    }

    #endregion

    #region 小血瓶 (BloodVial) - 治疗类遗物

    /// <summary>
    /// 追踪小血瓶（BloodVial）的触发
    /// 第一回合开始时治疗2点生命值
    /// </summary>
    [HarmonyPatch(typeof(BloodVial), nameof(BloodVial.AfterPlayerTurnStartLate))]
    public static class BloodVial_AfterPlayerTurnStartLate_Patch
    {
        public static void Postfix(BloodVial __instance, PlayerChoiceContext choiceContext, Player player)
        {
            // 只在第一回合且是当前玩家时记录
            if (player == __instance.Owner && player.Creature.CombatState.RoundNumber <= 1)
            {
                int healAmount = __instance.DynamicVars.Heal.IntValue;
                RelicStatsManager.RecordTrigger(__instance, RelicStatType.Heal, healAmount);
            }
        }
    }

    #endregion

    #region 锚 (Anchor) - 格挡类遗物

    /// <summary>
    /// 追踪锚（Anchor）的触发
    /// 战斗开始时获得10点格挡
    /// </summary>
    [HarmonyPatch(typeof(Anchor), nameof(Anchor.BeforeCombatStart))]
    public static class Anchor_BeforeCombatStart_Patch
    {
        public static void Postfix(Anchor __instance)
        {
            int blockAmount = __instance.DynamicVars.Block.IntValue;
            RelicStatsManager.RecordTrigger(__instance, RelicStatType.Block, blockAmount);
        }
    }

    #endregion

    #region 提灯 (Lantern) - 能量类遗物

    /// <summary>
    /// 追踪提灯（Lantern）的触发
    /// 第一回合开始时获得1点能量
    /// </summary>
    [HarmonyPatch(typeof(Lantern), nameof(Lantern.AfterSideTurnStart))]
    public static class Lantern_AfterSideTurnStart_Patch
    {
        public static void Postfix(Lantern __instance, CombatSide side, CombatState combatState)
        {
            // 只在第一回合且是玩家方时记录
            if (side == __instance.Owner.Creature.Side && combatState.RoundNumber <= 1)
            {
                int energyAmount = __instance.DynamicVars.Energy.IntValue;
                RelicStatsManager.RecordTrigger(__instance, RelicStatType.Energy, energyAmount);
            }
        }
    }

    #endregion

    #region 手里剑 (Shuriken) - 力量类遗物

    /// <summary>
    /// 追踪手里剑（Shuriken）的触发
    /// 每打出3张攻击牌获得1点力量
    /// </summary>
    [HarmonyPatch(typeof(Shuriken), nameof(Shuriken.AfterCardPlayed))]
    public static class Shuriken_AfterCardPlayed_Patch
    {
        public static void Postfix(Shuriken __instance, PlayerChoiceContext context, CardPlay cardPlay)
        {
            // 检查是否是攻击牌且是当前玩家打出的
            if (cardPlay.Card.Owner == __instance.Owner && 
                CombatManager.Instance.IsInProgress && 
                cardPlay.Card.Type == CardType.Attack)
            {
                // 检查是否触发了力量获取
                int attacksPlayed = __instance.DisplayAmount;
                int threshold = __instance.DynamicVars.Cards.IntValue;
                
                // 如果当前计数为0，说明刚刚触发了一次
                if (attacksPlayed == 0)
                {
                    int strengthAmount = __instance.DynamicVars.Strength.IntValue;
                    RelicStatsManager.RecordTrigger(__instance, RelicStatType.Strength, strengthAmount);
                }
            }
        }
    }

    #endregion

    #region 苦无 (Kunai) - 敏捷类遗物

    /// <summary>
    /// 追踪苦无（Kunai）的触发
    /// 每打出3张攻击牌获得1点敏捷
    /// </summary>
    [HarmonyPatch(typeof(Kunai), nameof(Kunai.AfterCardPlayed))]
    public static class Kunai_AfterCardPlayed_Patch
    {
        public static void Postfix(Kunai __instance, PlayerChoiceContext context, CardPlay cardPlay)
        {
            if (cardPlay.Card.Owner == __instance.Owner && 
                CombatManager.Instance.IsInProgress && 
                cardPlay.Card.Type == CardType.Attack)
            {
                int attacksPlayed = __instance.DisplayAmount;
                
                // 如果当前计数为0，说明刚刚触发了一次
                if (attacksPlayed == 0)
                {
                    int dexterityAmount = __instance.DynamicVars.Dexterity.IntValue;
                    RelicStatsManager.RecordTrigger(__instance, RelicStatType.Dexterity, dexterityAmount);
                }
            }
        }
    }

    #endregion

    #region 华丽扇 (OrnamentalFan) - 格挡类遗物

    /// <summary>
    /// 追踪华丽扇（OrnamentalFan）的触发
    /// 每打出3张攻击牌获得4点格挡
    /// </summary>
    [HarmonyPatch(typeof(OrnamentalFan), nameof(OrnamentalFan.AfterCardPlayed))]
    public static class OrnamentalFan_AfterCardPlayed_Patch
    {
        public static void Postfix(OrnamentalFan __instance, PlayerChoiceContext context, CardPlay cardPlay)
        {
            if (cardPlay.Card.Owner == __instance.Owner && 
                CombatManager.Instance.IsInProgress && 
                cardPlay.Card.Type == CardType.Attack)
            {
                int attacksPlayed = __instance.DisplayAmount;
                
                // 如果当前计数为0，说明刚刚触发了一次
                if (attacksPlayed == 0)
                {
                    int blockAmount = __instance.DynamicVars.Block.IntValue;
                    RelicStatsManager.RecordTrigger(__instance, RelicStatType.Block, blockAmount);
                }
            }
        }
    }

    #endregion
    

    #region 孙子兵法 (ArtOfWar) - 能量类遗物

    /// <summary>
    /// 追踪孙子兵法（ArtOfWar）的触发
    /// 如果本回合没有打出攻击牌，下回合获得1点能量
    /// </summary>
    [HarmonyPatch(typeof(ArtOfWar), nameof(ArtOfWar.AfterEnergyReset))]
    public static class ArtOfWar_AfterEnergyReset_Patch
    {
        public static void Postfix(ArtOfWar __instance, Player player)
        {
            if (player != __instance.Owner) return;
            
            // 检查是否触发了能量获取
            // 如果状态变为Active且回合数大于1，说明触发了
            if (__instance.Owner.Creature.CombatState?.RoundNumber > 1)
            {
                // 通过检查私有字段判断是否触发
                var anyAttacksPlayedLastTurn = GetPrivateField<bool>(__instance, "_anyAttacksPlayedLastTurn");
                if (!anyAttacksPlayedLastTurn)
                {
                    int energyAmount = __instance.DynamicVars.Energy.IntValue;
                    RelicStatsManager.RecordTrigger(__instance, RelicStatType.Energy, energyAmount);
                }
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

    #region 百年谜题 (CentennialPuzzle) - 抽牌类遗物

    /// <summary>
    /// 追踪百年谜题（CentennialPuzzle）的触发
    /// 第一次受到伤害时抽3张牌
    /// </summary>
    [HarmonyPatch(typeof(CentennialPuzzle), nameof(CentennialPuzzle.AfterDamageReceived))]
    public static class CentennialPuzzle_AfterDamageReceived_Patch
    {
        public static void Postfix(CentennialPuzzle __instance, PlayerChoiceContext choiceContext, 
            Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
        {
            // 检查是否触发了抽牌
            if (CombatManager.Instance.IsInProgress && 
                target == __instance.Owner.Creature && 
                result.UnblockedDamage > 0 && 
                !__instance.UsedThisCombat)
            {
                int cardsDrawn = __instance.DynamicVars.Cards.IntValue;
                RelicStatsManager.RecordTrigger(__instance, RelicStatType.CardsDrawn, cardsDrawn);
            }
        }
    }

    #endregion

    #region 双节棍 (Nunchaku) - 能量类遗物

    /// <summary>
    /// 追踪双节棍（Nunchaku）的触发
    /// 每打出10张攻击牌获得1点能量
    /// </summary>
    [HarmonyPatch(typeof(Nunchaku), nameof(Nunchaku.AfterCardPlayed))]
    public static class Nunchaku_AfterCardPlayed_Patch
    {
        public static void Postfix(Nunchaku __instance, PlayerChoiceContext context, CardPlay cardPlay)
        {
            if (cardPlay.Card.Owner == __instance.Owner && cardPlay.Card.Type == CardType.Attack)
            {
                int attacksPlayed = __instance.DisplayAmount;
                
                // 如果当前计数为0，说明刚刚触发了一次
                if (attacksPlayed == 0 && CombatManager.Instance.IsInProgress)
                {
                    int energyAmount = __instance.DynamicVars.Energy.IntValue;
                    RelicStatsManager.RecordTrigger(__instance, RelicStatType.Energy, energyAmount);
                }
            }
        }
    }

    #endregion

    #region 弹珠袋 (BagOfMarbles) - 易伤类遗物

    /// <summary>
    /// 追踪弹珠袋（BagOfMarbles）的触发
    /// 战斗开始时给予所有敌人1层易伤
    /// </summary>
    [HarmonyPatch(typeof(BagOfMarbles), nameof(BagOfMarbles.BeforeSideTurnStart))]
    public static class BagOfMarbles_BeforeSideTurnStart_Patch
    {
        public static void Postfix(BagOfMarbles __instance, PlayerChoiceContext choiceContext, 
            CombatSide side, CombatState combatState)
        {
            // 只在第一回合且是玩家方时记录
            if (side == __instance.Owner.Creature.Side && combatState.RoundNumber <= 1)
            {
                int vulnerableAmount = __instance.DynamicVars.Vulnerable.IntValue;
                RelicStatsManager.RecordCustomStat(__instance, "vulnerable_applied", vulnerableAmount);
            }
        }
    }

    #endregion
}