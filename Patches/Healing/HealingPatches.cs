using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rooms;

namespace RelicStatsTracker.Patches.Healing;

/// <summary>
/// 治疗类遗物统计 Patches
/// 包含：BurningBlood, BlackBlood, BloodVial, Pantograph, EternalFeather, DarkstonePeriapt
/// </summary>
public static class HealingPatches
{
    #region BurningBlood - 燃烧之血

    /// <summary>
    /// 燃烧之血 - 战斗胜利后治疗
    /// 使用前后生命值差值计算实际治疗量
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

    #region BlackBlood - 黑血

    /// <summary>
    /// 黑血 - 战斗胜利后治疗
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

    #endregion

    #region BloodVial - 小血瓶

    /// <summary>
    /// 小血瓶 - 第一回合开始时治疗
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

    #region Pantograph - 放大镜

    /// <summary>
    /// 放大镜 - 进入 Boss 房间时治疗
    /// </summary>
    [HarmonyPatch(typeof(Pantograph), nameof(Pantograph.AfterRoomEntered))]
    public static class PantographAfterRoomEnteredPatch
    {
        private static int _hpBeforeHeal;

        public static void Prefix(Pantograph __instance, AbstractRoom room)
        {
            if (room.RoomType == RoomType.Boss && !__instance.Owner.Creature.IsDead)
            {
                _hpBeforeHeal = __instance.Owner.Creature.CurrentHp;
            }
        }

        public static void Postfix(Pantograph __instance, AbstractRoom room, Task __result)
        {
            if (room.RoomType != RoomType.Boss) return;

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

    #region EternalFeather - 永恒羽毛

    /// <summary>
    /// 永恒羽毛 - 进入休息处时按卡组数量治疗
    /// </summary>
    [HarmonyPatch(typeof(EternalFeather), nameof(EternalFeather.AfterRoomEntered))]
    public static class EternalFeatherAfterRoomEnteredPatch
    {
        private static int _hpBeforeHeal;

        public static void Prefix(EternalFeather __instance, AbstractRoom room)
        {
            if (room is MegaCrit.Sts2.Core.Rooms.RestSiteRoom)
            {
                _hpBeforeHeal = __instance.Owner.Creature.CurrentHp;
            }
        }

        public static void Postfix(EternalFeather __instance, AbstractRoom room, Task __result)
        {
            if (room is not MegaCrit.Sts2.Core.Rooms.RestSiteRoom) return;

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

    #region DarkstonePeriapt - 黑石护身符

    /// <summary>
    /// 黑石护身符 - 获得诅咒牌时增加最大生命值
    /// </summary>
    [HarmonyPatch(typeof(DarkstonePeriapt), nameof(DarkstonePeriapt.AfterCardChangedPiles))]
    public static class DarkstonePeriaptAfterCardChangedPilesPatch
    {
        public static void Postfix(DarkstonePeriapt __instance, CardModel card, PileType oldPileType,
            AbstractModel? source, Task __result)
        {
            // 检查是否是诅咒牌加入牌组
            if (card.Pile?.Type != PileType.Deck) return;
            if (card.Owner != __instance.Owner) return;
            if (card.Type != CardType.Curse) return;

            __result.ContinueWith(_ =>
            {
                int maxHpGained = __instance.DynamicVars.MaxHp.IntValue;
                RelicStatsManager.RecordTrigger(__instance, RelicStatType.MaxHp, maxHpGained);
            });
        }
    }

    #endregion
}