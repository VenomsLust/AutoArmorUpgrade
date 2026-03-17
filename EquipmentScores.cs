using System;
using System.Runtime.CompilerServices;
using TaleWorlds.Core;

namespace AutoArmorUpgrade {
    internal static class EquipmentScores {

        public static float WGT_HEAD_ARMOR = 1.0f;
        public static float WGT_CAPE_ARMOR = 1.0f;
        public static float WGT_BODY_ARMOR = 1.0f;
        public static float WGT_HAND_ARMOR = 1.0f;
        public static float WGT_ARM_ARMOR = 1.0f;
        public static float WGT_LEG_ARMOR = 1.0f;
        public static float WGT_ARMOR_WGT = 0.5f;
        public static float WGT_MOUNT_ARMOR_WGT = 0.01f;
        public static float WGT_MOUNT_SPD = 10.0f;
        public static float WGT_MOUNT_MNV = 0.5f;
        public static float WGT_MOUNT_CHG = 0.05f;
        public static float WGT_MOUNT_HP = 0.1f;
        public static float WGT_MOUNT_ARM = 1.0f;
        public static float WGT_SHIELD_HP = 2.0f;
        public static float WGT_SHIELD_SZ = 1.0f;
        public static float WGT_SHIELD_SPD = 0.75f;
        public static float WGT_1H_SLASH_DMG = 2.25f;
        public static float WGT_1H_THRUST_DMG = 0.75f;
        public static float WGT_1H_LENGTH = 1.50f;
        public static float WGT_1H_HANDLE = 1.00f;
        public static float WGT_1H_WGT = 2.00f;
        public static float WGT_2H_SLASH_DMG = 2.25f;
        public static float WGT_2H_THRUST_DMG = 0.75f;
        public static float WGT_2H_LENGTH = 1.50f;
        public static float WGT_2H_HANDLE = 1.00f;
        public static float WGT_2H_WGT = 2.00f;
        public static float WGT_POLE_SWING_DMG = 2.00f;
        public static float WGT_POLE_THRUST_DMG = 2.00f;
        public static float WGT_POLE_LENGTH = 0.50f;
        public static float WGT_POLE_HANDLE = 1.00f;
        public static float WGT_POLE_WGT = 1.00f;
        public static float WGT_BOW_DMG = 1.0f;
        public static float WGT_XBOW_DMG = 1.0f;
        public static float WGT_ARROW_DMG = 1.0f;
        public static float WGT_BOLT_DMG = 1.0f;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CalcHeadArmorScore(EquipmentElement element) {
            if (element.Item?.ItemType == ItemObject.ItemTypeEnum.HeadArmor) {
                float head = element.GetModifiedHeadArmor() * WGT_HEAD_ARMOR;
                float wgt = element.Weight * WGT_ARMOR_WGT;
                return head - wgt;
            }
            return -1f;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CalcCapeArmorScore(EquipmentElement element) {
            if (element.Item?.ItemType == ItemObject.ItemTypeEnum.Cape) {
                float body = element.GetModifiedBodyArmor() * WGT_BODY_ARMOR;
                float arm = element.GetModifiedArmArmor() * WGT_ARM_ARMOR;
                float wgt = element.Weight * WGT_ARMOR_WGT;
                return body + arm - wgt;
            }
            return -1f;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CalcBodyArmorScore(EquipmentElement element) {
            if (element.Item?.ItemType == ItemObject.ItemTypeEnum.BodyArmor) {
                float body = element.GetModifiedBodyArmor() * WGT_BODY_ARMOR;
                float arm = element.GetModifiedArmArmor() * WGT_ARM_ARMOR;
                float leg = element.GetModifiedLegArmor() * WGT_LEG_ARMOR;
                float wgt = element.Weight * WGT_ARMOR_WGT;
                return body + arm + leg - wgt;
            }
            return -1f;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CalcHandArmorScore(EquipmentElement element) {
            if (element.Item?.ItemType == ItemObject.ItemTypeEnum.HandArmor) {
                float arm = element.GetModifiedArmArmor() * WGT_ARM_ARMOR;
                float wgt = element.Weight * WGT_ARMOR_WGT;
                return arm - wgt;
            }
            return -1f;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CalcLegArmorScore(EquipmentElement element) {
            if (element.Item?.ItemType == ItemObject.ItemTypeEnum.LegArmor) {
                float leg = element.GetModifiedLegArmor() * WGT_LEG_ARMOR;
                float wgt = element.Weight * WGT_ARMOR_WGT;
                return leg - wgt;
            }
            return -1f;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CalcMountScore(EquipmentElement element) {
            if (element.Item?.HorseComponent is HorseComponent horse) {
                float spd = horse.Speed * WGT_MOUNT_SPD;
                float mnv = horse.Maneuver * WGT_MOUNT_MNV;
                float chg = horse.ChargeDamage * WGT_MOUNT_CHG;
                float hp = horse.HitPoints * WGT_MOUNT_HP;
                return spd + mnv + chg + hp;
            }
            return -1f;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CalcMountArmorScore(EquipmentElement element) {
            if (element.Item?.ItemType == ItemObject.ItemTypeEnum.HorseHarness) {
                float body = element.GetModifiedMountBodyArmor() * WGT_MOUNT_ARM;
                float wgt = element.Weight * WGT_MOUNT_ARMOR_WGT;
                return body - wgt;
            }
            return -1f;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CalcShieldScore(EquipmentElement element) {
            if (element.Item?.WeaponComponent is WeaponComponent shield) {
                if (shield.PrimaryWeapon is WeaponComponentData shieldData && shieldData.IsShield) {
                    float hp = shieldData.MaxDataValue * WGT_SHIELD_HP;
                    float speed = shieldData.Handling * WGT_SHIELD_SPD;
                    float size = shieldData.BodyArmor * WGT_SHIELD_SZ;
                    float wgt = element.Weight * WGT_ARMOR_WGT;
                    return hp + speed + size - wgt;
                }
            }
            return -1f;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CalcBowScore(EquipmentElement element) {
            if (element.Item?.WeaponComponent is WeaponComponent bow) {
                if (bow.PrimaryWeapon is WeaponComponentData bowData) {
                    float dmg = element.GetModifiedMissileDamageForUsage(0) * WGT_BOW_DMG;
                    float spd = element.GetModifiedMissileSpeedForUsage(0) * 0.01f;
                    float acc = bowData.Accuracy * 0.02f; // double percentage
                    float rof = bowData.Handling * 0.01f;
                    return dmg * (spd + acc + rof);
                }
            }
            return -1f;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CalcCrossbowScore(EquipmentElement element) {
            if (element.Item?.WeaponComponent is WeaponComponent bow) {
                if (bow.PrimaryWeapon is WeaponComponentData bowData) {
                    float dmg = element.GetModifiedMissileDamageForUsage(0) * WGT_XBOW_DMG;
                    float spd = element.GetModifiedMissileSpeedForUsage(0) * 0.01f;
                    float acc = bowData.Accuracy * 0.02f; // double percentage
                    float rof = bowData.Handling * 0.01f;
                    return dmg * (spd + acc + rof);
                }
            }
            return -1f;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CalcArrowScore(EquipmentElement element) {
            if (element.Item?.ItemType == ItemObject.ItemTypeEnum.Arrows && element.Item.WeaponComponent is WeaponComponent arrow) {
                if (arrow.PrimaryWeapon is WeaponComponentData arrowData) {
                    float dmg = Math.Max(0.1f, element.GetModifiedMissileDamageForUsage(0)) * WGT_ARROW_DMG;
                    return dmg * arrowData.MaxDataValue;
                }
            }
            return -1f;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CalcBoltScore(EquipmentElement element) {
            if (element.Item?.ItemType == ItemObject.ItemTypeEnum.Bolts && element.Item.WeaponComponent is WeaponComponent bolts) {
                if (bolts.PrimaryWeapon is WeaponComponentData arrowData) {
                    float dmg = Math.Max(0.1f, element.GetModifiedMissileDamageForUsage(0)) * WGT_BOLT_DMG;
                    return dmg * arrowData.MaxDataValue;
                }
            }
            return -1f;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CalcOneHandedScore(EquipmentElement element) {
            if (element.Item?.ItemType == ItemObject.ItemTypeEnum.OneHandedWeapon) {
                if (element.Item.WeaponComponent is WeaponComponent weapon) {
                    float thrustDmg = element.GetModifiedThrustDamageForUsage(0) * WGT_1H_THRUST_DMG;
                    float thrustSpd = element.GetModifiedThrustSpeedForUsage(0) * 0.01f;

                    float swingDmg = element.GetModifiedSwingDamageForUsage(0) * WGT_1H_SLASH_DMG;
                    float swingSpd = element.GetModifiedSwingSpeedForUsage(0) * 0.01f;

                    float reach = weapon.PrimaryWeapon.WeaponLength * WGT_1H_LENGTH;

                    float handling = element.GetModifiedHandlingForUsage(0) * WGT_1H_HANDLE;

                    float weight = element.Weight * WGT_1H_WGT;

                    return (swingDmg * swingSpd) + (thrustDmg * thrustSpd) + reach + handling - weight;
                }
            }
            return -1f;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CalcTwoHandedScore(EquipmentElement element) {
            if (element.Item?.ItemType == ItemObject.ItemTypeEnum.TwoHandedWeapon) {
                if (element.Item.WeaponComponent is WeaponComponent weapon) {
                    float thrustDmg = element.GetModifiedThrustDamageForUsage(0) * WGT_2H_THRUST_DMG;
                    float thrustSpd = element.GetModifiedThrustSpeedForUsage(0) * 0.01f;

                    float swingDmg = element.GetModifiedSwingDamageForUsage(0) * WGT_2H_SLASH_DMG;
                    float swingSpd = element.GetModifiedSwingSpeedForUsage(0) * 0.01f;

                    float reach = weapon.PrimaryWeapon.WeaponLength * WGT_2H_LENGTH;

                    float handling = element.GetModifiedHandlingForUsage(0) * WGT_2H_HANDLE;

                    float weight = element.Weight * WGT_2H_WGT;

                    return (swingDmg * swingSpd) + (thrustDmg * thrustSpd) + reach + handling - weight;
                }
            }
            return -1f;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CalcPolearmScore(EquipmentElement element) {
            if (element.Item?.ItemType == ItemObject.ItemTypeEnum.Polearm) {
                if (element.Item.WeaponComponent is WeaponComponent weapon) {
                    float thrustDmg = element.GetModifiedThrustDamageForUsage(0) * WGT_POLE_THRUST_DMG;
                    float thrustSpd = element.GetModifiedThrustSpeedForUsage(0) * 0.01f;

                    float swingDmg = element.GetModifiedSwingDamageForUsage(0) * WGT_POLE_SWING_DMG;
                    float swingSpd = element.GetModifiedSwingSpeedForUsage(0) * 0.01f;

                    float reach = weapon.PrimaryWeapon.WeaponLength * WGT_POLE_LENGTH;

                    float handling = element.GetModifiedHandlingForUsage(0) * WGT_POLE_HANDLE;

                    float weight = element.Weight * WGT_POLE_WGT;

                    return (swingDmg * swingSpd) + (thrustDmg * thrustSpd) + reach + handling - weight;
                }
            }
            return -1f;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CalcThrownScore(EquipmentElement element) {
            if (element.Item?.ItemType == ItemObject.ItemTypeEnum.Thrown) {
                float dmg = element.GetModifiedMissileDamageForUsage(0);
                float spd = element.GetModifiedMissileSpeedForUsage(0) * 0.01f;
                float cnt = element.GetModifiedStackCountForUsage(0);
                return (dmg * spd) * cnt;
            }
            return -1f;
        }


        /// <summary>
        /// Returns true for item types that should be bucketed by WeaponClass sub-type
        /// (i.e. types where two items can share the same ItemTypeEnum but be fundamentally
        /// different weapon categories — sword vs axe, bow vs crossbow, etc.).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWeaponType(ItemObject.ItemTypeEnum type) {
            switch (type) {
                case ItemObject.ItemTypeEnum.OneHandedWeapon:
                case ItemObject.ItemTypeEnum.TwoHandedWeapon:
                case ItemObject.ItemTypeEnum.Polearm:
                case ItemObject.ItemTypeEnum.Bow:
                case ItemObject.ItemTypeEnum.Crossbow:
                case ItemObject.ItemTypeEnum.Thrown:
                case ItemObject.ItemTypeEnum.Shield:
                case ItemObject.ItemTypeEnum.Arrows:
                case ItemObject.ItemTypeEnum.Bolts:
                    return true;
                default:
                    return false;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private static Func<EquipmentElement, float>[] funcs = {
            null,                // Invalid
            CalcMountScore,      // Horse
            CalcOneHandedScore,  // OneHandedWeapon
            CalcTwoHandedScore,  // TwoHandedWeapon
            CalcPolearmScore,    // Polearm,
            CalcArrowScore,      // Arrows,
            CalcBoltScore,       // Bolts,
            null,                // SlingStones,
            CalcShieldScore,     // Shield,
            CalcBowScore,        // Bow,
            CalcCrossbowScore,   // Crossbow,
            null,                // Sling,
            CalcThrownScore,     // Thrown,
            null,                // Goods,
            CalcHeadArmorScore,  // HeadArmor,
            CalcBodyArmorScore,  // BodyArmor,
            CalcLegArmorScore,   // LegArmor,
            CalcHandArmorScore,  // HandArmor,
            null,                // Pistol,
            null,                // Musket,
            null,                // Bullets,
            null,                // Animal,
            null,                // Book,
            CalcBodyArmorScore,  // ChestArmor,
            CalcCapeArmorScore,  // Cape,
            CalcMountArmorScore, // HorseHarness,
            null                 // Banner
        };


        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<EquipmentElement, float> GetScoreFunc(ItemObject.ItemTypeEnum itemType) {
            return funcs[(int)itemType];
        }
    }
}
