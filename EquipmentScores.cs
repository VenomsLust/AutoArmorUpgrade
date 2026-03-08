using System;
using System.Runtime.CompilerServices;
using TaleWorlds.Core;

namespace AutoArmorUpgrade {
    internal static class EquipmentScores {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CalcHeadArmorScore(EquipmentElement element) {
            if (element.Item?.ItemType == ItemObject.ItemTypeEnum.HeadArmor) {
                return element.GetModifiedHeadArmor() - element.Weight * 0.5f;
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
                return element.GetModifiedBodyArmor() + element.GetModifiedArmArmor() - (element.Weight * 0.5f);
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
                return element.GetModifiedBodyArmor() + element.GetModifiedArmArmor() + element.GetModifiedLegArmor() - (element.Weight * 0.5f);
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
                return element.GetModifiedArmArmor() - (element.Weight * 0.5f);
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
                return element.GetModifiedLegArmor() - (element.Weight * 0.5f);
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
                return (horse.Speed * 10.0f) + horse.Maneuver + (horse.ChargeDamage * 0.05f);
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
                return element.GetModifiedBodyArmor() - (element.Weight * 0.01f);
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
                    float hp = shieldData.MaxDataValue;
                    float speed = shieldData.Handling;
                    float size = shieldData.BodyArmor;
                    return (hp * 0.1f) + speed + (size * 2.0f) - element.Weight;
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
                    float dmg = element.GetModifiedMissileDamageForUsage(0);
                    float spd = element.GetModifiedMissileSpeedForUsage(0);
                    float acc = bowData.Accuracy;
                    float rof = bowData.Handling;
                    return (dmg * 2) * (spd * 0.01f) * (acc * 0.01f) * (rof * 0.01f);
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
                    float dmg = element.GetModifiedMissileDamageForUsage(0);
                    float spd = element.GetModifiedMissileSpeedForUsage(0);
                    float acc = bowData.Accuracy;
                    float rof = bowData.Handling;
                    return (dmg * 2) * (spd * 0.01f) * (acc * 0.01f) * (rof * 0.01f);
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
                    return ((element.GetModifiedMissileDamageForUsage(0) + 1.0f) * 2.0f) * arrowData.MaxDataValue;
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
                    return ((element.GetModifiedMissileDamageForUsage(0) + 1.0f) * 2.0f) * arrowData.MaxDataValue;
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
                    float thrustDmg = element.GetModifiedThrustDamageForUsage(0);
                    float thrustSpd = element.GetModifiedThrustSpeedForUsage(0) * 0.01f;

                    float mult = thrustDmg == 0f ? 3f : 2f;
                    float swingDmg = element.GetModifiedSwingDamageForUsage(0) * mult;
                    float swingSpd = element.GetModifiedSwingSpeedForUsage(0) * 0.01f;

                    float reach = weapon.PrimaryWeapon.WeaponLength;

                    float handling = element.GetModifiedHandlingForUsage(0);

                    float weight = element.Weight;

                    return (swingDmg * swingSpd) + (thrustDmg * thrustSpd) + (reach * 0.5f) + (handling * 0.75f) - (weight * 0.5f);
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
                    float thrustDmg = element.GetModifiedThrustDamageForUsage(0);
                    float thrustSpd = element.GetModifiedThrustSpeedForUsage(0) * 0.01f;

                    float mult = thrustDmg == 0f ? 3f : 2f;
                    float swingDmg = element.GetModifiedSwingDamageForUsage(0) * mult;
                    float swingSpd = element.GetModifiedSwingSpeedForUsage(0) * 0.01f;

                    float reach = weapon.PrimaryWeapon.WeaponLength;

                    float handling = element.GetModifiedHandlingForUsage(0);

                    float weight = element.Weight;

                    return (swingDmg * swingSpd) + (thrustDmg * thrustSpd) + (reach * 0.75f) + (handling * 0.75f) - (weight * 0.5f);
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
                    float thrustDmg = element.GetModifiedThrustDamageForUsage(0);
                    float thrustSpd = element.GetModifiedThrustSpeedForUsage(0) * 0.01f;

                    float swingDmg = element.GetModifiedSwingDamageForUsage(0);
                    float swingSpd = element.GetModifiedSwingSpeedForUsage(0) * 0.01f;

                    float swingMult = thrustDmg == 0f ? 3f : 2.0f;
                    float thrustMult = swingDmg == 0f ? 3f : 1.0f;

                    thrustDmg *= thrustMult;
                    swingDmg *= swingMult;

                    float reach = weapon.PrimaryWeapon.WeaponLength;

                    float handling = element.GetModifiedHandlingForUsage(0);

                    float weight = element.Weight;

                    return (swingDmg * swingSpd) + (thrustDmg * thrustSpd) + (reach * 0.75f) + (handling * 0.75f) - (weight * 0.5f);
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
